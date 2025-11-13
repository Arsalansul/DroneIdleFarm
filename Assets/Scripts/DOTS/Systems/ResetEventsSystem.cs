using DOTS.Authorings;
using Unity.Burst;
using Unity.Entities;

namespace DOTS.Systems
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup), OrderLast = true)]
    public partial struct ResetEventsSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var inventory in SystemAPI.Query<RefRW<Inventory>>())
            {
                inventory.ValueRW.onChanged = false;
            }
            foreach (var courier in SystemAPI.Query<RefRW<Courier>>().WithPresent<Courier>())
            {
                courier.ValueRW.onArrived = false;
            }
        }
    }
}