﻿using Unity.Entities;
using Unity.Entities.Content;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

namespace NavMeshDots.Runtime
{
    // public struct NavMeshPrimitiveSource
    // {
    //     public float4x4 trs;
    //     public Vector3 size;
    //     public NavMeshBuildSourceShape shape;
    //     public int area;
    // }

    public struct NavMeshSourcesBlob
    {
        public BlobArray<NavMeshBuildSource> sources;
    }

    public partial struct NavMeshSourcesBlobStorage : IComponentData
    {
        public BlobAssetReference<NavMeshSourcesBlob> blob;
    }

    [InternalBufferCapacity(0)]
    public partial struct NavMeshSourceElement : IBufferElementData
    {
        public NavMeshBuildSource primitive;
    }

    public partial struct StaticTag : IComponentData
    {

    }

    public partial struct DynamicTag : IComponentData
    {

    }

    public partial struct EntityNavMeshBounds : IComponentData
    {
        public Bounds bounds;
    }

    public partial struct EntityLazyNavMeshData : IComponentData
    {
        public WeakObjectReference<NavMeshData> data;
    }

    public partial class EntityNavMeshData : IComponentData
    {
        public NavMeshData data;
    }

    public partial struct EntityDynamicNavMeshData : IComponentData
    {
        public int agentTypeId;
    }

    public partial struct Load : IComponentData
    {
    }

    public partial struct Build : IComponentData
    {
    }

    public partial struct Built : IComponentData
    {
    }

    public partial struct EntityNavMeshInstance : IComponentData, ICleanupComponentData
    {
        public NavMeshDataInstance instance;
    }
}