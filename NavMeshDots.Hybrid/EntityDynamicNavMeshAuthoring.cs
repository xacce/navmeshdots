#if UNITY_EDITOR
using NavMeshDots.Runtime;
using Unity.Entities;
using UnityEngine;
using UnityEngine.AI;

namespace NavMeshDots.Hybrid
{
    public class EntityDynamicNavMeshAuthoring : AbstractEntityNavMeshDataAuthoring
    {
        // [SerializeField] private PhysicsShapeAuthoring[] shapes_s = Array.Empty<PhysicsShapeAuthoring>();
        [SerializeField] private int agentTypeId = 0;
        [SerializeField] private bool buildAtstart = true;
        [SerializeField] private bool spawnAfterBuild = true;

        class B : Baker<EntityDynamicNavMeshAuthoring>
        {
            public override void Bake(EntityDynamicNavMeshAuthoring sourcesAggregatorAuthoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent<DynamicTag>(entity);
                AddComponentObject(
                    entity,
                    new EntityNavMeshData()
                    {
                        // data = sourcesAggregatorAuthoring.navMesh_s,
                        agentTypeId = sourcesAggregatorAuthoring.agentTypeId
                    });
                if (sourcesAggregatorAuthoring.navMesh_s != null) AddComponent<BuiltNavMesh>(entity);
                if (sourcesAggregatorAuthoring.buildAtstart && sourcesAggregatorAuthoring.navMesh_s == null) AddComponent<BuildNavMesh>(entity);
                if (sourcesAggregatorAuthoring.spawnAfterBuild) AddComponent<SpawnNavMesh>(entity);

            }
        }
    }
}
#endif