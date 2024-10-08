﻿#if UNITY_EDITOR
using NavMeshDots.Runtime;
using Unity.Entities;
using Unity.Entities.Content;
using UnityEngine;
using UnityEngine.AI;

namespace NavMeshDots.Hybrid
{
    public class EntityLazyNavMeshDataAuthoring : AbstractEntityNavMeshDataAuthoring
    {
        [SerializeField] private bool autoLoad = true;

        class EntityNavMeshDataBaker : Baker<EntityLazyNavMeshDataAuthoring>
        {
            public override void Bake(EntityLazyNavMeshDataAuthoring authoring)
            {
                if (!authoring.navMesh_s) return;
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent<StaticTag>(entity);
                AddComponent<BuiltNavMesh>(entity);
                AddComponent(
                    entity,
                    new EntityLazyNavMeshData()
                    {
                        data = new WeakObjectReference<NavMeshData>(authoring.navMesh_s),
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