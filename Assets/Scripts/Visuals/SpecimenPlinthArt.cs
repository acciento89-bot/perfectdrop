using UnityEngine;

namespace Kamilunavo.PerfectDrop.Visuals
{
    // Original modeled display furniture uses the live finish; never gameplay collision.
    public static class SpecimenPlinthArt
    {
        public static void Build(Transform root, int style)
        {
            int kind=style==5?5:style==3?3:0;
            var asset=Resources.Load<GameObject>("Art/ConceptV3/Plinth-"+kind);
            if(asset!=null){var instance=Object.Instantiate(asset,root,false);foreach(var node in instance.GetComponentsInChildren<Transform>())node.gameObject.layer=StackSpecimenStudio.Layer;return;}
            BuildOriginal(root,style);
        }

        public static void BuildOriginal(Transform root, int style)
        {
            var owner = root.gameObject.AddComponent<TerraceArtOwner>();
            var finish = StackStylePalette.Get(style);
            var body = new Material(Resources.Load<Shader>("PerfectDropSurface"))
            { name = "OriginalSculptedSpecimenBase", color = style == 5 ? new Color(.84f,.80f,.70f) : new Color(.065f,.095f,.12f) };
            body.SetFloat("_Metallic", style == 5 ? .08f : .62f); body.SetFloat("_Glossiness", .62f);
            if (style == 5) { body.SetFloat("_StoneFinish",1); body.SetTexture("_StoneAlbedo",Resources.Load<Texture2D>("Art/AuthoredIvoryLimestone")); }
            else { body.mainTexture = Resources.Load<Texture2D>("Art/ConceptV2/NavySatinAlbedo"); body.SetFloat("_OwnedNavy",1); }
            owner.Own(body);
            var gold = new Material(Resources.Load<Shader>("PerfectDropSurface"))
            { name = "OriginalSpecimenGiltRelief", color = finish.Accent };
            gold.SetFloat("_Metallic",.78f); gold.SetFloat("_Glossiness",.74f);
            gold.SetFloat("_OwnedGold",1); gold.SetFloat("_GoldFinish",finish.GoldFinish);
            gold.SetTexture("_GoldAlbedo",Resources.Load<Texture2D>("Art/ConceptV2/ChampagneGoldAlbedo")); owner.Own(gold);

            var stone = new TerraceArtGeometry.Buffer();
            stone.Lathe(Vector3.zero,new[]{-.99f,-.92f,-.87f,-.79f,-.60f,-.55f,-.50f},new[]{1.36f,1.36f,1.30f,1.23f,1.23f,1.18f,1.18f},48);
            var relief = new TerraceArtGeometry.Buffer();
            relief.Lathe(Vector3.zero,new[]{-.89f,-.87f,-.84f},new[]{1.32f,1.32f,1.29f},48);
            relief.Ring(Vector3.up*-.54f,1.18f,.035f,48);
            relief.Ring(Vector3.up*-.925f,1.35f,.025f,48);
            for(int i=0;i<16;i++)
            {
                float angle=i*Mathf.PI/8;var at=new Vector3(Mathf.Cos(angle)*1.235f,-.70f,Mathf.Sin(angle)*1.235f);
                stone.Lathe(at,new[]{-.08f,.08f},new[]{.025f,.025f},6);
                relief.Ellipsoid(at+Vector3.up*.015f,new Vector3(.050f,.095f,.050f),8,5);
            }
            stone.Upload(root,"AuthoredSpecimenStoneFoot",body,owner);
            relief.Upload(root,"OriginalSpecimenSculptedGilt",gold,owner);
            foreach(var name in new[]{"AuthoredSpecimenStoneFoot","OriginalSpecimenSculptedGilt"})
                root.Find(name).gameObject.layer=StackSpecimenStudio.Layer;
        }
    }
}
