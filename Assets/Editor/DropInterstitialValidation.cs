#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using Kamilunavo.PerfectDrop.Gameplay;
using Kamilunavo.PerfectDrop.Monetization;
using Kamilunavo.PerfectDrop.UI;
namespace Kamilunavo.PerfectDrop.Editor
{
    public static class DropInterstitialValidation
    {
        static int checks;
        static void Check(bool ok,string label) { checks++; if(!ok) throw new InvalidOperationException(label); }
        public static void BaselineRed() { Check(CommerceRules.KnownProduct("com.kamilunavo.perfectdrop.removeads"),"permanent RemoveAds product is supported"); }
        public static void Validate()
        {
            var previousQa=StackSave.QaKey;
            var key="perfectdrop.interstitial.qa."+Guid.NewGuid().ToString("N");StackSave.QaKey=key;
            try{ValidateIsolated();}finally{PlayerPrefs.DeleteKey(key);StackSave.QaKey=previousQa;}
        }
        private static void ValidateIsolated()
        {
            checks=0; BaselineRed(); const string id="com.kamilunavo.perfectdrop.removeads";
            var profile=StackSave.Parse("{\"Schema\":2,\"Coins\":80,\"OwnedStyles\":7,\"Style\":2}");
            Check(CommerceRules.ApplyPending(profile,id,"adfree-one"),"valid purchase grants entitlement");
            Check(profile.Coins==80&&profile.Style==2&&profile.OwnedStyles==7&&(profile.Commerce.Entitlements&4)!=0,"ad removal preserves wallet and designs");
            Check(!CommerceRules.ApplyPending(profile,id,"adfree-one"),"purchase replay is idempotent");
            var loaded=StackSave.Parse(JsonUtility.ToJson(profile));Check((loaded.Commerce.Entitlements&4)!=0,"RemoveAds survives save reload");
            var restored=new StackProfile();Check(CommerceRules.RestoreEntitlement(restored,id)&&restored.Coins==0&&restored.OwnedStyles==1,"restore has no bonus currency or premium design");
            CommerceRules.ReconcileEntitlements(restored,new[]{id});Check((restored.Commerce.Entitlements&4)!=0,"authoritative restore retains purchase");
            CommerceRules.ReconcileEntitlements(restored,Array.Empty<string>());Check((restored.Commerce.Entitlements&4)==0,"authoritative revocation removes purchase");
            var failed=new StackProfile{Coins=39};try{CommerceRules.FulfillPending(failed,id,"disk",_=>throw new InvalidOperationException("disk"));throw new Exception("missing persistence failure");}catch(InvalidOperationException){}
            Check(failed.Commerce.Entitlements==0&&failed.Coins==39&&failed.Commerce.FulfilledTransactions.Count==0,"failed persistence rolls back purchase before store acknowledgement");
            Check(CommerceRules.FulfillPending(failed,id,"disk",_=>{}),"failed purchase can retry");
            var type=typeof(CommerceRules).Assembly.GetType("Kamilunavo.PerfectDrop.Monetization.InterstitialPolicy");Check(type!=null,"actual policy exists");var gate=Activator.CreateInstance(type,new object[]{100.0});
            bool Record(string token,bool tutorial=false,bool eligible=true)=>(bool)type.GetMethod("RecordCompletion").Invoke(gate,new object[]{token,tutorial,eligible});
            bool Can(double now,bool owned=false,bool available=true,bool busy=false,bool focused=true,bool result=true)=>(bool)type.GetMethod("CanShow").Invoke(gate,new object[]{now,owned,available,busy,focused,result});
            Check(!Record("tutorial",true)&&!Record("fail",false,false),"tutorial and failures do not count");
            Check(Record("1")&&Record("2")&&!Can(900),"minimum three successes required");Check(!Record("1"),"duplicate result does not count");Check(Record("3")&&!Can(279.99)&&Can(280),"first180seconds grace and exact boundary");
            Check(!Can(500,true)&&!Can(500,false,false)&&!Can(500,false,true,true)&&!Can(500,false,true,false,false)&&!Can(500,false,true,false,true,false),"ownership nofill provider background and active-play guards");
            Check(!Can(double.NaN)&&!Can(double.PositiveInfinity),"invalid clocks fail closed");
            Check((bool)type.GetMethod("TryReserve").Invoke(gate,new object[]{280.0,false,true,false,true,true})&&!Can(280),"one provider reservation");
            type.GetMethod("CancelReservation").Invoke(gate,null);Check(Can(280),"failed opening does not spend quota");type.GetMethod("TryReserve").Invoke(gate,new object[]{280.0,false,true,false,true,true});type.GetMethod("Impression").Invoke(gate,new object[]{280.0});type.GetMethod("CancelReservation").Invoke(gate,null);
            Check(!Can(900),"real impression resets successful completion count");for(int i=0;i<3;i++)Check(Record("next"+i),"genuine repeated successful play counted once");Check(!Can(459.99)&&Can(460),"three minute separation after impression");
            type.GetMethod("RewardedOpened").Invoke(gate,new object[]{470.0});Check(!Can(649.99)&&Can(650),"rewarded opening delays next forced ad without consuming earned quota");
            string previousLanguage=PlayerPrefs.GetString("perfectdrop.language","");try{foreach(var lang in new[]{"de","en"}){PlayerPrefs.SetString("perfectdrop.language",lang);UiFixture();}}finally{if(previousLanguage.Length==0)PlayerPrefs.DeleteKey("perfectdrop.language");else PlayerPrefs.SetString("perfectdrop.language",previousLanguage);}
            Debug.Log("DROP_INTERSTITIALS_PASS checks="+checks);
        }
        static readonly BindingFlags Private=BindingFlags.Instance|BindingFlags.NonPublic;
        static void Set(object owner,string field,object value)=>owner.GetType().GetField(field,Private).SetValue(owner,value);
        static void UiFixture()
        {
            var root=new GameObject("AdShopFixture");Canvas canvas=null;
            try
            {
                var game=root.AddComponent<StackGame>();Set(game,"<Profile>k__BackingField",new StackProfile{TutorialComplete=true});Set(game,"<Run>k__BackingField",new StackRun());Set(game,"<Level>k__BackingField",StackCampaign.Level(1));
                var hud=root.AddComponent<StackHud>();Set(game,"<Hud>k__BackingField",hud);hud.Build(game);hud.ShowHome();var safe=(RectTransform)typeof(StackHud).GetField("_safe",Private).GetValue(hud);canvas=safe.GetComponentInParent<Canvas>();
                var direct=safe.Find("LevelMap/DirectShop").GetComponent<Button>();Check(direct.gameObject.activeInHierarchy,"Shop directly visible in main menu");direct.onClick.Invoke();Check(hud.ShopOpen,"direct Shop opens premium surface");
                var content=safe.Find("PremiumShop/ShopScroll/ShopViewport/ShopContent");Check(content.GetChild(0).name=="RemoveAds"&&content.GetChild(1).name=="RestorePurchases","ad removal and restore appear before premium and design lists");
                var remove=content.Find("RemoveAds").GetComponent<Button>();Check(!remove.interactable&&!remove.GetComponentInChildren<Text>().text.Contains("1.99")&&!remove.GetComponentInChildren<Text>().text.Contains("1,99"),"unavailable store never fakes a localized purchase price");
                safe.Find("PremiumShop/ShopBack").GetComponent<Button>().onClick.Invoke();Check(hud.HomeOpen&&!hud.ShopOpen,"direct Shop Back returns to main menu");
                var store=root.AddComponent<StorePurchases>();Set(game,"<Purchases>k__BackingField",store);store.Initialize(game);
                typeof(StorePurchases).GetMethod("Connect",Private).Invoke(store,null);store.RetryConnection();store.Buy(CommerceRules.RemoveAds);store.Restore();
                Check(!store.Busy&&!store.IsPresenting&&typeof(StorePurchases).GetField("_store",Private).GetValue(store)==null,"QA reconnect and purchase entries create no native store controller");
                var videos=root.AddComponent<RewardedVideos>();Set(game,"<Videos>k__BackingField",videos);videos.Initialize(game);
                Set(videos,"<IsPresenting>k__BackingField",true);
                var run=game.Run;game.StartChallenge();
                Check(!game.Profile.RunChallenge&&game.Run==run,"native consent blocks starting challenge before profile mutation");
                game.NewRun();Check(game.Run==run,"native consent blocks direct retry");
                Set(videos,"<IsPresenting>k__BackingField",false);
                var present=typeof(StorePurchases).GetMethod("PresentRestore",Private);
                present.Invoke(store,new object[]{(Action<Action<bool,string>>)(done=>{
                    Check(store.IsPresenting&&hud.ModalOpen,"native restore reserves provider and gameplay lock");
                    var consent=typeof(RewardedVideos).GetMethod("TryPresentConsent",Private);int opened=0;
                    Check(!(bool)consent.Invoke(videos,new object[]{(Action)(()=>opened++)})&&opened==0,"restore excludes delayed consent");done(false,"cancel");})});
                Check(!store.IsPresenting&&!store.Busy,"restore cancellation releases native ownership");
                present.Invoke(store,new object[]{(Action<Action<bool,string>>)(done=>throw new InvalidOperationException("native unavailable"))});
                Check(!store.IsPresenting&&!store.Busy,"restore exception releases native ownership");
                Action<bool,string> restored=null;
                present.Invoke(store,new object[]{(Action<Action<bool,string>>)(done=>restored=done)});
                Set(store,"<Busy>k__BackingField",false);
                typeof(StorePurchases).GetMethod("SetStatus",Private).Invoke(store,new object[]{"incidental purchase callback"});
                Check(store.IsPresenting,"incidental purchase callback cannot release still-open restore");
                restored(true,null);Check(!store.IsPresenting,"successful native restore callback releases presentation");
            }
            finally{if(canvas!=null)UnityEngine.Object.DestroyImmediate(canvas.gameObject);UnityEngine.Object.DestroyImmediate(root);}
        }
    }
}
#endif
