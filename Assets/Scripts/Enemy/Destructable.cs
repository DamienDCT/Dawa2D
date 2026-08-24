using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

[RequireComponent(typeof(SpriteRenderer))]
public class Destructable : MonoBehaviour
{
    [Header("Die Animation Settings")]
    [SerializeField] private float dissolveTimer;
    [SerializeField] private VisualEffect dieVFXPrefab;

    [Header("Screen Shake Profile")]
    [SerializeField] private ScreenShakeProfile profile;
    private CinemachineImpulseSource impulseSource;

    [Space]
    [Header("Other settings")]
    [SerializeField] private HitType hitType;
    [SerializeField] private float blinkTime;
    [SerializeField] private Color hitColor;

    [SerializeField] private VisualEffect customHitEffect;
    [SerializeField] private AudioSource customHitSound;

    [SerializeField] private float healthAmount;

    [SerializeField] private Transform hitDamageVFXPrefab;
    private const string PARAMETER_HIT_VALUE_SHADER = "_BlendHitValue";
    private const string PARAMETER_SCALE_VALUE = "_Scale";


    //[SerializeField] private 

    private SpriteRenderer spriteRenderer;
    private Material hitMaterial;

    private float maxHealth;
    private bool canBeHit = true;

    private void Awake()
    {
        maxHealth = healthAmount;
        spriteRenderer = GetComponent<SpriteRenderer>();
        hitMaterial = spriteRenderer?.material;
        TryGetComponent<CinemachineImpulseSource>(out impulseSource);


        if(hitType == HitType.Color)
        {
            hitMaterial.SetColor("_HitColor", hitColor);
        }

    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.mKey.wasPressedThisFrame)
        {
            StartCoroutine(Die());
        }

        if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
        {
            ResetEnemy();
        }
    }

    private void ResetEnemy()
    {
        int id = Shader.PropertyToID(PARAMETER_SCALE_VALUE);

        Material material = spriteRenderer.material;





        if (material.HasProperty(id))
        {
            LeanTween.scaleX(this.transform.gameObject, 1f, 0.01f);

            material.SetFloat(id, 1f);
        }
    }

    public void Hit(float damageAmount, Vector2 hitPosition)
    {
        if (!canBeHit) return;
        healthAmount -= damageAmount;
        canBeHit = false;
        if (impulseSource != null)
            ApplyShakeEffect();

        ShowHitEffects();
        ShowHitVisuals(hitPosition);

        if(IsEnemyDead())
            StartCoroutine(Die());
    }

    private void ApplyShakeEffect()
    {
        CameraShakeTrigger.Instance.ScreenShakeFromProfile(profile, impulseSource);
    }

    private IEnumerator Die()
    {
        float currentTime = 0f;
        var material = spriteRenderer.material;
        
        int id = Shader.PropertyToID(PARAMETER_SCALE_VALUE);
        LeanTween.scaleX(this.transform.gameObject, 0f, dissolveTimer);
        Instantiate(dieVFXPrefab, transform.position, Quaternion.identity);
       // CameraShakeTrigger.Instance.TriggerShake();

        while (currentTime < dissolveTimer)
        {
            currentTime += Time.deltaTime;
            float currentScale = Mathf.Lerp(1f, 0f, currentTime / dissolveTimer);

            if (material.HasProperty(id))
            {
                material.SetFloat(id, currentScale);
            }
            yield return null;
        }
    }

    private bool IsEnemyDead()
    {
        return healthAmount <= 0f;
    }

    private void ShowHitEffects()
    {
        // Play the SFX in case it exists
        if (customHitSound != null)
        {
            customHitSound.Play();
        }

        // Play the VFX in case it exists
        if (customHitEffect != null)
        {
            customHitEffect.Play();
        }
    }

    private void ShowHitVisuals(Vector2 hitPosition)
    {
        if (hitType == HitType.None) return;

        if(hitDamageVFXPrefab != null)
        {
            Transform _vfxInstantiated = Instantiate(hitDamageVFXPrefab, hitPosition, Quaternion.identity);
          //  _vfxInstantiated.GetComponent<VisualEffect>().Play();
        }

        switch (hitType)
        {
            case HitType.Color:
                StartCoroutine(ShowSprite());
                break;
            default:
                return;
        }
    }

    private IEnumerator ShowSprite()
    {
        float duration = blinkTime / 2; // Environ 3 frames
        float timer = 0f;

        // Phase 1 : Monter à 1
        while (timer <= duration)
        {
            timer += Time.deltaTime;
            float t = Mathf.Lerp(0f, 1f, timer / duration);
            spriteRenderer.material.SetFloat(PARAMETER_HIT_VALUE_SHADER, t);

            yield return null;
        }

        spriteRenderer.material.SetFloat(PARAMETER_HIT_VALUE_SHADER, 1f);

        timer = 0f;

        // Phase 2 : Redescendre à 0
        while (timer <= duration)
        {
            timer += Time.deltaTime;
            float t = Mathf.Lerp(1f, 0f, timer / duration);
            spriteRenderer.material.SetFloat(PARAMETER_HIT_VALUE_SHADER, t);

            yield return null;
        }

        spriteRenderer.material.SetFloat(PARAMETER_HIT_VALUE_SHADER, 0f);

        canBeHit = true;
    }
}

[System.Serializable]
public enum HitType
{
    None,
    Color
}
