using System.Collections.Generic;
using UnityEngine;
namespace Kamilunavo.PerfectDrop.Visuals
{
    // District rebuilds replace this owned mesh geometry, never cached source meshes.
    public sealed class CityMeshOwner : MonoBehaviour
    {
        public readonly List<Mesh> Meshes=new();
        private void OnDestroy(){foreach(var mesh in Meshes)if(mesh!=null){if(Application.isPlaying)Destroy(mesh);else DestroyImmediate(mesh);}}
    }
}
