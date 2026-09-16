using System;
using Unity.Behavior;
using Unity.Cinemachine;
using Unity.Properties;
using UnityEngine;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Jump", story: "Agent jumps", category: "Action", id: "un-id-super-cool")]
public partial class Jump : EnemyAction
{
    [SerializeReference] public BlackboardVariable<float> horizontalForce;
    [SerializeReference] public BlackboardVariable<float> jumpForce;

    [SerializeReference] public BlackboardVariable<float> buildupTime;
    [SerializeReference] public BlackboardVariable<float> jumpTime;

    public string animationTriggerName;

    [Header("Shake cam (if yes => put a profile")]
    public bool shakeCameraOnLanding;
    public ScreenShakeProfile shakeProfile;
    public CinemachineImpulseSource cinemachineImpulseSource;

    private bool hasLanded;

    protected override void Setup()
    {
        base.Setup();
        if (shakeCameraOnLanding)
            cinemachineImpulseSource = enemy.GetComponent<CinemachineImpulseSource>();
    }

    protected override Status OnStart()
    {
        base.OnStart();
        LeanTween.delayedCall(buildupTime, StartJump);

        return Status.Running;
    }

    private void StartJump()
    {
        var direction = player.transform.position.x < enemy.transform.position.x ? -1 : 1;
        rb2D.AddForce(new Vector2(horizontalForce * direction, jumpForce), ForceMode2D.Impulse);

        LeanTween.delayedCall(jumpTime, () =>
        {
            hasLanded = true;
            if(shakeCameraOnLanding && shakeProfile)
            {
                CameraShakeTrigger.Instance.ScreenShakeFromProfile(shakeProfile, cinemachineImpulseSource);
            }
        });

    }

    protected override Status OnUpdate()
    {
        return hasLanded ? Status.Success : Status.Running;
    }

    protected override void OnEnd()
    {
        hasLanded = false;
    }
}
