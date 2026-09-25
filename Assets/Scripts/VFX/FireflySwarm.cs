using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class FireflySwarm : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private VisualEffect visualEffect;
    [SerializeField] private Transform player;

    [Header("Detection")]
    [SerializeField] private float fleeDistance = 2.5f;

    [Header("Flight")]
    [SerializeField] private float takeOffDuration = 0.5f;
    [SerializeField] private float flightDuration = 0.8f;
    [SerializeField] private float panicDuration = 0.15f;

    [Header("Curves")]
    [SerializeField] private AnimationCurve fleeAmountCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);


    private bool hasFled;

    private static readonly int FleeAmount = Shader.PropertyToID("FleeAmount");
    private static readonly int FleeDirection = Shader.PropertyToID("FleeDirection");
    private static readonly int FleeSide = Shader.PropertyToID("FleeSide");
    private static readonly int FlightProgress = Shader.PropertyToID("FlightProgress");
    private static readonly int PanicAmount = Shader.PropertyToID("PanicAmount");
    private static readonly int PanicProgress = Shader.PropertyToID("PanicProgress");

    private void Start()
    {
        visualEffect.SetFloat(FleeAmount, 0f);
        visualEffect.SetFloat(FlightProgress, 0f);
        visualEffect.SetFloat(PanicAmount, 1f);
        visualEffect.SetFloat(PanicProgress, 0f);

        // En 2D, la variation se fera verticalement.
        visualEffect.SetVector3(FleeSide, Vector3.up);
    }

    private void Update()
    {
        if (hasFled || player == null)
            return;

        float distance = Vector2.Distance(
            transform.position,
            player.position
        );

        if (distance <= fleeDistance)
        {
            StartCoroutine(Flee());
        }
    }

    private IEnumerator Flee()
    {
        hasFled = true;

        // -------------------------
        // RUNAWAY DIRECTION
        // -------------------------

        float directionX = Mathf.Sign(
            transform.position.x - player.position.x
        );

        //Vector3 direction = new Vector3(
        //    directionX * 0.2f,
        //    0f,
        //    20f
        //).normalized;
        Vector3 direction = new Vector3(
            directionX,
            1f,
            0f
        ).normalized;

        visualEffect.SetVector3(FleeDirection, direction);

        Vector3 side = new Vector3(
            -direction.y,
            direction.x,
            0f
        ).normalized;

        visualEffect.SetVector3(FleeSide, side);

        // -------------------------
        // PANIC
        // -------------------------

        float timer = 0f;

        while (timer < panicDuration)
        {
            timer += Time.deltaTime;

            float progress = Mathf.Clamp01(
                timer / panicDuration
            );

            visualEffect.SetFloat(PanicProgress, progress);

            yield return null;
        }

        visualEffect.SetFloat(PanicProgress, 1f);

        // -------------------------
        // FLYING
        // -------------------------

        timer = 0f;

        while (timer < takeOffDuration)
        {
            timer += Time.deltaTime;

            float progress = Mathf.Clamp01(
                timer / takeOffDuration
            );

            float curvedProgress = fleeAmountCurve.Evaluate(progress);

            visualEffect.SetFloat(FleeAmount, curvedProgress);

            yield return null;
        }

        visualEffect.SetFloat(FleeAmount, 1f);

        // -------------------------
        // VOL
        // -------------------------

        timer = 0f;

        while (timer < flightDuration)
        {
            timer += Time.deltaTime;

            float progress = Mathf.Clamp01(
                timer / flightDuration
            );

            visualEffect.SetFloat(FlightProgress, progress);

            yield return null;
        }

        visualEffect.SetFloat(FlightProgress, 1f);

        // On laisse le VFX terminer son fade.
        yield return new WaitForSeconds(0.2f);

        visualEffect.enabled = false;
    }
}