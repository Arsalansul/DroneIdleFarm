using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace DOTS.Authorings
{
    public class SpawnerAuthoring : MonoBehaviour
    {
        public GameObject entityPrefab;
        
        private class Baker : Baker<SpawnerAuthoring>
        {
            public override void Bake(SpawnerAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new Spawner
                {
                    entity = GetEntity(authoring.entityPrefab, TransformUsageFlags.Dynamic),
                });
                AddBuffer<EntityBuffer>(entity);
            }
        }
    }

    public struct Spawner : IComponentData
    {
        public Entity entity;
    }

    public struct EntityBuffer : IBufferElementData
    {
        public Entity entity;
    }
}