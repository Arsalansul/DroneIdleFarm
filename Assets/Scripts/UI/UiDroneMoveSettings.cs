using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UiDroneMoveSettings : MonoBehaviour, IDroneComponentSettingsView
    {
        [SerializeField] private Slider moveSpeed;
        [SerializeField] private Text moveSpeedText;
        
        public void SetMinMaxValues(float min, float max)
        {
            moveSpeed.minValue = min;
            moveSpeed.maxValue = max;
        }

        public void SetValue(float value)
        {
            moveSpeed.value = value;
        }

        public event Action<IDroneComponentSettingsView, float> onValueChanged;
        
        private void OnEnable()
        {
            moveSpeed.onValueChanged.AddListener(OnMoveSpeedValueChanged);
        }

        private void OnDisable()
        {
            moveSpeed.onValueChanged.RemoveAllListeners();
        }

        private void OnMoveSpeedValueChanged(float value)
        {
            moveSpeedText.text = $"moveSpeed: {String.Format("{0:0.00}", value)}";
            onValueChanged?.Invoke(this, value);
        }
    }
}