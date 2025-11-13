using DOTS.Authorings;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

namespace DOTS.Systems
{
    public partial struct SpawnUnitsSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var entityCommandBuffer = new EntityCommandBuffer(Allocator.Temp);
            var gameSettings = SystemAPI.GetSingleton<GameSettings>();
            foreach (var (unitSpawner, localTransform, spawner, entity) in 
                     SystemAPI.Query<RefRW<UnitSpawner>, RefRO<LocalTransform>, RefRW<Spawner>>().WithEntityAccess())
            {
                ref var droneSettings = ref gameSettings.factionsSettings.Value.droneSettings;
                if (unitSpawner.ValueRO.maxCount == 0)
                {
                    unitSpawner.ValueRW.maxCount = droneSettings[(int) unitSpawner.ValueRO.faction].droneCount;
                }
                if (unitSpawner.ValueRO.count == unitSpawner.ValueRO.maxCount) continue;
                
                var entityBuffer = SystemAPI.GetBuffer<EntityBuffer>(entity);
                
                if (unitSpawner.ValueRO.count > unitSpawner.ValueRO.maxCount)
                {
                    var unitLastEntity = entityBuffer[^1];
                    entityBuffer.RemoveAt(entityBuffer.Length - 1);
                    entityCommandBuffer.DestroyEntity(unitLastEntity.entity);
                    unitSpawner.ValueRW.count--;
                    continue;
                }
                var unitEntity = entityCommandBuffer.Instantiate(spawner.ValueRO.entity);
                
                var unitComponent = SystemAPI.GetComponent<Unit>(spawner.ValueRO.entity);
                unitComponent.faction = unitSpawner.ValueRO.faction;
                entityCommandBuffer.SetComponent(unitEntity, unitComponent);
                
                var spawnPosition = localTransform.ValueRO.Position + GetNextSpawnPosition(unitSpawner.ValueRO.count, unitSpawner.ValueRO.ringAdditionalSize);
                entityCommandBuffer.SetComponent(unitEntity, LocalTransform.FromPosition(spawnPosition));
                
                var unitMover = SystemAPI.GetComponent<UnitMover>(spawner.ValueRO.entity);
                unitMover.targetPosition = spawnPosition;
                unitMover.moveSpeed = droneSettings[(int) unitSpawner.ValueRO.faction].droneSpeed;
                
                entityCommandBuffer.SetComponent(unitEntity, unitMover);

                var courier = SystemAPI.GetComponent<Courier>(spawner.ValueRO.entity);
                courier.ClientLocalTransform = localTransform.ValueRO;
                
                entityCommandBuffer.SetComponent(unitEntity, courier);
                
                var colorComponent = new URPMaterialPropertyBaseColor();
                colorComponent.Value = droneSettings[(int) unitSpawner.ValueRO.faction].color;
                var query = new EntityQueryBuilder(Allocator.Temp)
                    .WithAll<URPMaterialPropertyBaseColor>()
                    .Build(state.EntityManager);
                entityCommandBuffer.SetComponentForLinkedEntityGroup(unitEntity, query.GetEntityQueryMask(), colorComponent);
                
                unitSpawner.ValueRW.count++;

                entityCommandBuffer.AppendToBuffer(entity, new EntityBuffer(){entity = unitEntity});
            }
            
            entityCommandBuffer.Playback(state.EntityManager);
            entityCommandBuffer.Dispose();
        }

        //спаун позиции кольцами. каждое следующее кольце вмещает больше спаун поинтов по формуле ringIndex * 2 + 3.
        //при желании формулу можно изменить, ну и поправить другие вычисления
        private float3 GetNextSpawnPosition(int index, float ringAdditionalSize)
        {
            if (index == 0) return new float3(0, 0, 0);

            var ringIndex = GetRingIndex(index);
            var countInRing = ringIndex * 2 + 3;
            var indexOnRing = index - (ringIndex - 1) * 2 - 3;
            var angle = indexOnRing * math.PI2 / countInRing;
            return math.rotate(quaternion.RotateY(angle), new float3(ringAdditionalSize * ringIndex, 0f, 0f));
        }

        private int GetRingIndex(int index)
        {
            var result = 1;
            var countInRing = result * 2 + 3;
            var count = countInRing;
            while (index > count)
            {
                result++;
                countInRing = result * 2 + 3;
                count += countInRing;
            }
            return result;
        }
    }
}