#if UNITY_EDITOR
using NavMeshDots.Runtime;
using Unity.Entities;
using Unity.Entities.Content;
using UnityEngine;
using UnityEngine.AI;

namespace NavMeshDots.Hybrid
{
    public class EntityLazyNavMeshDataAuthoring : MonoBehaviour
    {
        [SerializeField] private EntityNavMeshDataSo data_s;
        [SerializeField] private bool autoLoad = true;

        class EntityNavMeshDataBaker : Baker<EntityLazyNavMeshDataAuthoring>
        {
            public override void Bake(EntityLazyNavMeshDataAuthoring authoring)
            {
                if (!authoring.data_s) return;
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent<StaticTag>(entity);
                AddComponent<Built>(entity);
                AddComponent(
                    entity,
                    new EntityLazyNavMeshData()
                    {
                        data = new WeakObjectReference<NavMeshData>(authoring.data_s.data),
                    });
                if (authoring.autoLoad)
                {
                    AddComponent(entity, new Load());
                }
            }
        }
    }
}
#endif