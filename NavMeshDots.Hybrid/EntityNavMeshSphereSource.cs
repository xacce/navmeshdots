#if UNITY_EDITOR
using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

namespace NavMeshDots.Hybrid
{
    public class EntityNavMeshSphereSource : MonoBehaviour, IEntityNavMeshSourceProvider
    {
        [SerializeField] private float radius = 1f;
        [SerializeField] private int m_Area;

        private void OnDrawGizmosSelected()
        {
            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
            Gizmos.DrawWireSphere(Vector3.zero, radius);
            Gizmos.matrix = Matrix4x4.identity;
        }

        public bool TryGetSource(out _NavMeshBuildSource result)
        {
            result = new _NavMeshBuildSource()
            {
                m_Area = m_Area,
                m_Size = Vector3.one,
                m_Transform = Matrix4x4.TRS(transform.position, transform.rotation, new Vector3(radius, radius, radius)),
                m_Shape = NavMeshBuildSourceShape.Sphere,
            };
            return true;
        }
    }
}
#endif