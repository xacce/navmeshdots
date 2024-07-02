# if UNITY_EDITOR
using System.Linq;
using Unity.Entities;
using Unity.Physics.Authoring;
using UnityEngine;
using UnityEngine.AI;

namespace NavMeshDots.Hybrid
{
    public interface IEntityNavMeshSourceCollector
    {
        public GameObject[] Collect(GameObject go);
    }

    public interface ICustomNavMeshSourceBuilder
    {
        public NavMeshBuildSource Build(GameObject go);
    }

    public class EntityNavMeshSourceCollectorColliders : MonoBehaviour, IEntityNavMeshSourceCollector
    {

        public GameObject[] Collect(GameObject go)
        {
            return FindObjectsOfType<PhysicsShapeAuthoring>().ToList().ConvertAll(input => input.gameObject).ToArray(); //kek
        }

    }

}
#endif