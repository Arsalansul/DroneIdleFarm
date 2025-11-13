using System;
using System.Collections.Generic;
using UI;
using UnityEngine;
using Zenject;

namespace Configs
{
    [CreateAssetMenu(fileName = "GameSettingsInstaller", menuName = "Configs/GameSettingsInstaller", order = 0)]
    public class GameSettingsInstaller : ScriptableObjectInstaller<GameSettingsInstaller>
    {
        public List<FactionUiSettings> Factions;
        public UiPrefabs uiPrefabs;
        public DroneSettings droneSettings;
        public PickupSettings pickSettings;
        public PhysicsSettings physicsSettings;
        public AudioSettings audioSettings;
    
        public override void InstallBindings()
        {
            Container.BindInstance(Factions);
            Container.BindInstance(uiPrefabs);
            Container.BindInstance(droneSettings);
            Container.BindInstance(pickSettings);
            Container.BindInstance(physicsSettings);
            Container.BindInstance(audioSettings);
        }
    }

    [Serializable]
    public class FactionUiSettings
    {
        public Faction faction;
        public Color mainColor;
        public Color textColor;
    }

    [Serializable]
    public class DroneSettings
    {
        [Header("Speed")]
        public float startSpeed;
        public float minSpeed;
        public float maxSpeed;
        [Header("Count")] 
        public int startCount;
        public int minCount;
        public int maxCount;
    }

    [Serializable]
    public class PickupSettings
    {
        public float pickupTime;
        public int reward;
    }

    [Serializable]
    public class PhysicsSettings
    {
        public int targetLayer;
    }

    [Serializable]
    public class AudioSettings
    {
        public AudioClip receiveGemClip;
        public AudioClip pickupGemClip;
    }
}