using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

[RequireComponent(typeof(VisualEffect))]
public class DestroyVFX : MonoBehaviour
{
    private VisualEffect visualEffect;
    [SerializeField] private float destroyTimer = 2f;


    private void Awake()
    {
        visualEffect = GetComponent<VisualEffect>();

        //visualEffect.Play();
        Destroy(this.gameObject, destroyTimer);
    }

}
