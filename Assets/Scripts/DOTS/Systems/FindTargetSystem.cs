using DOTS.Authorings;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;

namespace DOTS.Systems
{
    public partial struct FindTargetSystem : ISystem
    {
        private NativeList<DistanceHit> distanceHits;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            distanceHits = new NativeList<DistanceHit>(Allocator.Persistent);
        }
        
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            //todo job
            var physicsWorldSingleton = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
            var gameSettings = SystemAPI.GetSingleton<GameSettings>();
            var collisionWorld = physicsWorldSingleton.CollisionWorld;
            var collisionFilter = new CollisionFilter
            {
                BelongsTo = ~0u,
                CollidesWith = 1u << gameSettings.PhysicsSettings.targetLayer,
                GroupIndex = 0
            };
            foreach (var (localTransform, findTarget, unitMover) in
                     SystemAPI.Query<RefRO<LocalTransform>, RefRW<FindTarget>, RefRW<UnitMover>>())
            {
                if (findTarget.ValueRO.targetEntity != Entity.Null) continue;
                
                distanceHits.Clear();
                
                if (!collisionWorld.OverlapSphere(
                        localTransform.ValueRO.Position, 
                        findTarget.ValueRO.radius,
                        ref distanceHits, 
                        collisionFilter)) continue;
                
                var minDistance = float.MaxValue;
                DistanceHit minDistanceHit = new DistanceHit();
                RefRW<Target> target = new RefRW<Target>();
                
                foreach (var distanceHit in distanceHits)
                {
                    if (!SystemAPI.Exists(distanceHit.Entity) || 
                        !SystemAPI.HasComponent<Target>(distanceHit.Entity)) continue;
                        
                    var distanceHitTarget = SystemAPI.GetComponentRW<Target>(distanceHit.Entity);

                    if (distanceHitTarget.ValueRO.targeted) continue;
                    if (minDistance < distanceHit.Distance) continue;
                        
                    minDistance = distanceHit.Distance;
                    minDistanceHit = distanceHit;
                    target = distanceHitTarget;
                }

                if (minDistance < float.MaxValue)
                {
                    findTarget.ValueRW.targetEntity = minDistanceHit.Entity;
                    unitMover.ValueRW.targetPosition = minDistanceHit.Position;
                    target.ValueRW.targeted = true;
                }
            }
        }
    }
}