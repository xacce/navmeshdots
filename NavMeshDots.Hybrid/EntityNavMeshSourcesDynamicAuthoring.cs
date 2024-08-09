#if UNITY_EDITOR
using NavMeshDots.Runtime;
using Unity.Entities;
using UnityEngine;

namespace NavMeshDots.Hybrid
{
    [DisallowMultipleComponent]
    public class EntityNavMeshSourcesDynamicAuthoring : MonoBehaviour
    {
        class B : Baker<EntityNavMeshSourcesDynamicAuthoring>
        {
            public override void Bake(EntityNavMeshSourcesDynamicAuthoring sourcesStaticAuthoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddBuffer<NavMeshSourceElement>(entity);
            }
        }
    }
}
#endif