using DOTS.Authorings;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace DOTS.Systems
{
    public partial struct DeliverySystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var courierCallJob = new CourierCall
            {
                ecb = SystemAPI.GetSingleton<EndFixedStepSimulationEntityCommandBufferSystem.Singleton>()
                    .CreateCommandBuffer(state.WorldUnmanaged)
                    .AsParallelWriter()
            };
            courierCallJob.ScheduleParallel();
            
            var deliveryJob = new DeliveryJob();
            deliveryJob.ScheduleParallel();
        }
    }

    [BurstCompile]
    public partial struct DeliveryJob : IJobEntity
    {
        public void Execute(
            ref Courier courier, 
            in LocalTransform localTransform,
            ref UnitMover unitMover)
        {
            var moveDirection = localTransform.Position - courier.ClientLocalTransform.Position;
            if (math.lengthsq(moveDirection) <= UnitMoverSystem.ReachedTargetPositionDistanceSQ)
            {
                courier.onArrived = true;
                return;
            }
            unitMover.targetPosition = courier.ClientLocalTransform.Position;
        }
    }

    [BurstCompile]
    public partial struct CourierCall : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter ecb;
        
        public void Execute(
            in Inventory inventory,
            in Entity entity,
            [ChunkIndexInQuery] int chunkIndexInQuery)
        {
            if (inventory.onChanged)
            {
                ecb.SetComponentEnabled<Courier>(chunkIndexInQuery, entity, true);
            }
        }
    }
}