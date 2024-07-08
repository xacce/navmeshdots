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
    public partial class PreInitializeNavMeshSystem : SystemBase
    {
        protected override void OnCreate()
        {
            // NavMesh.RemoveAllNavMeshData();
        }

        protected override void OnUpdate()
        {
        }
    }

    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [UpdateAfter(typeof(PreInitializeNavMeshSystem))]
    public partial class NavMeshDotsManagedSystem : SystemBase
    {
        protected override void OnDestroy()
        {
            // foreach (var (dataAccess, entity) in SystemAPI.Query<RefRW<EntityLazyNavMeshData>>().WithEntityAccess())
            // {
            // dataAccess.ValueRW.data.Release();
            // }
            foreach (var (instance, entity) in SystemAPI.Query<RefRW<EntityNavMeshInstance>>().WithEntityAccess())
            {
                NavMesh.RemoveNavMeshData(instance.ValueRO.instance);
            }
            base.OnDestroy();
        }

        protected override void OnUpdate()
        {
            var ecbSingleton = SystemAPI.GetSingleton<EndInitializationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(EntityManager.WorldUnmanaged);

            #region Create instances from not loaded (weak referenced) nav meshes

            foreach (var (dataAccess, entity) in SystemAPI.Query<RefRW<EntityLazyNavMeshData>>().WithAll<LoadNavMesh, BuiltNavMesh>().WithNone<EntityNavMeshInstance>().WithEntityAccess())
            {
                var dataRo = dataAccess.ValueRO;
                switch (dataRo.data.LoadingStatus)
                {
                    case ObjectLoadingStatus.None:
                        dataAccess.ValueRW.data.LoadAsync();
                        continue;
                    case ObjectLoadingStatus.Completed when !dataRo.data.IsReferenceValid || dataRo.data.Result == null:
                        Debug.LogWarning($"Cant load lazy navmesh for {entity} entity (Invalid reference or null result)");
                        ecb.RemoveComponent<SpawnNavMesh>(entity);
                        break;
                    case ObjectLoadingStatus.Completed:
                    {
                        ecb.AddComponent(
                            entity,
                            new EntityNavMeshData()
                            {
                                data = dataRo.data.Result,
                            });
                        ecb.AddComponent<SpawnNavMesh>(entity);
                        ecb.RemoveComponent<LoadNavMesh>(entity);
                        break;
                    }
                    default:
                        Debug.Log($"Status: {dataRo.data.LoadingStatus}");
                        break;
                }

            }

            #endregion


            #region Create instances for nav meshes

            foreach (var (dataAccess, entity) in SystemAPI.Query<EntityNavMeshData>().WithAll<BuiltNavMesh, SpawnNavMesh>().WithNone<EntityNavMeshInstance>().WithEntityAccess())
            {
                var instance = NavMesh.AddNavMeshData(dataAccess.data); //todo position rotation
                Debug.Log($"New nav mesh instance was created: {instance.GetHashCode()}, valid: {instance.valid}");
                ecb.AddComponent(
                    entity,
                    new EntityNavMeshInstance()
                    {
                        instance = instance,
                    });
                ecb.RemoveComponent<SpawnNavMesh>(entity);
            }

            #endregion


            #region Invalidate instances for deleted navmeshes

            foreach (var (dataAccess, entity) in SystemAPI.Query<RefRO<EntityNavMeshInstance>>().WithAll<StaticTag>().WithNone<EntityNavMeshData, EntityLazyNavMeshData>()
                         .WithEntityAccess())
            {
                NavMesh.RemoveNavMeshData(dataAccess.ValueRO.instance);
                ecb.DestroyEntity(entity);
            }

            #endregion


            # region Create instances for  dynamic navmeshes

            foreach (var (sources, bounds, navMeshData, ltw, entity) in SystemAPI
                         .Query<DynamicBuffer<NavMeshSourceElement>, RefRO<EntityNavMeshBounds>, EntityNavMeshData, RefRO<LocalToWorld>>()
                         .WithAll<BuildNavMesh, DynamicTag>().WithNone<BuiltNavMesh>()
                         .WithEntityAccess())
            {
                var sourcesList = new List<NavMeshBuildSource>(sources.Length);
                sourcesList.AddRange(sources.Reinterpret<NavMeshBuildSource>().AsNativeArray());
                var settings = NavMesh.GetSettingsByIndex(navMeshData.agentTypeId);
                Debug.Log($"Bounds: {bounds.ValueRO.bounds.center}, {bounds.ValueRO.bounds.extents}");
                var data = NavMeshBuilder.BuildNavMeshData(settings, sourcesList, bounds.ValueRO.bounds, Vector3.zero, Quaternion.identity);
                navMeshData.data = data;
                ecb.RemoveComponent<BuildNavMesh>(entity);
                ecb.AddComponent<BuiltNavMesh>(entity);
                ecb.AddComponent<SpawnNavMesh>(entity);
            }

            #endregion


            #region Rebuild dynamic navmeshes

            foreach (var (sources, bounds, navMeshData, entity) in SystemAPI
                         .Query<DynamicBuffer<NavMeshSourceElement>, RefRO<EntityNavMeshBounds>, EntityNavMeshData>()
                         .WithAll<BuildNavMesh, BuiltNavMesh, DynamicTag>()
                         .WithEntityAccess())
            {
                var sourcesList = new List<NavMeshBuildSource>(sources.Length);
                sourcesList.AddRange(sources.Reinterpret<NavMeshBuildSource>().AsNativeArray());
                var settings = NavMesh.GetSettingsByIndex(navMeshData.agentTypeId);
                Debug.Log($"Nav mesh was rebuild, sources size: {sourcesList.Count}");
                NavMeshBuilder.UpdateNavMeshDataAsync(navMeshData.data, settings, sourcesList, bounds.ValueRO.bounds);
                ecb.RemoveComponent<BuildNavMesh>(entity);
            }

            #endregion

        }
    }
}