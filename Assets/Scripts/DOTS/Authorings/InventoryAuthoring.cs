using Unity.Entities;
using UnityEngine;

namespace DOTS.Authorings
{
    public class InventoryAuthoring : MonoBehaviour
    {
        private class Baker : Baker<InventoryAuthoring>
        {
            public override void Bake(InventoryAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new Inventory());
            }
        }
    }

    public struct Inventory : IComponentData
    {
        public int gemCount;
        public bool onChanged;
    }
}