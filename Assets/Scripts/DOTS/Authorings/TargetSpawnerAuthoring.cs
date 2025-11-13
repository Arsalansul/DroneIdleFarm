using Unity.Entities;
using UnityEngine;

namespace DOTS.Authorings
{
    public class TargetSpawnerAuthoring : MonoBehaviour
    {
        public float radius;
        public float minDistance;
        [Range(0.1f, 3f)]
        public float cooldown;
        
        private class Baker : Baker<TargetSpawnerAuthoring>
        {
            public override void Bake(TargetSpawnerAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new TargetSpawner
                {
                    radius = authoring.radius,
                    minDistance = authoring.minDistance,
                    cooldown = authoring.cooldown,
                    timer = 0
                });
            }
        }
    }

    public struct TargetSpawner : IComponentData
    {
        public float radius;
        public float minDistance;
        public float cooldown;
        public float timer;
    }
}