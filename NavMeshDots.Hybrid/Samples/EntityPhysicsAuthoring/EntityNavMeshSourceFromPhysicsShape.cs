#if UNITY_EDITOR && HAS_ENTITIES_PHYSICS
using Unity.Mathematics;
using Unity.Physics.Authoring;
using UnityEngine;
using UnityEngine.AI;

namespace NavMeshDots.Hybrid
{
    public class EntityNavMeshSourceFromPhysicsShape : MonoBehaviour, IEntityNavMeshSourceProvider
    {
        [SerializeField] private int area = 0;
        public bool TryGetSource(out _NavMeshBuildSource result)
        {
            result = default;
            if (!gameObject.TryGetComponent(out PhysicsShapeAuthoring entityCollider)) return false;
            switch (entityCollider.ShapeType)
            {
                case ShapeType.Capsule:
                {
                    var props = entityCollider.GetCapsuleProperties();
                    result.m_Area = area;
                    result.m_Shape = NavMeshBuildSourceShape.Capsule;
                    result.m_Size = new Vector3(props.Radius, props.Height, props.Radius); //todo where is edges count? z?
                    result.m_Transform = Matrix4x4.TRS(
                        entityCollider.transform.TransformPoint(props.Center),
                        math.mul(math.mul(props.Orientation, quaternion.RotateX(90)), entityCollider.transform.rotation),
                        Vector3.one);
                    break;
                }
                case ShapeType.Cylinder:
                {
                    var props = entityCollider.GetCylinderProperties();
                    result.m_Area = area;
                    result.m_Shape = NavMeshBuildSourceShape.Box;
                    result.m_Size = new Vector3(props.Radius, props.Height, props.Radius);
                    result.m_Transform = Matrix4x4.TRS(
                        entityCollider.transform.TransformPoint(props.Center),
                        math.mul(math.mul(props.Orientation, quaternion.RotateX(90)), entityCollider.transform.rotation),
                        Vector3.one);

                    break;
                }
                case ShapeType.Box:
                    result.m_Area = area;
                    result.m_Shape = NavMeshBuildSourceShape.Box;
                    result.m_Size = entityCollider.GetBoxProperties().Size * entityCollider.transform.localScale;
                    result.m_Transform = Matrix4x4.TRS(
                        (float3)entityCollider.transform.TransformPoint(entityCollider.GetBoxProperties().Center),
                        math.mul(entityCollider.GetBoxProperties().Orientation, entityCollider.transform.rotation),
                        Vector3.one);
                    break;
                case ShapeType.Plane:
                    entityCollider.GetPlaneProperties(out var center, out var size, out var rotation);
                    result.m_Area = area;
                    result.m_Shape = NavMeshBuildSourceShape.Box;
                    result.m_Size = new float3(size.x * entityCollider.transform.localScale.x, 0.1f, size.y * entityCollider.transform.localScale.y);
                    result.m_Transform = Matrix4x4.TRS(
                        (float3)entityCollider.transform.TransformPoint(center + new float3(0f, -0.1f, 0f)),
                        math.mul(rotation, entityCollider.transform.rotation),
                        Vector3.one);
                    break;
                default: return false;
            }
            return true;
        }
    }
}
#endif