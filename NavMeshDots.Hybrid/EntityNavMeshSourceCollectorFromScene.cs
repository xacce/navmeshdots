# if UNITY_EDITOR
using System.Linq;
using UnityEngine;

namespace NavMeshDots.Hybrid
{
    public interface IEntityNavMeshSourceCollector
    {
        public GameObject[] Collect(GameObject go);
    }


    public class EntityNavMeshSourceCollectorFromScene : MonoBehaviour, IEntityNavMeshSourceCollector
    {

        public GameObject[] Collect(GameObject go)
        {
            return FindObjectsOfType<GameObject>();
        }

    }

}
#endif