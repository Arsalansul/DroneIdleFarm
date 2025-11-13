using System;
using DOTS.Authorings;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using Random = Unity.Mathematics.Random;

namespace DOTS.Systems
{
    public partial struct SpawnTargetsSystem : ISystem
    {
        private Random random;
        private NativeList<DistanceHit> distanceHits;

        // [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            random = new Random((uint)DateTime.Now.Ticks);
            distanceHits = new NativeList<DistanceHit>(Allocator.Persistent);
        }
        
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var entityCommandBuffer = new EntityCommandBuffer(Allocator.Temp);
            var physicsWorldSingleton = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
            var gameSettings = SystemAPI.GetSingleton<GameSettings>();
            var collisionWorld = physicsWorldSingleton.CollisionWorld;
            var collisionFilter = new CollisionFilter
            {
                BelongsTo = ~0u,
                CollidesWith = 1u << gameSettings.PhysicsSettings.targetLayer,
                GroupIndex = 0
            };
            foreach (var (targetSpawner, localTransform, spawner) in 
                     SystemAPI.Query<RefRW<TargetSpawner>, RefRO<LocalTransform>, RefRO<Spawner>>())
            {
                targetSpawner.ValueRW.timer -= SystemAPI.Time.DeltaTime;

                if (targetSpawner.ValueRO.timer > 0) continue;
                
                targetSpawner.ValueRW.timer = targetSpawner.ValueRO.cooldown;
                var targetEntity = entityCommandBuffer.Instantiate(spawner.ValueRO.entity);
                var spawnPosition = new float3();
                
                distanceHits.Clear();

                for (int i = 0; i < 100; i++)
                {
                    var direction = new float3(random.NextFloat(-1f, 1f), 0, random.NextFloat(-1f, 1f));
                    spawnPosition = localTransform.ValueRO.Position + math.normalize(direction) * random.NextFloat(targetSpawner.ValueRO.radius);
                    if (!collisionWorld.OverlapSphere(spawnPosition, targetSpawner.ValueRO.minDistance, ref distanceHits, collisionFilter))
                        break;
                }
                
                entityCommandBuffer.SetComponent(targetEntity, LocalTransform.FromPosition(spawnPosition));
            }
            
            entityCommandBuffer.Playback(state.EntityManager);
            entityCommandBuffer.Dispose();
        }
    }
}