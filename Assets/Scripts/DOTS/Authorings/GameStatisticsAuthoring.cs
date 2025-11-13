using System;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace DOTS.Authorings
{
    public class GameStatisticsAuthoring : MonoBehaviour
    {
        private class Baker : Baker<GameStatisticsAuthoring>
        {
            public override void Bake(GameStatisticsAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                var gemStats = CreateGemStatsBlobAsset();
                AddComponent(entity, new GameStatistics
                {
                    gemStats = gemStats,
                });
                AddComponent(entity, new OnChanged());
                SetComponentEnabled<OnChanged>(false);
            }
            
            private BlobAssetReference<GemStats> CreateGemStatsBlobAsset()
            {
                using var builder = new BlobBuilder(Allocator.Temp);
                ref var gemStatsBlob = ref builder.ConstructRoot<GemStats>();

                var count = Enum.GetNames(typeof(Faction)).Length;
                var arrayBuilder = builder.Allocate(ref gemStatsBlob.Array, count);

                for (var i = 0; i < count; i++)
                {
                    arrayBuilder[i].count = 0; //можно из сохранения значения подставлять будет
                }

                return builder.CreateBlobAssetReference<GemStats>(Allocator.Persistent);
            }
        }
    }

    public struct GameStatistics : IComponentData
    {
        public BlobAssetReference<GemStats> gemStats;
    }

    public struct GemStats
    {
        public BlobArray<GemBlob> Array;
    }

    public struct GemBlob
    {
        public int count;
    }

    public struct OnChanged : IComponentData , IEnableableComponent { }
}