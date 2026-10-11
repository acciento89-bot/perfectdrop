using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using Kamilunavo.PerfectDrop.Gameplay;
namespace Kamilunavo.PerfectDrop.Monetization
{
    public sealed class StorePurchases : MonoBehaviour
    {
        private StoreController _store;
        private readonly StoreReconnectGate _reconnect=new();
        private bool _restoring;
        private static bool QaNetworkDisabled
        {
            get
            {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                return !string.IsNullOrEmpty(StackSave.QaKey);
#else
                return false;
#endif
            }
        }
        public bool CanRetry=>!QaNetworkDisabled && !OtherProviderPresenting && Application.isMobilePlatform&&!Ready&&!Busy&&!IsPresenting&&_reconnect.CanBegin(Time.realtimeSinceStartupAsDouble);
        public void RetryConnection(){if(CanRetry)Connect();}
        private StackGame _game;
        private readonly HashSet<string> _fetched=new();
        private readonly HashSet<string> _deferred=new();
        public event Action Changed;
        public bool Ready {get;private set;}
        public bool IsPresenting {get;private set;}
        public bool Busy {get;private set;}
        public string Status {get;private set;}="";
        private static string T(string de,string en)=>UI.StackHud.T(de,en);
        public void Initialize(StackGame game)
        {
            _game=game;
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if(!string.IsNullOrEmpty(StackSave.QaKey)){SetStatus(T("Store im Prüflauf deaktiviert","Store disabled during QA"));return;}
#endif
            if(!Application.isMobilePlatform){SetStatus(T("Käufe auf iOS und Android verfügbar","Purchases available on iOS and Android"));return;}
            Connect();
        }
        private async void Connect()
        {
            if(QaNetworkDisabled||Ready||Busy||!_reconnect.TryBegin(Time.realtimeSinceStartupAsDouble))return;
            Busy=true;SetStatus(T("Store wird verbunden …","Connecting to store …"));
            try
            {
                if(_store==null){_store=UnityIAPServices.StoreController();
                _store.OnStoreDisconnected+=Disconnected;
                _store.OnProductsFetched+=ProductsFetched;
                _store.OnProductsFetchFailed+=ProductsFailed;
                _store.OnPurchasesFetched+=PurchasesFetched;
                _store.OnPurchasesFetchFailed+=PurchasesFailed;
                _store.OnPurchasePending+=Pending;
                _store.OnPurchaseFailed+=Failed;
                _store.OnPurchaseDeferred+=Deferred;
                _store.OnPurchaseConfirmed+=Confirmed;
                // Own pending-order handling also covers restoration, without double dispatch.
                _store.ProcessPendingOrdersOnPurchasesFetched(false);}
                await _store.Connect();
                if(this==null||QaNetworkDisabled)return;
                if(_store.GetConnectionState()!=ConnectionState.Connected){Unavailable(T("Store gerade nicht erreichbar","Store currently unavailable"));return;}
                _store.FetchProductsWithNoRetries(new List<ProductDefinition>{
                    new(CommerceRules.Starter,ProductType.NonConsumable),
                    new(CommerceRules.Collection,ProductType.NonConsumable),
                    new(CommerceRules.RemoveAds,ProductType.NonConsumable)});
            }
            catch(Exception){if(this!=null)Unavailable(T("Store gerade nicht erreichbar","Store currently unavailable"));}
            finally{_reconnect.EndAttempt();}
        }
        public string Price(string id)=>_store?.GetProductById(id)?.metadata?.localizedPriceString??"";
        public bool Owned(string id)=>_game!=null && (_game.Profile.Commerce.Entitlements&CommerceRules.ProductBit(id))!=0;
        private bool OtherProviderPresenting=>(_game?.Videos?.IsPresenting??false)||(_game?.Interstitials?.IsPresenting??false);
        public bool CanBuy(string id)=>!QaNetworkDisabled && !OtherProviderPresenting && Ready && !Busy && !_deferred.Contains(id) && !Owned(id) && CommerceRules.KnownProduct(id) &&
            _store?.GetProductById(id)?.availableToPurchase==true && !string.IsNullOrWhiteSpace(Price(id));
        public void Buy(string id)
        {
            if(!CanBuy(id))return;
            _game.Save();Busy=true;IsPresenting=true;SetStatus(T("Kauf wird geöffnet …","Opening purchase …"));
            try{_store.PurchaseProduct(id);}catch(Exception){Busy=false;SetStatus(T("Kauf konnte nicht gestartet werden","Purchase could not start"));}
        }
        public void Restore()
        {
            if(QaNetworkDisabled || _store==null || !Ready || Busy || OtherProviderPresenting)return;
            PresentRestore(_store.RestoreTransactions);
        }
        private void PresentRestore(Action<Action<bool,string>> restore)
        {
            _game.Save();_restoring=true;Busy=true;IsPresenting=true;SetStatus(T("Käufe werden wiederhergestellt …","Restoring purchases …"));
            try{restore((ok,_)=>
            {
                if(this==null)return;
                _restoring=false;IsPresenting=false;
                if(!ok){Busy=false;SetStatus(T("Wiederherstellung nicht verfügbar","Restore unavailable"));}
                else Changed?.Invoke();
            });}catch(Exception){_restoring=false;Busy=false;SetStatus(T("Wiederherstellung nicht verfügbar","Restore unavailable"));}
        }
        private void ProductsFetched(List<Product> products)
        {
            _fetched.Clear();foreach(var product in products)if(CommerceRules.KnownProduct(product.definition.id))_fetched.Add(product.definition.id);
            Ready=_fetched.Count>0;
            if(!Ready){Unavailable(T("Shop ist derzeit nicht verfügbar","Shop currently unavailable"));return;}
            _store.FetchPurchases();
        }
        private void PurchasesFetched(Orders orders)
        {
            var active=new HashSet<string>();_deferred.Clear();var pendingFailure=false;
            foreach(var order in orders.PendingOrders)
            {
                var fulfilled=Fulfill(order);pendingFailure|=!fulfilled;
                if(fulfilled)foreach(var item in order.CartOrdered.Items())if(CommerceRules.KnownProduct(item.Product.definition.id))active.Add(item.Product.definition.id);
            }
            var previousCommerce=JsonUtility.ToJson(_game.Profile.Commerce);var previousStyles=_game.Profile.OwnedStyles;var previousSelection=_game.Profile.Style;
            foreach(var order in orders.ConfirmedOrders)
                foreach(var item in order.CartOrdered.Items())
                {
                    var id=item.Product.definition.id;
                    if(!CommerceRules.KnownProduct(id))continue;
                    active.Add(id);CommerceRules.RestoreEntitlement(_game.Profile,id);
                }
            foreach(var order in orders.DeferredOrders)
                foreach(var item in order.CartOrdered.Items())if(CommerceRules.KnownProduct(item.Product.definition.id))_deferred.Add(item.Product.definition.id);
            try
            {
                // A partial catalog response is not evidence that another owned product was revoked.
                if(!pendingFailure && _fetched.Contains(CommerceRules.Starter) && _fetched.Contains(CommerceRules.Collection) && _fetched.Contains(CommerceRules.RemoveAds))CommerceRules.ReconcileEntitlements(_game.Profile,active);
                StackSave.Save(_game.Profile);
                Busy=false;SetStatus(pendingFailure?T("Kauf noch offen – später wiederherstellen","Purchase pending – restore later"):_deferred.Count>0?T("Ein Kauf wartet auf Freigabe","A purchase is awaiting approval"):T("Shop bereit","Shop ready"));
                _game.RefreshProfileStyle();_game.Hud.Refresh();
            }
            catch(Exception){_game.Profile.Commerce=JsonUtility.FromJson<CommerceProfile>(previousCommerce);_game.Profile.OwnedStyles=previousStyles;_game.Profile.Style=previousSelection;Busy=false;SetStatus(T("Käufe konnten nicht gespeichert werden","Purchases could not be saved"));}
        }
        private bool Fulfill(PendingOrder order)
        {
            var items=order.CartOrdered.Items();
            if(items.Count!=1 || items[0].Quantity!=1 || !CommerceRules.KnownProduct(items[0].Product.definition.id))
            {Busy=false;SetStatus(T("Kauf konnte nicht zugeordnet werden","Purchase could not be matched"));return false;}
            try
            {
                if(!CommerceRules.FulfillPending(_game.Profile,items[0].Product.definition.id,order.Info.TransactionID,StackSave.Save))
                {Busy=false;SetStatus(T("Kaufbestätigung fehlt","Purchase confirmation missing"));return false;}
                // Never acknowledge until the wallet and transaction marker have persisted together.
                _store.ConfirmPurchase(order);
                _deferred.Remove(items[0].Product.definition.id);
                _game.Hud.Refresh();return true;
            }
            catch(Exception){Busy=false;SetStatus(T("Kauf noch nicht abgeschlossen – bitte später wiederherstellen","Purchase pending – please restore later"));return false;}
        }
        private void Pending(PendingOrder order)
        {
            if(Fulfill(order)){Busy=false;SetStatus(T("Kauf freigeschaltet","Purchase unlocked"));}
        }
        private void Confirmed(Order order){Busy=false;IsPresenting=_restoring;if(order is FailedOrder)SetStatus(T("Kaufbestätigung noch offen – später wiederherstellen","Purchase confirmation pending – restore later"));else Changed?.Invoke();}
        private void Failed(FailedOrder order){Busy=false;SetStatus(order.FailureReason==PurchaseFailureReason.UserCancelled?T("Kauf abgebrochen","Purchase cancelled"):T("Kauf nicht abgeschlossen","Purchase not completed"));}
        private void Deferred(DeferredOrder order)
        {
            foreach(var item in order.CartOrdered.Items())_deferred.Add(item.Product.definition.id);
            Busy=false;SetStatus(T("Kauf wartet auf Freigabe","Purchase awaiting approval"));
        }
        private void Disconnected(StoreConnectionFailureDescription failure)=>Unavailable(T("Store-Verbindung unterbrochen","Store disconnected"));
        private void ProductsFailed(ProductFetchFailed failure)=>Unavailable(T("Shop gerade nicht verfügbar","Shop currently unavailable"));
        private void PurchasesFailed(PurchasesFetchFailureDescription failure){Busy=false;SetStatus(T("Käufe konnten nicht geladen werden","Purchases could not be loaded"));}
        private void Unavailable(string message){_reconnect.Failed(Time.realtimeSinceStartupAsDouble);Ready=false;Busy=false;SetStatus(message);}
        private void SetStatus(string message){if(!Busy&&!_restoring)IsPresenting=false;Status=message;Changed?.Invoke();}
        private void OnDestroy()
        {
            if(_store==null)return;
            _store.OnStoreDisconnected-=Disconnected;_store.OnProductsFetched-=ProductsFetched;_store.OnProductsFetchFailed-=ProductsFailed;
            _store.OnPurchasesFetched-=PurchasesFetched;_store.OnPurchasesFetchFailed-=PurchasesFailed;
            _store.OnPurchasePending-=Pending;_store.OnPurchaseFailed-=Failed;_store.OnPurchaseDeferred-=Deferred;_store.OnPurchaseConfirmed-=Confirmed;
        }
    }
}
