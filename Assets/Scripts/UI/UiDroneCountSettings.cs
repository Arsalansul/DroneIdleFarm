using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UiDroneCountSettings : MonoBehaviour, IDroneComponentSettingsView
    {
        [SerializeField] private Slider count;
        [SerializeField] private Text countText;
        
        public void SetMinMaxValues(float min, float max)
        {
            count.minValue = min;
            count.maxValue = max;
        }

        public void SetValue(float value)
        {
            count.value = value;
        }

        public event Action<IDroneComponentSettingsView, float> onValueChanged;
        
        
        private void OnEnable()
        {
            count.onValueChanged.AddListener(OnMoveSpeedValueChanged);
        }

        private void OnDisable()
        {
            count.onValueChanged.RemoveAllListeners();
        }

        private void OnMoveSpeedValueChanged(float value)
        {
            countText.text = $"count: {(int) value}";
            onValueChanged?.Invoke(this, value);
        }
    }
}