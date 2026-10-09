using UnityEngine;
using UnityEngine.UI;
using Kamilunavo.PerfectDrop.Monetization;
namespace Kamilunavo.PerfectDrop.UI
{
    public sealed partial class StackHud
    {
        private RectTransform _shop, _shopViewport, _shopContent;
        private ScrollRect _shopScroll;
        private Text _shopStatus;
        private Button _starterBuy, _collectionBuy, _restorePurchases, _retryPurchases, _video, _privacy;
        private int _shopSecond=-1;
        private Button[] _premiumStyles;
        public bool ShopOpen=>_shop!=null && _shop.gameObject.activeSelf;
        private void BuildCommerceMenu()
        {
            _shop=UiFactory.Panel(_safe,"PremiumShop",Navy,Vector2.zero,Vector2.one);
            UiFactory.Label(_shop,"Title",T("EXTRA-DESIGNS","EXTRA DESIGNS"),42,new Vector2(.06f,.90f),new Vector2(.94f,.98f),TextAnchor.MiddleLeft,Gold,FontStyle.Bold);
            _shopStatus=UiFactory.Label(_shop,"StoreStatus","",25,new Vector2(.06f,.80f),new Vector2(.94f,.9f),TextAnchor.MiddleLeft,Color.white);
            var scrollObject=new GameObject("ShopScroll",typeof(RectTransform),typeof(ScrollRect));
            scrollObject.transform.SetParent(_shop,false);Set((RectTransform)scrollObject.transform,.04f,.19f,.96f,.78f);
            _shopScroll=scrollObject.GetComponent<ScrollRect>();_shopScroll.horizontal=false;_shopScroll.movementType=ScrollRect.MovementType.Clamped;
            _shopViewport=UiFactory.Panel(scrollObject.transform,"ShopViewport",new Color(.025f,.045f,.08f),Vector2.zero,Vector2.one);
            _shopViewport.gameObject.AddComponent<RectMask2D>();
            _shopContent=new GameObject("ShopContent",typeof(RectTransform)).GetComponent<RectTransform>();_shopContent.SetParent(_shopViewport,false);
            _shopContent.anchorMin=new Vector2(0,1);_shopContent.anchorMax=Vector2.one;_shopContent.pivot=new Vector2(.5f,1);
            _shopScroll.viewport=_shopViewport;_shopScroll.content=_shopContent;
            _starterBuy=ShopButton("BuyStarter","",()=>Game.Purchases?.Buy(CommerceRules.Starter));
            _collectionBuy=ShopButton("BuyCollection","",()=>Game.Purchases?.Buy(CommerceRules.Collection));
            _starterBuy.GetComponent<Image>().color=new Color(.24f,.13f,.065f);_collectionBuy.GetComponent<Image>().color=new Color(.19f,.10f,.32f);
            TowerPreviewGraphic.AddStyle(_starterBuy.transform,"StarterSpecimen",4,new Vector2(.02f,.04f),new Vector2(.25f,.96f),4);
            TowerPreviewGraphic.AddStyle(_collectionBuy.transform,"CollectionSpecimen",6,new Vector2(.02f,.04f),new Vector2(.25f,.96f),5);
            foreach(var pack in new[]{_starterBuy,_collectionBuy})Set((RectTransform)pack.transform.Find("Label"),.28f,.10f,.97f,.90f);
            _premiumStyles=new Button[4];
            for(var i=0;i<4;i++)
            {
                var style=i+4;_premiumStyles[i]=ShopButton("PremiumStyle"+style,"",()=>{if(Game.SelectStyle(style))Game.UiClick();RefreshCommerce();});
                var rect=(RectTransform)_premiumStyles[i].transform;Set((RectTransform)rect.Find("Label"),.18f,.05f,.98f,.95f);
                TowerPreviewGraphic.AddStyle(rect,"DesignPreview",style,new Vector2(.02f,.04f),new Vector2(.25f,.96f),3);
                Set((RectTransform)rect.Find("Label"),.28f,.10f,.97f,.90f);
            }
            _video=ShopButton("OptionalVideo","",()=>Game.Videos?.Watch());
            _privacy=ShopButton("AdPrivacy",T("WERBE-DATENSCHUTZ","AD PRIVACY OPTIONS"),()=>Game.Videos?.ShowPrivacy());
            _retryPurchases=ShopButton("RetryStore",T("STORE ERNEUT VERBINDEN","RECONNECT STORE"),()=>Game.Purchases?.RetryConnection());
            _restorePurchases=ShopButton("RestorePurchases",T("KÄUFE WIEDERHERSTELLEN","RESTORE PURCHASES"),()=>Game.Purchases?.Restore());
            UiFactory.Button(_shop,"ShopBack",T("ZURÜCK ZU DESIGNS","BACK TO DESIGNS"),Gold,Navy,new Vector2(.08f,.04f),new Vector2(.92f,.17f),ShowStyles);
            UiFactory.Button(_styles,"ExtraDesigns",T("EXTRA-DESIGNS / SHOP","EXTRA DESIGNS / SHOP"),new Color(.20f,.13f,.33f),Color.white,new Vector2(.08f,.16f),new Vector2(.92f,.27f),ShowShop);
            for(var i=0;i<4;i++)Set((RectTransform)_styleButtons[i].transform,.08f,.66f-i*.125f,.92f,.765f-i*.125f);
            _shop.gameObject.SetActive(false);
        }
        private Button ShopButton(string name,string title,UnityEngine.Events.UnityAction action)=>UiFactory.Button(_shopContent,name,title,new Color(.13f,.20f,.30f),Color.white,Vector2.zero,Vector2.one,action);
        public void ShowShop()
        {
            Game.Save();HideMenus();_shop.gameObject.SetActive(true);_shopScroll.verticalNormalizedPosition=1;Refresh();RefreshCommerce();Game.Videos?.Prepare();
        }
        public void RefreshCommerce()
        {
            if(_shop==null)return;
            var store=Game.Purchases;
            var videos=Game.Videos;
            _video.interactable=videos?.CanWatch==true && !(store?.Busy??false);
            var wait=RewardRules.WaitSeconds(Game.Profile,System.DateTime.UtcNow);var remaining=RewardRules.Remaining(Game.Profile,System.DateTime.UtcNow);
            SetLabel(_video,T("FREIWILLIGES VIDEO · +50 COINS","OPTIONAL VIDEO · +50 COINS")+"\n"+(remaining==0?T("MORGEN WIEDER","COME BACK TOMORROW"):wait>0?wait+" s":videos?.Status??T("GERADE NICHT VERFÜGBAR","CURRENTLY UNAVAILABLE")));
            _privacy.gameObject.SetActive(videos?.PrivacyRequired==true);_privacy.interactable=videos!=null && !videos.IsPresenting;
            _shopStatus.text=Game.Profile.Coins+" COINS\n"+(store?.Status??T("Store wird vorbereitet …","Preparing store …"));
            PurchaseLabel(_starterBuy,CommerceRules.Starter,T("STARTER · KUPFER + 500 COINS","STARTER · COPPER + 500 COINS"));
            PurchaseLabel(_collectionBuy,CommerceRules.Collection,T("NEON-KOLLEKTION · 3 DESIGNS","NEON COLLECTION · 3 DESIGNS"));
            var names=new[]{T("KUPFER","COPPER"),T("PERLE","PEARL"),T("VIOLETT","VIOLET"),T("SOLAR","SOLAR")};
            for(var i=0;i<4;i++)
            {
                var style=i+4;var owned=(Game.Profile.OwnedStyles&(1<<style))!=0;
                SetLabel(_premiumStyles[i],names[i]+" · "+(Game.Profile.Style==style?T("AKTIV","ACTIVE"):owned?T("AUSWÄHLEN","SELECT"):T("IM PAKET ENTHALTEN","INCLUDED IN PACK")));
                _premiumStyles[i].GetComponent<Image>().color=Game.Profile.Style==style?new Color(.24f,.18f,.09f):new Color(.045f,.075f,.13f,.98f);
                _premiumStyles[i].interactable=owned && !(store?.Busy??false);
            }
            _retryPurchases.gameObject.SetActive(store!=null && !store.Ready);_retryPurchases.interactable=store?.CanRetry==true;
            _restorePurchases.interactable=store!=null && store.Ready && !store.Busy;
        }
        private void PurchaseLabel(Button button,string id,string title)
        {
            var store=Game.Purchases;var price=store?.Price(id)??"";
            SetLabel(button,title+"\n"+(store?.Owned(id)==true?T("GEKAUFT","OWNED"):string.IsNullOrWhiteSpace(price)?T("GERADE NICHT VERFÜGBAR","CURRENTLY UNAVAILABLE"):price));
            button.interactable=store?.CanBuy(id)==true;
        }
        private void LayoutCommerce(Rect pane)
        {
            if(_shop==null)return;
            Place(_shop,pane,new Rect(.04f,.02f,.92f,.96f));
            LayoutShopRows();
        }
        private void UpdateCommerce()
        {
            if(!ShopOpen)return;var second=(int)Time.unscaledTime;if(second==_shopSecond)return;_shopSecond=second;Game.Videos?.Prepare();RefreshCommerce();LayoutShopRows();
        }
        private void LayoutShopRows()
        {
            Canvas.ForceUpdateCanvases();
            var canvas=_shop.GetComponentInParent<Canvas>();
            // Minimum 52 logical screen points even on short landscape screens.
            var row=Mathf.Max(154f,56f*UiMetrics.PointScale/canvas.scaleFactor);var gap=14f;
            var buttons=System.Array.FindAll(_shopContent.GetComponentsInChildren<Button>(true),button=>button.gameObject.activeSelf);
            _shopContent.sizeDelta=new Vector2(0,buttons.Length*(row+gap)+gap);
            for(var i=0;i<buttons.Length;i++)
            {
                var rect=(RectTransform)buttons[i].transform;rect.anchorMin=new Vector2(.02f,1);rect.anchorMax=new Vector2(.98f,1);rect.pivot=new Vector2(.5f,1);
                rect.sizeDelta=new Vector2(0,row);rect.anchoredPosition=new Vector2(0,-gap-i*(row+gap));
                buttons[i].GetComponentInChildren<Text>().fontSize=29;
            }
        }
    }
}
