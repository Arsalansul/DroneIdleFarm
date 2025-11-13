using DOTS.Authorings;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

namespace DOTS.Systems
{
    public partial struct UnitSpawnerVisualSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var entityCommandBuffer = new EntityCommandBuffer(Allocator.Temp);
            var gameSettings = SystemAPI.GetSingleton<GameSettings>();
            foreach (var (unitSpawner, entity) in SystemAPI.Query<RefRW<UnitSpawner>>().WithEntityAccess())
            {
                ref var droneSettings = ref gameSettings.factionsSettings.Value.droneSettings;
                var colorComponent = new URPMaterialPropertyBaseColor();
                colorComponent.Value = droneSettings[(int) unitSpawner.ValueRO.faction].color;
                var query = new EntityQueryBuilder(Allocator.Temp)
                    .WithAll<URPMaterialPropertyBaseColor>()
                    .Build(state.EntityManager);
                entityCommandBuffer.SetComponentForLinkedEntityGroup(entity, query.GetEntityQueryMask(), colorComponent);
            }
            
            entityCommandBuffer.Playback(state.EntityManager);
            entityCommandBuffer.Dispose();
            state.Enabled = false;
        }
    }
}