using DOTS.Authorings;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;

namespace DOTS.Systems
{
    [UpdateAfter(typeof(DeliverySystem))]
    public partial struct ReceiveSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var gameStatisticsEntity = SystemAPI.GetSingletonEntity<GameStatistics>();
            var gameStatistics = SystemAPI.GetComponent<GameStatistics>(gameStatisticsEntity);
            ref var gemStats = ref gameStatistics.gemStats;
            var entityCommandBuffer = SystemAPI.GetSingleton<EndFixedStepSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            var receiveJob = new ReceiveJob()
            {
                gemStats = gemStats,
                ecb = entityCommandBuffer.AsParallelWriter(),
                gameStatsEntity = gameStatisticsEntity
            };
            receiveJob.ScheduleParallel();
        }
    }

    [BurstCompile]
    public partial struct ReceiveJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter ecb;
        public BlobAssetReference<GemStats> gemStats;
        public Entity gameStatsEntity;
        
        public void Execute(
            ref Inventory inventory,
            in Courier courier,
            in Unit unit,
            ref FindTarget findTarget,
            Entity entity,
            [ChunkIndexInQuery] int chunkIndexInQuery)
        {
            if (!courier.onArrived) return;

            var factionIndex = (int) unit.faction;
            gemStats.Value.Array[factionIndex].count += inventory.gemCount;
            inventory.gemCount = 0;
            findTarget.targetEntity = Entity.Null;
            ecb.SetComponentEnabled<Courier>(chunkIndexInQuery, entity, false);
            ecb.SetComponentEnabled<OnChanged>(chunkIndexInQuery, gameStatsEntity, true);
            ecb.SetComponentEnabled<DeliveryEvent>(chunkIndexInQuery, entity, true);
        }
    }
}