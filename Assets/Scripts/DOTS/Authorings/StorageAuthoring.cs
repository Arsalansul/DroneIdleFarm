using Unity.Entities;
using UnityEngine;

namespace DOTS.Authorings
{
    public class StorageAuthoring : MonoBehaviour
    {
        private class Baker : Baker<StorageAuthoring>
        {
            public override void Bake(StorageAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new Storage());
            }
        }
    }

    public struct Storage : IComponentData
    {
        public int gemCount;
    }
}