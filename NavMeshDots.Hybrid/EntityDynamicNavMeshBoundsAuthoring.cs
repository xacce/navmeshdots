#if UNITY_EDITOR
using NavMeshDots.Runtime;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;


namespace NavMeshDots.Hybrid
{
    public class EntityDynamicNavMeshBoundsAuthoring : MonoBehaviour
    {
        [SerializeField] private float3 boundsCenter;
        [SerializeField] private float3 boundsExtents;

        class B : Baker<EntityDynamicNavMeshBoundsAuthoring>
        {

            public override void Bake(EntityDynamicNavMeshBoundsAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.WorldSpace);
                AddComponent<EntityNavMeshBounds>(entity);
                SetComponent(
                    entity,
                    new EntityNavMeshBounds()
                    {
                        bounds = new Bounds(authoring.boundsCenter, authoring.boundsExtents)
                    });
            }
        }
    }
}
#endif