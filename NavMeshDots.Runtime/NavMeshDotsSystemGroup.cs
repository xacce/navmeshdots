using Unity.Entities;
using Unity.Transforms;

namespace NavMeshDots.Runtime
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(TransformSystemGroup))]
    public partial class NavMeshDotsSystemGroup : ComponentSystemGroup
    {
    }
}