using System.Collections;
using UnityEngine;

public class CameraFollowObject : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private float _flipYRotationTime;

    private bool _isFacingRight;

    private PlayerMovements _player;

    

  //  private Coroutine _turnCoroutine;

    private void Awake()
    {
        _player = _playerTransform.gameObject.GetComponent<PlayerMovements>();

        _isFacingRight = _player.IsFacingRight;
    }

    private void Update()
    {
        // make the camera follow object follow's player position
     //   transform.position = _playerTransform.position;
    }

    public void CallTurn()
    {
        //   _turnCoroutine = StartCoroutine(FlipYLerp());
        //LeanTween.rotateY(gameObject, DetermineEndRotation(), _flipYRotationTime).setEaseInOutSine();
    }

  /*  private IEnumerator FlipYLerp()
    {
        float startRotation = transform.localEulerAngles.y;
        float endRotationAmount = DetermineEndRotation();

    }*/

    private float DetermineEndRotation()
    {
        _isFacingRight = !_isFacingRight;

        return _isFacingRight ? 180f : 0f;
    }
}
