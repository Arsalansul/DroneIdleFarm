using System.Collections.Generic;
using Configs;
using DOTS.Authorings;
using ModestTree.Util;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace MonobehaviourBridges
{
    public class HybridHandler
    {
        public bool TryUpdateGemsCount(List<int> gemsCount)
        {
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var entityQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<GameStatistics>()
                .WithAll<OnChanged>()
                .Build(entityManager);

            var componentArray = entityQuery.ToComponentDataArray<GameStatistics>(Allocator.Temp);
            if (componentArray.Length == 0) return false;
        
            var gameStatistics = componentArray[0];
        
            ref var gemArray = ref gameStatistics.gemStats.Value.Array;
            for (int i = 0; i < gemsCount.Count; i++)
            {
                if (gemArray[i].count != gemsCount[i])
                {
                    gemsCount[i] = gemArray[i].count;
                }
            }
        
            var entities = entityQuery.ToEntityArray(Allocator.Temp);
            entityManager.SetComponentEnabled<OnChanged>(entities[0], false);

            return true;
        }
        
        public void SetDroneSettings(DroneSettings droneSettings, List<FactionUiSettings> factionUiSettings)
        {
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var entityQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<GameSettings>()
                .Build(entityManager);
            var componentArray = entityQuery.ToComponentDataArray<GameSettings>(Allocator.Temp);
            var entities = entityQuery.ToEntityArray(Allocator.Temp);
            var gameSettings = componentArray[0];

            for (int i = 0; i < gameSettings.factionsSettings.Value.droneSettings.Length; i++)
            {
                gameSettings.factionsSettings.Value.droneSettings[i].droneCount = droneSettings.startCount;
                gameSettings.factionsSettings.Value.droneSettings[i].droneSpeed = droneSettings.startSpeed;
                gameSettings.factionsSettings.Value.droneSettings[i].color = new float4(factionUiSettings[i].mainColor.r, factionUiSettings[i].mainColor.g, factionUiSettings[i].mainColor.b, 1);
            }
            entityManager.SetComponentData(entities[0], gameSettings);
        }

        public void SetDroneMoveSpeedSettings(Faction faction, float moveSpeed)
        {
            if ((int) faction < 0) return;
            
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var entityQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<GameSettings>()
                .Build(entityManager);
            var gameSettingsArray = entityQuery.ToComponentDataArray<GameSettings>(Allocator.Temp);
            var entities = entityQuery.ToEntityArray(Allocator.Temp);
            var gameSettings = gameSettingsArray[0];
            gameSettings.factionsSettings.Value.droneSettings[(int) faction].droneSpeed = moveSpeed;
            entityManager.SetComponentData(entities[0], gameSettings);
        }
        
        public void ChangeDronesSpeed(Faction faction, float dronesSpeed)
        {
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var entityQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<UnitMover>()
                .WithAll<Unit>()
                .Build(entityManager);
            var entities = entityQuery.ToEntityArray(Allocator.Temp);
            var unitMoverArray = entityQuery.ToComponentDataArray<UnitMover>(Allocator.Temp);
            var unitArray = entityQuery.ToComponentDataArray<Unit>(Allocator.Temp);

            for (int i = 0; i < unitMoverArray.Length; i++)
            {
                if (unitArray[i].faction != faction) continue;
                var unitMover = unitMoverArray[i];
                unitMover.moveSpeed = dronesSpeed;
                entityManager.SetComponentData(entities[i], unitMover);
            }
            
            SetDroneMoveSpeedSettings(faction, dronesSpeed);
        }
    
        public void ChangeDronesSpeed(int faction, float dronesSpeed) => ChangeDronesSpeed((Faction) faction, dronesSpeed);

        public void ChangeDronesCount(Faction faction, int count)
        {
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var entityQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<UnitSpawner>()
                .Build(entityManager);
            var entities = entityQuery.ToEntityArray(Allocator.Temp);
            var unitSpawnerArray = entityQuery.ToComponentDataArray<UnitSpawner>(Allocator.Temp);
        
            for (int i = 0; i < unitSpawnerArray.Length; i++)
            {
                if (unitSpawnerArray[i].faction != faction) continue;
                var unitSpawner = unitSpawnerArray[i];
                unitSpawner.maxCount = count;
                entityManager.SetComponentData(entities[i], unitSpawner);
            }
        }
    
        public void ChangeDronesCount(int faction, int count) => ChangeDronesCount((Faction) faction, count);

        public void ChangeTargetSpawnTime(float value)
        {
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var entityQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<TargetSpawner>()
                .Build(entityManager);
            var entities = entityQuery.ToEntityArray(Allocator.Temp);
            var targetSpawnerArray = entityQuery.ToComponentDataArray<TargetSpawner>(Allocator.Temp);

            for (int i = 0; i < targetSpawnerArray.Length; i++)
            {
                var targetSpawner = targetSpawnerArray[i];
                targetSpawner.cooldown = value;
                entityManager.SetComponentData(entities[i], targetSpawner);
            }
        }

        public bool TryUpdateReceiveTargetsEvents(LinkedList<Vector3> positions)
        {
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var entityQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<DeliveryEvent>()
                .WithPresent<Courier>()
                .Build(entityManager);
            var entities = entityQuery.ToEntityArray(Allocator.Temp);
            var deliveryEventArray = entityQuery.ToComponentDataArray<DeliveryEvent>(Allocator.Temp);
            
            if (deliveryEventArray.Length == 0) return false;

            for (int i = 0; i < deliveryEventArray.Length; i++)
            {
                var courier = entityManager.GetComponentData<Courier>(entities[i]);
                var vector = new Vector3(courier.ClientLocalTransform.Position.x, courier.ClientLocalTransform.Position.y, courier.ClientLocalTransform.Position.z);
                positions.AddLast(vector);
                entityManager.SetComponentEnabled<DeliveryEvent>(entities[i], false);
            }

            return true;
        }

        public void SetPickupSettings(Configs.PickupSettings pickupSettings)
        {
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var entityQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<GameSettings>()
                .Build(entityManager);
            var componentArray = entityQuery.ToComponentDataArray<GameSettings>(Allocator.Temp);
            var entities = entityQuery.ToEntityArray(Allocator.Temp);
            var gameSettings = componentArray[0];

            gameSettings.PickupSettings.time = pickupSettings.pickupTime;
            gameSettings.PickupSettings.reward = pickupSettings.reward;
            entityManager.SetComponentData(entities[0], gameSettings);
        }

        public void SetPhysicsSettings(Configs.PhysicsSettings physicsSettings)
        {
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var entityQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<GameSettings>()
                .Build(entityManager);
            var componentArray = entityQuery.ToComponentDataArray<GameSettings>(Allocator.Temp);
            var entities = entityQuery.ToEntityArray(Allocator.Temp);
            var gameSettings = componentArray[0];

            gameSettings.PhysicsSettings.targetLayer = physicsSettings.targetLayer;
            entityManager.SetComponentData(entities[0], gameSettings);
        }

        public void ToggleShowPath(bool value)
        {
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var entityQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<GameSettings>()
                .Build(entityManager);
            var componentArray = entityQuery.ToComponentDataArray<GameSettings>(Allocator.Temp);
            var entities = entityQuery.ToEntityArray(Allocator.Temp);
            var gameSettings = componentArray[0];

            gameSettings.showPath = value;
            entityManager.SetComponentData(entities[0], gameSettings);
        }

        public bool TryUpdateLineRenderers(LinkedList<ValuePair<Vector3, Vector3>> linesPositions)
        {
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var entityQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<GameSettings>()
                .Build(entityManager);
            var componentArray = entityQuery.ToComponentDataArray<GameSettings>(Allocator.Temp);
            var gameSettings = componentArray[0];
            
            if (!gameSettings.showPath) return false;
            
            var entityUnitMoverQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<UnitMover>()
                .WithAll<LocalTransform>()
                .Build(entityManager);
            var componentUnitMoverArray = entityUnitMoverQuery.ToComponentDataArray<UnitMover>(Allocator.Temp);
            var componentLocalTransformArray = entityUnitMoverQuery.ToComponentDataArray<LocalTransform>(Allocator.Temp);

            linesPositions.Clear();

            for (int i = 0; i < componentUnitMoverArray.Length; i++)
            {
                var startPosition = componentLocalTransformArray[i].Position;
                startPosition.y = 10;
                var endPosition = componentUnitMoverArray[i].targetPosition;
                endPosition.y = 10;
                linesPositions.AddLast(new ValuePair<Vector3, Vector3>(startPosition, endPosition));
            }

            return true;
        }

        public bool IsPickup()
        {
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var entityQuery = new EntityQueryBuilder(Allocator.Temp)
                .WithAll<EventsHandler>()
                .WithAll<PlayClipOnPickup>()
                .Build(entityManager);
            var playClipOnPickupArray = entityQuery.ToComponentDataArray<PlayClipOnPickup>(Allocator.Temp);
            
            if (playClipOnPickupArray.Length == 0) return false;
            
            var entities = entityQuery.ToEntityArray(Allocator.Temp);
            entityManager.SetComponentEnabled<PlayClipOnPickup>(entities[0], false);
            
            return true;
        }
    }
}