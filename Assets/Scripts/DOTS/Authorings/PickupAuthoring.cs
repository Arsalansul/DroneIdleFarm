using Unity.Entities;
using UnityEngine;

namespace DOTS.Authorings
{
    public class PickupAuthoring : MonoBehaviour
    {
        private class Baker : Baker<PickupAuthoring>
        {
            public override void Bake(PickupAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new Pickup
                {
                });
            }
        }
    }

    public struct Pickup : IComponentData
    {
        public float timer;
        public Entity triggerEntity;
        public bool activated;
    }
}