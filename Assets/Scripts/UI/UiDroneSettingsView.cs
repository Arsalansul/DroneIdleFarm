using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UiDroneSettingsView : MonoBehaviour
    {
        [SerializeField] private UiDroneMoveSettings moveSettingsView;
        [SerializeField] private UiDroneCountSettings countSettingsView;
        [SerializeField] private RawImage backgroundImage;

        public event Action <UiDroneSettingsView, float> OnMoveSettingsChange;
        public event Action <UiDroneSettingsView, int> OnCountSettingsChange;
        private void OnEnable()
        {
            moveSettingsView.onValueChanged += OnMoveSettingsChanged;
            countSettingsView.onValueChanged += OnCountSettingsChanged;
        }

        private void OnDisable()
        {
            moveSettingsView.onValueChanged -= OnMoveSettingsChanged;
            countSettingsView.onValueChanged -= OnCountSettingsChanged;
        }

        public void SetBackgroundColor(Color color) => backgroundImage.color = color;
        
        public void SetMoveSettings(float minSpeed, float maxSpeed) => moveSettingsView.SetMinMaxValues(minSpeed, maxSpeed);
        public void SetMoveSpeedValue(float speed) => moveSettingsView.SetValue(speed);
        
        public void SetCountSettings(int minCount, int maxCount) => countSettingsView.SetMinMaxValues(minCount, maxCount);
        public void SetCountValue(int count) => countSettingsView.SetValue(count);

        private void OnMoveSettingsChanged(IDroneComponentSettingsView view, float value) => OnMoveSettingsChange?.Invoke(this, value);
        private void OnCountSettingsChanged(IDroneComponentSettingsView view, float value) => OnCountSettingsChange?.Invoke(this, (int) value);
    }
}