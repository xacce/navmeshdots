#if UNITY_EDITOR
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

namespace NavMeshDots.Hybrid
{
    public abstract class AbstractEntityNavMeshDataAuthoring : MonoBehaviour
    {
           
        [SerializeField] private float3 center_s;
        [SerializeField] private float3 bounds_s;
        [SerializeField] protected NavMeshData navMesh_s;

        public float3 center => center_s;

        public float3 bounds => bounds_s;

        public NavMeshData navMesh => navMesh_s;
    }
}
#endif