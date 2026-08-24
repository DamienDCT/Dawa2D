using Unity.Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CinemachineImpulseSource))]
public class CameraShakeTrigger : MonoBehaviour
{
    public static CameraShakeTrigger Instance;
    private CinemachineImpulseSource _impulseSource;

    private CinemachineImpulseDefinition _impulseDefinition;
    [SerializeField] private CinemachineImpulseListener[] cinemachineImpulseListeners;

    private void Awake()
    {
        _impulseSource = GetComponent<CinemachineImpulseSource>();
        Instance = this;
    }

    public void TriggerShake()
    {
        _impulseSource.GenerateImpulse();
    }

    public void ScreenShakeFromProfile(ScreenShakeProfile profile, CinemachineImpulseSource impulseSource)
    {
        Debug.Log("we go there");
        // Apply settings to profile
        UpdateProfile(profile, impulseSource);

        // Start impulse camera
        impulseSource.GenerateImpulseWithForce(profile.impactForce);
    }

    private void UpdateProfile(ScreenShakeProfile profile, CinemachineImpulseSource impulseSource)
    {
        _impulseDefinition = impulseSource.ImpulseDefinition;

        // Change the impulse source settings
        _impulseDefinition.ImpulseDuration = profile.impactTime;
        impulseSource.DefaultVelocity = profile.defaultVelocity;
        _impulseDefinition.CustomImpulseShape = profile.impulseCurve;

        // Change impulse listener settings

        for(int i = 0; i < cinemachineImpulseListeners.Length; i++)
        {
            cinemachineImpulseListeners[i].ReactionSettings.AmplitudeGain = profile.listenerAmplitude;
            cinemachineImpulseListeners[i].ReactionSettings.FrequencyGain = profile.listenerFrequency;
            cinemachineImpulseListeners[i].ReactionSettings.Duration = profile.listenerDuration;
        }

    }
}
