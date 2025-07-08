#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.AI;

namespace NavMeshDots.Hybrid
{
    public class EntityNavMeshBoxSource : MonoBehaviour, IEntityNavMeshSourceProvider
    {
        [SerializeField] private Vector3 offset;
        [SerializeField] private Vector3 extraScale;
        [SerializeField] private int m_Area;

        private void OnDrawGizmosSelected()
        {
            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
            Gizmos.DrawWireCube(offset, transform.lossyScale + extraScale);
            Gizmos.matrix = Matrix4x4.identity;
        }

        public bool TryGetSource(out _NavMeshBuildSource result)
        {
            var position = transform.TransformPoint(offset);
            result = new _NavMeshBuildSource()
            {
                m_Area = m_Area,
                m_Size = transform.lossyScale + extraScale,
                m_Transform = Matrix4x4.TRS(position, transform.rotation, Vector3.one),
                m_Shape = NavMeshBuildSourceShape.Box,
            };
            return true;
        }
    }
}
#endif