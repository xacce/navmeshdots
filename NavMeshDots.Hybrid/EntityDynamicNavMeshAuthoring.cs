#if UNITY_EDITOR
using NavMeshDots.Runtime;
using Unity.Entities;
using UnityEngine;

namespace NavMeshDots.Hybrid
{
    public class EntityDynamicNavMeshAuthoring : MonoBehaviour
    {
        // [SerializeField] private PhysicsShapeAuthoring[] shapes_s = Array.Empty<PhysicsShapeAuthoring>();
        [SerializeField] private int agentTypeId = 0;
        [SerializeField] private bool buildAtstart = true;
        [SerializeField] private bool loadAfterBuild = true;

        class B : Baker<EntityDynamicNavMeshAuthoring>
        {
            public override void Bake(EntityDynamicNavMeshAuthoring sourcesAggregatorAuthoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent<EntityDynamicNavMeshData>(entity);
                AddComponent<DynamicTag>(entity);
                AddComponentObject(entity, new EntityNavMeshData());
                if (sourcesAggregatorAuthoring.buildAtstart) AddComponent<Build>(entity);
                if (sourcesAggregatorAuthoring.loadAfterBuild) AddComponent<Load>(entity);
                SetComponent(
                    entity,
                    new EntityDynamicNavMeshData()
                    {
                        agentTypeId = sourcesAggregatorAuthoring.agentTypeId
                    });
            }
        }
    }
}
#endif