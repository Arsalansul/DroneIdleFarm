using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI
{
    [Serializable]
    public class UiPrefabs
    {
        public ScoreView scoreView;
        public UiDroneSettingsView droneSettingsView;
    }
    public class UiManager : MonoBehaviour
    {
        [SerializeField] private Transform scoreViewContainer;
        [SerializeField] private Transform droneSettingsViewContainer;
        [SerializeField] private InputField targetSpawnTimeInputField;
        [SerializeField] private Toggle showPathToggle;
        
        [Inject] private UiPrefabs uiPrefabs;

        public event Action<int, float> OnDroneMoveSpeedSettings;
        public event Action<int, int> OnDroneCountSettings;
        public event Action<float> OnSpawnTimeInput;
        public event Action<bool> OnShowPath;

        private List<ScoreView> scoreViews = new List<ScoreView>();
        private List<UiDroneSettingsView> droneSettingsViews = new List<UiDroneSettingsView>();

        public void SetScore(int index, int score)
        {
            scoreViews[index].SetScore(score);
        }

        public void AddScoreView(Color mainColor, Color textColor)
        {
            var scoreView = Instantiate(uiPrefabs.scoreView, scoreViewContainer);
            scoreView.SetBackgroundColor(mainColor);
            scoreView.SetTextColor(textColor);
            scoreView.SetScore(0);
            scoreViews.Add(scoreView);
        }

        public void AddDroneSettingsView(float minSpeed, float maxSpeed, int minCount, int maxCount, Color mainColor, Color textColor)
        {
            var droneSettingsView = Instantiate(uiPrefabs.droneSettingsView, droneSettingsViewContainer);
            
            droneSettingsView.OnMoveSettingsChange += OnDroneMoveSpeedSettingsChanged;
            droneSettingsView.SetMoveSettings(minSpeed, maxSpeed);
            
            droneSettingsView.OnCountSettingsChange += OnDroneCountSettingsChanged;
            droneSettingsView.SetCountSettings(minCount, maxCount);
            
            droneSettingsView.SetBackgroundColor(mainColor);
            
            droneSettingsViews.Add(droneSettingsView);
        }

        public void SetDroneMoveSpeedSettings(int index, float speed)
        {
            droneSettingsViews[index].SetMoveSpeedValue(speed);
        }

        public void SetDroneCountSettings(int index, int count)
        {
            droneSettingsViews[index].SetCountValue(count);
        }

        private void OnDroneMoveSpeedSettingsChanged(UiDroneSettingsView droneSettings, float value)
        {
            OnDroneMoveSpeedSettings?.Invoke(droneSettingsViews.IndexOf(droneSettings), value);
        }
        
        private void OnDroneCountSettingsChanged(UiDroneSettingsView droneSettings, int value)
        {
            OnDroneCountSettings?.Invoke(droneSettingsViews.IndexOf(droneSettings), value);
        }

        private void OnTargetSpawnTimeInput(string value)
        {
            value = value.Replace('.', ',');
            if (float.TryParse(value, out float result))
            {
                OnSpawnTimeInput?.Invoke(result);
            }
        }

        private void OnShowPathToggle(bool value)
        {
            OnShowPath?.Invoke(value);
        }

        private void OnEnable()
        {
            targetSpawnTimeInputField.onEndEdit.AddListener(OnTargetSpawnTimeInput);
            showPathToggle.onValueChanged.AddListener(OnShowPathToggle);
        }

        private void OnDisable()
        {
            targetSpawnTimeInputField.onEndEdit.RemoveAllListeners();
            showPathToggle.onValueChanged.RemoveAllListeners();
        }

        private void OnDestroy()
        {
            foreach (var droneSettingsView in droneSettingsViews)
            {
                droneSettingsView.OnMoveSettingsChange -= OnDroneMoveSpeedSettingsChanged;
                droneSettingsView.OnCountSettingsChange -= OnDroneCountSettingsChanged;
            }
        }
    }
}