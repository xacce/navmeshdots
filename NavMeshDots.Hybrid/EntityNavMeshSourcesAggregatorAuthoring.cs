#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using NavMeshDots.Runtime;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics.Authoring;
using UnityEngine;
using UnityEditor;
using UnityEngine.AI;

namespace NavMeshDots.Hybrid
{
    public class EntityNavMeshSourcesAggregatorAuthoring : MonoBehaviour
    {
        // [SerializeField] private bool generateBakedSources_s;
        [Serializable]
        struct _NavMeshBuildSource
        {
            public Matrix4x4 m_Transform;
            public Vector3 m_Size;
            public NavMeshBuildSourceShape m_Shape;
            public int m_Area;
            public int m_InstanceID;
            public int m_ComponentID;
            public int m_GenerateLinks;
        }

        [EditorButton(nameof(Collect), "Collect")] [SerializeField]
        private _NavMeshBuildSource[] serialized_s = Array.Empty<_NavMeshBuildSource>();

        void Collect()
        {
            List<GameObject> gameObjects = new List<GameObject>();
            List<NavMeshBuildSource> primitives = new List<NavMeshBuildSource>();
            foreach (var collector in GetComponents<IEntityNavMeshSourceCollector>())
            {
                gameObjects.AddRange(collector.Collect(gameObject));
            }
            var so = new SerializedObject(this);
            var prop = so.FindProperty(nameof(serialized_s));
            prop.ClearArray();

            foreach (var go in gameObjects)
            {
                if (go.TryGetComponent(out EntityNavMeshIgnoreSource _)) continue;
                NavMeshBuildSource source = default;
                if (go.TryGetComponent(out ICustomNavMeshSourceBuilder custom))
                {
                    primitives.Add(custom.Build(gameObject));
                }
                else if (go.TryGetComponent(out PhysicsShapeAuthoring collider))
                {
                    var primitive = new NavMeshBuildSource();
                    switch (collider.ShapeType)
                    {
                        case ShapeType.Capsule:
                        {
                            var props = collider.GetCapsuleProperties();
                            primitive.area = 0;
                            primitive.shape = NavMeshBuildSourceShape.Capsule;
                            primitive.size = new Vector3(props.Radius, props.Height, props.Radius); //todo where is edges count? z?
                            primitive.transform = Matrix4x4.TRS(
                                collider.transform.TransformPoint(props.Center),
                                math.mul(math.mul(props.Orientation, quaternion.RotateX(90)), collider.transform.rotation),
                                Vector3.one);
                            break;
                        }
                        case ShapeType.Cylinder:
                        {
                            var props = collider.GetCylinderProperties();
                            primitive.area = 0;
                            primitive.shape = NavMeshBuildSourceShape.Box;
                            primitive.size = new Vector3(props.Radius, props.Height, props.Radius);
                            primitive.transform = Matrix4x4.TRS(
                                collider.transform.TransformPoint(props.Center),
                                math.mul(math.mul(props.Orientation, quaternion.RotateX(90)), collider.transform.rotation),
                                Vector3.one);

                            break;
                        }
                        case ShapeType.Box:
                            primitive.area = 0;
                            primitive.shape = NavMeshBuildSourceShape.Box;
                            primitive.size = collider.GetBoxProperties().Size;
                            primitive.transform = Matrix4x4.TRS(
                                (float3)collider.transform.TransformPoint(collider.GetBoxProperties().Center),
                                math.mul(collider.GetBoxProperties().Orientation, collider.transform.rotation),
                                Vector3.one);
                            break;
                        case ShapeType.Plane:
                            collider.GetPlaneProperties(out var center, out var size, out var rotation);
                            primitive.area = 0;
                            primitive.shape = NavMeshBuildSourceShape.Box;
                            primitive.size = new float3(size.x, 0.1f, size.y);
                            primitive.transform = Matrix4x4.TRS((float3)collider.transform.TransformPoint(center+new float3(0f,-0.1f,0f)), math.mul(rotation, collider.transform.rotation), Vector3.one);
                            break;
                        default: continue;
                    }
                    primitives.Add(primitive);

                }
            }
            prop.arraySize = primitives.Count;
            for (int i = 0; i < primitives.Count; i++)
            {
                var p = primitives[i];
                prop.GetArrayElementAtIndex(i).boxedValue = new _NavMeshBuildSource()
                {
                    m_Area = p.area,
                    m_Shape = p.shape,
                    m_Size = p.size,
                    m_Transform = p.transform,
                    m_GenerateLinks = 0,
                };
            }
            so.ApplyModifiedProperties();
        }

        class B : Baker<EntityNavMeshSourcesAggregatorAuthoring>
        {
            public override void Bake(EntityNavMeshSourcesAggregatorAuthoring sourcesAggregatorAuthoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddBuffer<NavMeshSourceElement>(entity);

                foreach (var p in sourcesAggregatorAuthoring.serialized_s)
                {
                    AppendToBuffer(
                        entity,
                        new NavMeshSourceElement()
                        {
                            primitive = new NavMeshBuildSource()
                            {
                                area = p.m_Area,
                                size = p.m_Size,
                                transform = p.m_Transform,
                                shape = p.m_Shape,
                            }
                        }
                    );
                }

                // if (sourcesAggregatorAuthoring.generateBakedSources_s)
                // {
                //     var assetPath = $"Assets/NavMeshBakedSources_{this.GetSceneGUID()}.asset";
                //     var exists = AssetDatabase.LoadAssetAtPath<EntityNavMeshSourcesBaked>(assetPath);
                //     if (!exists)
                //     {
                //         exists = ScriptableObject.CreateInstance<EntityNavMeshSourcesBaked>();
                //         AssetDatabase.CreateAsset(exists, assetPath);
                //     }
                //     exists.elements = primitives.ToArray();
                //     EditorUtility.SetDirty(exists);
                //     AssetDatabase.SaveAssets();
                // }

            }
        }

    }

}
#endif