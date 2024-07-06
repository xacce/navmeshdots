#if UNITY_EDITOR
namespace NavMeshDots.Hybrid
{
    public interface IEntityNavMeshSourceProvider
    {
        public bool TryGetSource(out _NavMeshBuildSource source);
    }
}
#endif