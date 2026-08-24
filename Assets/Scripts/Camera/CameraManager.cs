using UnityEngine;
using Unity.Cinemachine;

/// <summary>
/// Pilote Weight0 d'une CinemachineMixingCamera en fonction de la distance
/// au sol détectée par un BoxCast.
///
/// - Si le BoxCast touche le sol : Weight0 est interpolé entre minDistance
///   (poids max) et maxDistance (poids min), selon hit.distance.
/// - Si le BoxCast ne touche rien (portée max atteinte) : on garde le
///   dernier poids valide pendant "noHitDelay" secondes, puis on
///   transitionne en douceur vers 0.
/// </summary>
[DisallowMultipleComponent]
public class CameraManager : MonoBehaviour
{
    [SerializeField] private CinemachineMixingCamera mixingCamera;

    [SerializeField] private Transform castOrigin;

    [SerializeField] private Vector2 boxSize = new Vector2(0.6f, 0.2f);
    [SerializeField] private Vector2 castDirectionLocal = Vector2.down;
    [SerializeField] private float maxCastDistance = 20f;
    [SerializeField] private LayerMask groundLayer;

    [SerializeField] private float minDistance = 1f;
    [SerializeField] private float maxDistance = 15f;

    [SerializeField] private float noHitDelay = 1.5f;

    [SerializeField] private float transitionSpeed = 3f;

    [SerializeField] private bool drawGizmos = true;

    private float _noHitTimer;
    private float _currentWeight;
    private bool _lastHit;
    private float _lastHitDistance;

    private void Reset()
    {
        castOrigin = transform;
    }

    private void Update()
    {
        float targetWeight = ComputeTargetWeight(out bool hasHit, out float distance);
        _lastHit = hasHit;
        _lastHitDistance = distance;

        _currentWeight = Mathf.MoveTowards(_currentWeight, targetWeight, transitionSpeed * Time.deltaTime);

        if (mixingCamera != null)
        {
            mixingCamera.Weight0 = _currentWeight;
        }
    }

    private float ComputeTargetWeight(out bool hasHit, out float distance)
    {
        Vector2 direction = ((Vector2)(castOrigin.rotation * castDirectionLocal)).normalized;
        float angle = castOrigin.eulerAngles.z;

        RaycastHit2D hit = Physics2D.BoxCast(
            castOrigin.position,
            boxSize,
            angle,
            direction,
            maxCastDistance,
            groundLayer);

        hasHit = hit.collider != null;

        if (hasHit)
        {
            _noHitTimer = 0f;
            distance = hit.distance;
            // InverseLerp(max, min, distance) : proche => 1, loin => 0
            float t = Mathf.InverseLerp(maxDistance, minDistance, distance);
            return Mathf.Clamp01(t);
        }

        distance = -1f;
        _noHitTimer += Time.deltaTime;

        if (_noHitTimer >= noHitDelay)
        {
            return 0f;
        }

        return _currentWeight;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (!drawGizmos || castOrigin == null) return;

        Vector2 direction = ((Vector2)(castOrigin.rotation * castDirectionLocal)).normalized;
        float shownDistance = _lastHit ? _lastHitDistance : maxCastDistance;

        Gizmos.color = _lastHit ? Color.green : Color.red;
        Gizmos.matrix = Matrix4x4.TRS(
            (Vector2)castOrigin.position + direction * shownDistance,
            Quaternion.Euler(0f, 0f, castOrigin.eulerAngles.z),
            Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, boxSize);
        Gizmos.matrix = Matrix4x4.identity;

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(castOrigin.position, (Vector2)castOrigin.position + direction * maxCastDistance);
    }
#endif
}