#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using NavMeshDots.Runtime;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEditor;
using UnityEngine.AI;

namespace NavMeshDots.Hybrid
{
    [Serializable]
    public struct _NavMeshBuildSource
    {
        public Matrix4x4 m_Transform;
        public Vector3 m_Size;
        public NavMeshBuildSourceShape m_Shape;
        public int m_Area;
        public int m_InstanceID;
        public int m_ComponentID;
        public int m_GenerateLinks;

        public NavMeshBuildSource AsNative()
        {
            return new NavMeshBuildSource
            {
                area = m_Area,
                size = m_Size,
                transform = m_Transform,
                shape = m_Shape,
            };
        }
    }

    public class EntityNavMeshSourcesStaticAuthoring : MonoBehaviour
    {
        [SerializeField] private bool bake_s = true;
        [SerializeField] private _NavMeshBuildSource[] serialized_s = Array.Empty<_NavMeshBuildSource>();


        public _NavMeshBuildSource[] serialized => serialized_s;

        public void Collect()
        {
            List<GameObject> gameObjects = new List<GameObject>();
            List<_NavMeshBuildSource> primitives = new List<_NavMeshBuildSource>();
            foreach (var collector in GetComponents<IEntityNavMeshSourceCollector>())
            {
                gameObjects.AddRange(collector.Collect(gameObject));
            }
            Debug.Log($"Count: {gameObjects.Count}");
            var so = new SerializedObject(this);
            var prop = so.FindProperty(nameof(serialized_s));
            prop.ClearArray();

            foreach (var go in gameObjects)
            {
                if (go.TryGetComponent(out EntityNavMeshIgnoreSource _)) continue;
                if (go.TryGetComponent(out IEntityNavMeshSourceProvider provider) && provider.TryGetSource(out var result))
                {
                    primitives.Add(result);
                }
            }
            prop.arraySize = primitives.Count;
            for (int i = 0; i < primitives.Count; i++)
            {
                prop.GetArrayElementAtIndex(i).boxedValue = primitives[i];
            }
            so.ApplyModifiedProperties();
        }

        class B : Baker<EntityNavMeshSourcesStaticAuthoring>
        {
            public override void Bake(EntityNavMeshSourcesStaticAuthoring sourcesStaticAuthoring)
            {
                if (!sourcesStaticAuthoring.bake_s) return;
                var entity = GetEntity(TransformUsageFlags.None);
                AddBuffer<NavMeshSourceElement>(entity);

                foreach (var p in sourcesStaticAuthoring.serialized_s)
                {
                    AppendToBuffer(
                        entity,
                        new NavMeshSourceElement()
                        {
                            primitive = p.AsNative()
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