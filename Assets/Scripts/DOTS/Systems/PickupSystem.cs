using DOTS.Authorings;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;

namespace DOTS.Systems
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateAfter(typeof(PhysicsSystemGroup))]
    public partial struct PickupSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var simulation = SystemAPI.GetSingleton<SimulationSingleton>();
            var gameSettings = SystemAPI.GetSingleton<GameSettings>();
            var eventsHandlerEntity = SystemAPI.GetSingletonEntity<EventsHandler>();
            
            state.Dependency = new ProcessTriggerEventsJob()
            {
                Inventories = SystemAPI.GetComponentLookup<Inventory>(),
                Pickups = SystemAPI.GetComponentLookup<Pickup>(),
                deltaTime = SystemAPI.Time.DeltaTime,
                gameSettings = gameSettings,
            }.Schedule(simulation, state.Dependency);

            var cleanupJob = new CleanupPickupJob()
            {
                ecb = SystemAPI.GetSingleton<EndFixedStepSimulationEntityCommandBufferSystem.Singleton>()
                    .CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter(),
                eventsHandlerEntity = eventsHandlerEntity
            };
            cleanupJob.ScheduleParallel();
        }
    }

    [BurstCompile]
    public partial struct ProcessTriggerEventsJob : ITriggerEventsJob
    {
        [ReadOnly] public float deltaTime;
        [ReadOnly] public GameSettings gameSettings;
        
        public ComponentLookup<Inventory> Inventories;
        public ComponentLookup<Pickup> Pickups;
        
        public void Execute(TriggerEvent triggerEvent)
        {
            var entityA = triggerEvent.EntityA;
            var entityB = triggerEvent.EntityB;

            if (!Pickups.EntityExists(entityA) && !Pickups.EntityExists(entityB)) return;
            
            if (Inventories.TryGetRefRW(entityA, out var inventory) && Pickups.TryGetRefRW(entityB, out var pickup) ||
                Inventories.TryGetRefRW(entityB, out inventory) && Pickups.TryGetRefRW(entityA, out pickup))
            {
                if (pickup.ValueRO.activated) return;
                
                var inventoryEntity = Inventories.EntityExists(entityA) ? entityA : entityB;
                if (pickup.ValueRO.triggerEntity != inventoryEntity)
                {
                    pickup.ValueRW.timer = gameSettings.PickupSettings.time;
                    pickup.ValueRW.triggerEntity = inventoryEntity;
                }

                pickup.ValueRW.timer -= deltaTime;
                
                if (pickup.ValueRO.timer > 0 || pickup.ValueRO.activated) return;

                pickup.ValueRW.activated = true;
                inventory.ValueRW.gemCount += gameSettings.PickupSettings.reward;
                inventory.ValueRW.onChanged = true;
            }
        }
    }

    [BurstCompile]
    public partial struct CleanupPickupJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter ecb;
        public Entity eventsHandlerEntity;
        public void Execute(ref Pickup pickup, [ChunkIndexInQuery] int chunkIndex, Entity entity)
        {
            if (!pickup.activated) return;
            ecb.DestroyEntity(chunkIndex, entity);
            ecb.SetComponentEnabled<PlayClipOnPickup>(chunkIndex, eventsHandlerEntity, true);
        }
    }
}