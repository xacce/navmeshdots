﻿#if UNITY_EDITOR
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using NavMeshBuilder = UnityEngine.AI.NavMeshBuilder;

namespace NavMeshDots.Hybrid.Editor
{
    [CustomEditor(typeof(EntityNavMeshSourcesStaticAuthoring))]
    public class EntityNavMeshSourcesAggregatorAuthoringEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            GUILayout.Space(10);
            var trg = (EntityNavMeshSourcesStaticAuthoring)target;
            if (GUILayout.Button("Collect"))
            {
                trg.Collect();
            }
        }
    }
}
#endif