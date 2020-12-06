using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; 

//Tagging component for use with LocalNavMeshBuilder
//Supports mesh-filter and terrain - can be extended to physics and/or primitives
[DefaultExecutionOrder(-200)]
public class OurNavMeshSourceTag : MonoBehaviour
{
    //Global containers for all active mesh/terrain tags
    public static List<MeshFilter> meshes = new List<MeshFilter>();
    //public static List<TerrainChunk> terrains = new List<TerrainChunk>();

    void OnEnable()
    {
        var m = GetComponent<MeshFilter>(); 
        if (m != null)
        {
            meshes.Add(m); 
        }
        /*var t = GetComponent<TerrainChunk>(); 
        if (t != null)
        {
            terrains.Add(t); 
        }*/
    }

    void OnDisable()
    {
        var m = GetComponent<MeshFilter>(); 
        if (m != null)
        {
            meshes.Remove(m); 
        }
        /*var t = GetComponent<TerrainChunk>(); 
        if (t != null)
        {
            terrains.Remove(t); 
        }*/
    }

    //Collect all navmesh build sources for enabled objects tagged by this component
    public static void Collect(ref List<NavMeshBuildSource> sources)
    {
        sources.Clear(); 

        for (var i = 0; i < meshes.Count; ++i)
        {
            var mf = meshes[i];
            if (mf == null) continue;

            var m = mf.sharedMesh;
            if (m == null) continue;

            var s = new NavMeshBuildSource();
            s.shape = NavMeshBuildSourceShape.Mesh;
            s.sourceObject = m;
            s.transform = mf.transform.localToWorldMatrix;
            s.area = 0;
            sources.Add(s); 
        }

        /*for (var i = 0; i < terrains.Count; ++i)
        {
            var t = terrains[i];
            if (t == null) continue;

            var s = new NavMeshBuildSource();
            s.shape = NavMeshBuildSourceShape.Terrain;
            s.sourceObject = t.meshObject; 
            //Terrain system only supports translation - so we pass translation only to back-end
            s.transform = Matrix4x4.TRS(t.meshObject.transform.position, Quaternion.identity, Vector3.one);
            s.area = 0;
            sources.Add(s); 
        }*/
    }
}
