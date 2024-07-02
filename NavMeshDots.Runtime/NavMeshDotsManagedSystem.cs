using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using Unity.Entities.Content;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.AI;

namespace NavMeshDots.Runtime
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class NavMeshDotsManagedSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var ecbSingleton = SystemAPI.GetSingleton<EndInitializationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(EntityManager.WorldUnmanaged);

            #region Create instances from not loaded (weak referenced) nav meshes

            foreach (var (dataAccess, entity) in SystemAPI.Query<RefRW<EntityLazyNavMeshData>>().WithAll<Load, Built>().WithNone<EntityNavMeshInstance>().WithEntityAccess())
            {
                var dataRo = dataAccess.ValueRO;
                switch (dataRo.data.LoadingStatus)
                {
                    case ObjectLoadingStatus.None:
                        dataAccess.ValueRW.data.LoadAsync();
                        continue;
                    case ObjectLoadingStatus.Completed:
                    {
                        var instance = NavMesh.AddNavMeshData(dataRo.data.Result);
                        ecb.AddComponent(
                            entity,
                            new EntityNavMeshInstance()
                            {
                                instance = instance,
                            });
                        ecb.RemoveComponent<Load>(entity);
                        break;
                    }
                }

            }

            #endregion

            #region Create instances for nav meshes

            foreach (var (dataAccess, entity) in SystemAPI.Query<EntityNavMeshData>().WithAll<Load, Built>().WithNone<EntityNavMeshInstance>().WithEntityAccess())
            {
                var instance = NavMesh.AddNavMeshData(dataAccess.data); //todo position rotation
                Debug.Log(dataAccess.data.position);
                Debug.Log(dataAccess.data.rotation);
                Debug.Log(dataAccess.data.sourceBounds);
                Debug.Log($"New nav mesh instance was created: {instance.GetHashCode()}, valid: {instance.valid}");
                ecb.AddComponent(
                    entity,
                    new EntityNavMeshInstance()
                    {
                        instance = instance,
                    });
                ecb.RemoveComponent<Load>(entity);
            }

            #endregion

            #region Invalidate instances for deleted navmeshes

            foreach (var (dataAccess, entity) in SystemAPI.Query<RefRO<EntityNavMeshInstance>>().WithAll<StaticTag>().WithNone<EntityNavMeshData>().WithEntityAccess())
            {
                NavMesh.RemoveNavMeshData(dataAccess.ValueRO.instance);
                ecb.DestroyEntity(entity);
            }

            #endregion


            # region Create instances for dynamic navmeshes

            foreach (var (sources, bounds, navMeshData, ltw, dynamicData, entity) in SystemAPI
                         .Query<DynamicBuffer<NavMeshSourceElement>, RefRO<EntityNavMeshBounds>, EntityNavMeshData, RefRO<LocalToWorld>, RefRO<EntityDynamicNavMeshData>>()
                         .WithAll<Build, DynamicTag>().WithNone<Built>()
                         .WithEntityAccess())
            {
                var sourcesList = new List<NavMeshBuildSource>(sources.Length);
                sourcesList.AddRange(sources.Reinterpret<NavMeshBuildSource>().AsNativeArray());
                var settings = NavMesh.GetSettingsByIndex(dynamicData.ValueRO.agentTypeId);
                Debug.Log($"Bounds: {bounds.ValueRO.bounds.center}, {bounds.ValueRO.bounds.extents}");
                var data = NavMeshBuilder.BuildNavMeshData(settings, sourcesList, bounds.ValueRO.bounds, Vector3.zero, Quaternion.identity);
                navMeshData.data = data;
                ecb.RemoveComponent<Build>(entity);
                ecb.AddComponent<Built>(entity);
            }

            #endregion

            foreach (var (sources, bounds, navMeshData, dynamicData, entity) in SystemAPI
                         .Query<DynamicBuffer<NavMeshSourceElement>, RefRO<EntityNavMeshBounds>, EntityNavMeshData, RefRO<EntityDynamicNavMeshData>>()
                         .WithAll<Build, Built, DynamicTag>()
                         .WithEntityAccess())
            {
                var sourcesList = new List<NavMeshBuildSource>(sources.Length);
                sourcesList.AddRange(sources.Reinterpret<NavMeshBuildSource>().AsNativeArray());
                var settings = NavMesh.GetSettingsByIndex(dynamicData.ValueRO.agentTypeId);
                Debug.Log($"Nav mesh was rebuild, sources size: {sourcesList.Count}");
                NavMeshBuilder.UpdateNavMeshDataAsync(navMeshData.data, settings, sourcesList, bounds.ValueRO.bounds);
                ecb.RemoveComponent<Build>(entity);
            }

        }
    }
}