using System;
using UnityEngine;
using UnityEngine.AI;

namespace NavMeshDots.Hybrid
{
    [CreateAssetMenu(menuName = "NavMeshDots/Create nav mesh data so")]
    public class EntityNavMeshDataSo : ScriptableObject
    {
        [SerializeField] private NavMeshData data_s;

        private void OnValidate()
        {
            Debug.LogWarning($"Empty nav mesh data in  so file {name}. This happes if u regenerate nav mesh data.", this);
        }
        
        public NavMeshData data => data_s;
    }
}