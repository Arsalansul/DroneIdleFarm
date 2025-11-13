using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace DOTS.Authorings
{
    public class CourierAuthoring : MonoBehaviour
    {
        private class Baker : Baker<CourierAuthoring>
        {
            public override void Bake(CourierAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new Courier());
                SetComponentEnabled<Courier>(entity, false);
                AddComponent(entity, new DeliveryEvent());
                SetComponentEnabled<DeliveryEvent>(entity, false);
            }
        }
    }

    public struct Courier : IComponentData, IEnableableComponent
    {
        public LocalTransform ClientLocalTransform;
        public bool onArrived;
    }

    public struct DeliveryEvent : IComponentData, IEnableableComponent
    {
    }
}