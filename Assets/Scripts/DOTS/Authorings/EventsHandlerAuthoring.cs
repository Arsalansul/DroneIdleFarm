using Unity.Entities;
using UnityEngine;

namespace DOTS.Authorings
{
    public class EventsHandlerAuthoring : MonoBehaviour
    {
        private class Baker : Baker<EventsHandlerAuthoring>
        {
            public override void Bake(EventsHandlerAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new EventsHandler());
                AddComponent(entity, new PlayClipOnPickup());
                SetComponentEnabled<PlayClipOnPickup>(entity, false);
            }
        }
    }
    
    public struct EventsHandler : IComponentData{}
    
    public struct PlayClipOnPickup : IComponentData, IEnableableComponent{}
}