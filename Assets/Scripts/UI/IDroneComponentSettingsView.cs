using System;

namespace UI
{
    public interface IDroneComponentSettingsView
    {
        void SetMinMaxValues(float min, float max);
        void SetValue(float value);
        event Action<IDroneComponentSettingsView, float> onValueChanged;
    }
}