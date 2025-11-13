using System.Collections.Generic;
using System.Linq;
using Configs;
using ModestTree.Util;
using MonobehaviourBridges;
using UI;
using UnityEngine;
using Zenject;

public class GameManager : MonoBehaviour
{
    [Inject] private UiManager uiManager;
    [Inject] private List<FactionUiSettings> factionUiSettings;
    [Inject] private DroneSettings droneSettings;
    [Inject] private PickupSettings pickupSettings;
    [Inject] private PhysicsSettings physicsSettings;
    [Inject] private PoolManager poolManager;
    [Inject] private Configs.AudioSettings audioSettings;

    private List<int> gemsCount = new ();
    private LinkedList<Vector3> receiveGemPositions = new ();
    private LinkedList<ValuePair<Vector3, Vector3>> linesPositions = new ();
    private LinkedList<LineRenderer> linesRenderers = new ();
    private HybridHandler hybridHandler = new ();
    
    private void Start()
    {
        hybridHandler.SetDroneSettings(droneSettings, factionUiSettings);
        hybridHandler.SetPickupSettings(pickupSettings);
        hybridHandler.SetPhysicsSettings(physicsSettings);
        for (int i = 0; i < factionUiSettings.Count; i++)
        {
            var factionSettings = factionUiSettings.First(settings => (int) settings.faction == i);
            uiManager.AddScoreView(factionSettings.mainColor, factionSettings.textColor);
            gemsCount.Add(0);
            
            uiManager.AddDroneSettingsView(droneSettings.minSpeed, droneSettings.maxSpeed, droneSettings.minCount, droneSettings.maxCount, factionSettings.mainColor, factionSettings.textColor);
            uiManager.SetDroneMoveSpeedSettings(i, droneSettings.startSpeed);
            uiManager.SetDroneCountSettings(i, droneSettings.startCount);
        }
    }

    private void Update()
    {
        if (hybridHandler.TryUpdateGemsCount(gemsCount))
        {
            for (int i = 0; i < gemsCount.Count; i++)
            {
                uiManager.SetScore(i, gemsCount[i]);
            }
        }

        if (hybridHandler.TryUpdateReceiveTargetsEvents(receiveGemPositions))
        {
            foreach (var position in receiveGemPositions)
            {
                var particle = poolManager.GetParticlesFromPool(position);
                particle.Play();
                poolManager.ReturnParticleToPool(particle, particle.main.duration);
            }
            receiveGemPositions.Clear();
            PlayAudioClipOneShot(audioSettings.receiveGemClip);
        }

        // if (hybridHandler.IsPickup())
        // {
        //     PlayAudioClipOneShot(audioSettings.pickupGemClip);
        // }
        
        foreach (var lineRenderer in linesRenderers)
        {
            poolManager.ReturnLineRendererToPool(lineRenderer);
        }
        linesRenderers.Clear();

        if (hybridHandler.TryUpdateLineRenderers(linesPositions))
        {
            foreach (var line in linesPositions)
            {
                var lineRenderer = poolManager.GetLineRendererFromPool();
                lineRenderer.positionCount = 2;
                lineRenderer.SetPosition(0, line.First);
                lineRenderer.SetPosition(1, line.Second);
                linesRenderers.AddLast(lineRenderer);
            }
        }
    }

    private void PlayAudioClipOneShot(AudioClip audioClip)
    {
        var audioSource = poolManager.GetAudioSource();
        audioSource.clip = audioClip;
        audioSource.Play();
        poolManager.ReturnAudioSourceToPool(audioSource, audioSource.clip.length);
    }

    private void OnEnable()
    {
        uiManager.OnDroneMoveSpeedSettings += hybridHandler.ChangeDronesSpeed;
        uiManager.OnDroneCountSettings += hybridHandler.ChangeDronesCount;
        uiManager.OnSpawnTimeInput += hybridHandler.ChangeTargetSpawnTime;
        uiManager.OnShowPath += hybridHandler.ToggleShowPath;
    }

    private void OnDisable()
    {
        uiManager.OnDroneMoveSpeedSettings -= hybridHandler.ChangeDronesSpeed;
        uiManager.OnDroneCountSettings -= hybridHandler.ChangeDronesCount;
        uiManager.OnSpawnTimeInput -= hybridHandler.ChangeTargetSpawnTime;
        uiManager.OnShowPath -= hybridHandler.ToggleShowPath;
    }
}
