#if UNITY_EDITOR
using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

namespace NavMeshDots.Hybrid
{
    public class EntityNavMeshExcludeBoxSource : MonoBehaviour, IEntityNavMeshSourceProvider
    {
        [SerializeField] private Vector3 thinkness;
        [SerializeField] private int m_Area;

        private void OnDrawGizmosSelected()
        {
            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale + thinkness);
            Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
        }

        public bool TryGetSource(out _NavMeshBuildSource result)
        {
            result= new _NavMeshBuildSource()
            {
                m_Area = math.max(0, m_Area), //avoid unity crash if ModifierBox area value less zero
                m_Size = transform.localScale + thinkness,
                m_Transform = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one),
                m_Shape = NavMeshBuildSourceShape.ModifierBox,
            };
            return true;
        }
    }
}
#endif