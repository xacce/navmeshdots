#if UNITY_EDITOR
using NavMeshDots.Runtime;
using Unity.Entities;
using Unity.Entities.Content;
using UnityEngine;
using UnityEngine.AI;

namespace NavMeshDots.Hybrid
{
    public class EntityStaticNavMeshDataAuthoring : AbstractEntityNavMeshDataAuthoring
    {
        [SerializeField] private bool autoLoad = true;

        class EntityStaticNavMeshDataAuthoringB : Baker<EntityStaticNavMeshDataAuthoring>
        {
            public override void Bake(EntityStaticNavMeshDataAuthoring authoring)
            {
                if (!authoring.navMesh_s) return;
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent<StaticTag>(entity);
                AddComponent<BuiltNavMesh>(entity);
                AddComponentObject(
                    entity,
                    new EntityNavMeshData()
                    {
                        data = authoring.navMesh_s,
                    });
                if (authoring.autoLoad)
                {
                    AddComponent(entity, new SpawnNavMesh());
                }
            }
        }
    }
}
#endif