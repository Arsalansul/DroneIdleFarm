using System;
using Configs;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace DOTS.Authorings
{
    public class GameSettingsAuthoring : MonoBehaviour
    {
        private class Baker : Baker<GameSettingsAuthoring>
        {
            public override void Bake(GameSettingsAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                var settings = CreateSettingsBlobAsset();
                AddComponent(entity, new GameSettings
                {
                    factionsSettings = settings
                });
            }
            
            private BlobAssetReference<FactionSettingsArray> CreateSettingsBlobAsset()
            {
                using var builder = new BlobBuilder(Allocator.Temp);
                ref var settingsBlob = ref builder.ConstructRoot<FactionSettingsArray>();

                var count = Enum.GetNames(typeof(Faction)).Length;
                var arrayBuilder = builder.Allocate(ref settingsBlob.droneSettings, count);

                // for (var i = 0; i < count; i++)
                // {
                //     arrayBuilder[i].droneCount = 0; //можно из сохранения значения подставлять будет
                //     arrayBuilder[i].droneSpeed = 0; //можно из сохранения значения подставлять будет
                // }

                return builder.CreateBlobAssetReference<FactionSettingsArray>(Allocator.Persistent);
            }
        }
    }

    public struct GameSettings : IComponentData
    {
        public BlobAssetReference<FactionSettingsArray> factionsSettings;
        public PickupSettings PickupSettings;
        public PhysicsSettings PhysicsSettings;
        public bool showPath;
    }

    public struct FactionSettingsArray
    {
        public BlobArray<FactionSettings> droneSettings;
    }

    public struct FactionSettings
    {
        public float droneSpeed;
        public int droneCount;
        public float4 color;
    }

    public struct PickupSettings
    {
        public float time;
        public int reward;
    }

    public struct PhysicsSettings
    {
        public int targetLayer;
    }
}