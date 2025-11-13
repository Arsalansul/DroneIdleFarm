using Unity.Entities;
using UnityEngine;

namespace DOTS.Authorings
{
    public class UnitSpawnerAuthoring : MonoBehaviour
    {
        [Range(3,10)] public float ringAdditionalSize;
        public Faction faction;
        private class Baker : Baker<UnitSpawnerAuthoring>
        {
            public override void Bake(UnitSpawnerAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new UnitSpawner
                {
                    ringAdditionalSize = authoring.ringAdditionalSize,
                    faction = authoring.faction,
                });
            }
        }
    }

    public struct UnitSpawner : IComponentData
    {
        public int count;
        public int maxCount;
        public float ringAdditionalSize;
        public Faction faction;
    }
}