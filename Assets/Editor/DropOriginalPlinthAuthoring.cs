#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using Kamilunavo.PerfectDrop.Visuals;

namespace Kamilunavo.PerfectDrop.Editor
{
    public static class DropOriginalPlinthAuthoring
    {
        public static void Author()
        {
            Directory.CreateDirectory("Assets/Art/ConceptV3");
            foreach(int style in new[]{0,3,5})
            {
                string prefab="Assets/Resources/Art/ConceptV3/Plinth-"+style+".prefab";
                string library="Assets/Art/ConceptV3/Plinth-"+style+".asset";
                if(File.Exists(prefab)||File.Exists(library))throw new InvalidOperationException("Original plinth already exists; preserve artist edits: "+prefab);
                var root=new GameObject("Original sculpted plinth "+style);
                try
                {
                    SpecimenPlinthArt.BuildOriginal(root.transform,style);
                    var container=new Material(Resources.Load<Shader>("PerfectDropSurface")){name="OriginalPlinthResourceLibrary"};
                    AssetDatabase.CreateAsset(container,library);
                    foreach(var renderer in root.GetComponentsInChildren<MeshRenderer>())
                    {
                        var material=new Material(renderer.sharedMaterial){name=renderer.sharedMaterial.name};
                        AssetDatabase.AddObjectToAsset(material,container);renderer.sharedMaterial=material;
                        var filter=renderer.GetComponent<MeshFilter>();var mesh=UnityEngine.Object.Instantiate(filter.sharedMesh);
                        mesh.name=filter.sharedMesh.name;AssetDatabase.AddObjectToAsset(mesh,container);filter.sharedMesh=mesh;
                    }
                    // The temporary owner releases only the originals; clones are persistent.
                    UnityEngine.Object.DestroyImmediate(root.GetComponent<TerraceArtOwner>());
                    foreach(var node in root.GetComponentsInChildren<Transform>())node.gameObject.layer=StackSpecimenStudio.Layer;
                    AssetDatabase.SaveAssets();PrefabUtility.SaveAsPrefabAsset(root,prefab);
                }
                finally{UnityEngine.Object.DestroyImmediate(root);}
            }
            AssetDatabase.SaveAssets();AssetDatabase.Refresh();
            Debug.Log("DROP_ORIGINAL_PLINTH_AUTHOR_PASS three editable original prefab/mesh/material assets consumed by the actual preview");
        }
    }
}
#endif
