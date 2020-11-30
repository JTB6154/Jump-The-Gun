using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New DynamicBusVolumeController", menuName = "Dynamic FMOD Bus Volume Controller", order = 51)]
public class DynamicBusVolumeController : ScriptableObject
{
    [SerializeField]
    private SoundBus soundBusType;

    [SerializeField]
    private float minInputValue;

    [SerializeField]
    private float maxInputValue;

    [SerializeField]
    [Tooltip("X-Axis: A converted value based on inputValue, minInputValue, and maxInputValue; \n" +
        "Y-Axis: Volume Level")]
    private AnimationCurve volumeCurve;

    public SoundBus SoundBusType { get => soundBusType; }
    public float GetOutputVolume(float inputValue)
    {
        float outputVolume = 0f;
        float convertedValue = (inputValue - minInputValue) / (maxInputValue - minInputValue);

        outputVolume = volumeCurve.Evaluate(convertedValue);

        return outputVolume;
    }
}
