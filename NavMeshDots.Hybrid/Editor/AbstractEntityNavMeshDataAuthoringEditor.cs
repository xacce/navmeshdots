#if UNITY_EDITOR
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using NavMeshBuilder = UnityEngine.AI.NavMeshBuilder;

namespace NavMeshDots.Hybrid.Editor
{
    [CustomEditor(typeof(AbstractEntityNavMeshDataAuthoring), true)]
    public class AbstractEntityNavMeshDataAuthoringEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            GUILayout.Space(10);
            var trg = (AbstractEntityNavMeshDataAuthoring)target;
            if (!(((target as AbstractEntityNavMeshDataAuthoring)!).gameObject).TryGetComponent(out EntityNavMeshSourcesAggregatorAuthoring aggregator)) return;
            var bounds = new Bounds(trg.center, trg.bounds);
            if (GUILayout.Button("Preview"))
            {
                aggregator.Collect();
                var sources = aggregator.serialized.ToList().ConvertAll(input => input.AsNative());
                var navMeshData = NavMeshBuilder.BuildNavMeshData(
                    NavMesh.GetSettingsByIndex(0),
                    sources,
                    bounds,
                    Vector3.zero,
                    Quaternion.identity);
                NavMesh.RemoveAllNavMeshData();
                NavMesh.AddNavMeshData(navMeshData); ///Todo check for correct disposing and cleanups. 
            }
            if (GUILayout.Button("Create static nav mesh data"))
            {
                aggregator.Collect();
                var sources = aggregator.serialized.ToList().ConvertAll(input => input.AsNative());

                var navMeshData = NavMeshBuilder.BuildNavMeshData(
                    NavMesh.GetSettingsByIndex(0),
                    sources,
                    bounds,
                    Vector3.zero,
                    Quaternion.identity);
                var navMeshPath = Path.Combine("Assets", $"NavMesh_0_{(int)(bounds.center.x)}_{(int)bounds.center.y}_{(int)bounds.center.z}_{trg.GetHashCode()}.asset");

                if (trg.navMesh != null)
                {
                    AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(trg.navMesh));
                }
                navMeshPath = AssetDatabase.GenerateUniqueAssetPath(navMeshPath);
                AssetDatabase.CreateAsset(navMeshData, navMeshPath);
                serializedObject.FindProperty("navMesh_s").objectReferenceValue = AssetDatabase.LoadAssetAtPath<NavMeshData>(navMeshPath);
                serializedObject.ApplyModifiedPropertiesWithoutUndo();
                EditorUtility.SetDirty(trg.gameObject);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }
    }
}
#endif