using DOTS.Authorings;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace DOTS.Systems
{
    public partial struct UnitMoverSystem : ISystem
    {
        public const float ReachedTargetPositionDistanceSQ = 0.01f;

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var unitMoverJob = new UnitMoverJob
            {
                deltaTime = SystemAPI.Time.DeltaTime
            };

            unitMoverJob.ScheduleParallel();
        }
    }

    [BurstCompile]
    public partial struct UnitMoverJob : IJobEntity
    {
        [ReadOnly] public float deltaTime;

        public void Execute(ref LocalTransform localTransform, ref UnitMover unitMover)
        {
            var moveDirection = unitMover.targetPosition - localTransform.Position;
            var lookDirection = unitMover.lookPosition - localTransform.Position;
            lookDirection.y = 0;

            if (math.lengthsq(lookDirection) > UnitMoverSystem.ReachedTargetPositionDistanceSQ)
            {
                localTransform.Rotation = 
                    math.slerp(localTransform.Rotation,
                    quaternion.LookRotation(lookDirection, math.up()),
                    deltaTime * unitMover.rotationSpeed);
            }

            if (math.lengthsq(moveDirection) <= UnitMoverSystem.ReachedTargetPositionDistanceSQ)
            {
                return;
            }

            moveDirection = math.normalize(moveDirection);
            localTransform.Position += moveDirection * deltaTime * unitMover.moveSpeed;
        }
    }
}