using Unity.Entities;
using UnityEngine;

namespace DOTS.Authorings
{
    public class FindTargetAuthoring : MonoBehaviour
    {
        public float radius;
        private class Baker : Baker<FindTargetAuthoring>
        {
            public override void Bake(FindTargetAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new FindTarget
                {
                    radius = authoring.radius,
                });
            }
        }
    }

    public struct FindTarget : IComponentData
    {
        public float radius;
        public Entity targetEntity;
    }
}