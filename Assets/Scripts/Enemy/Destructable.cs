using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.VFX;

public class Destructable : MonoBehaviour
{
    [Header("Die Animation Settings")]
    [SerializeField] protected float dissolveTimer;
    [SerializeField] protected VisualEffect dieVFXPrefab;

    [Header("Screen Shake Profile")]
    [SerializeField] protected ScreenShakeProfile profile;
    protected CinemachineImpulseSource impulseSource;

    [Space]
    [Header("Other settings")]
    [SerializeField] protected HitType hitType;
    [SerializeField] protected float blinkTime;
    [SerializeField] protected Color hitColor;

    [SerializeField] protected VisualEffect customHitEffect;
    [SerializeField] protected AudioSource customHitSound;

    [SerializeField] protected float healthAmount;

    [SerializeField] protected Transform hitDamageVFXPrefab;

    // Calculés une seule fois pour toute la classe au lieu de rappeler
    // Shader.PropertyToID à chaque Hit/Die (c'était fait à chaque appel avant).
    private static readonly int ShaderHitValue = Shader.PropertyToID("_BlendHitValue");
    private static readonly int ShaderScaleValue = Shader.PropertyToID("_Scale");

    [SerializeField] protected SpriteRenderer spriteRenderer;
    protected Material hitMaterial;

    protected float maxHealth;
    protected bool canBeHit = true;

    protected virtual void Awake()
    {
        maxHealth = healthAmount;
        hitMaterial = spriteRenderer != null ? spriteRenderer.material : null;
        TryGetComponent(out impulseSource);

        if (hitType == HitType.Color && hitMaterial != null)
        {
            hitMaterial.SetColor("_HitColor", hitColor);
        }
    }

    public float GetHealthAmount() => healthAmount;
    public float GetMaxHealth() => maxHealth;
    public bool IsDead() => healthAmount <= 0f;

    // Point d'entrée commun à tous les Destructable (joueur ou ennemi).
    // Les sous-classes ne touchent pas à cette méthode : elles redéfinissent
    // les hooks OnDamaged / OnDeath ci-dessous pour ajouter leur propre logique.
    public void Hit(float damageAmount, Vector2 hitPosition)
    {
        if (!canBeHit) return;

        healthAmount -= damageAmount;
        canBeHit = false;

        OnDamaged(damageAmount, hitPosition);

        if (impulseSource != null)
            ApplyShakeEffect();

        ShowHitEffects();

        // hitPosition == negativeInfinity sert de sentinelle "pas de position
        // fournie" : le test était inversé dans la version originale (== au
        // lieu de !=), ce qui cachait les effets visuels dès qu'une vraie
        // position était donnée.
        if (hitPosition != Vector2.negativeInfinity)
            ShowHitVisuals(hitPosition);

        if (IsDead())
        {
            OnDeath();
            StartCoroutine(Die());
        }
    }

    // Hook : appelé à chaque hit, avant les effets visuels/sonores.
    // PlayerDestructable l'utilise pour mettre à jour l'UI de vie.
    protected virtual void OnDamaged(float damageAmount, Vector2 hitPosition) { }

    // Hook : appelé une seule fois, quand la santé passe à 0 (avant la
    // coroutine de dissolution). Utile pour du loot, un game over, etc.
    protected virtual void OnDeath() { }

    private void ApplyShakeEffect()
    {
        CameraShakeTrigger.Instance.ScreenShakeFromProfile(profile, impulseSource);
    }

    protected virtual IEnumerator Die()
    {
        float currentTime = 0f;

        if (dieVFXPrefab != null)
            Instantiate(dieVFXPrefab, transform.position, Quaternion.identity);

        LeanTween.scaleX(gameObject, 0f, dissolveTimer);

        while (currentTime < dissolveTimer)
        {
            currentTime += Time.deltaTime;
            float currentScale = Mathf.Lerp(1f, 0f, currentTime / dissolveTimer);

            if (hitMaterial != null && hitMaterial.HasProperty(ShaderScaleValue))
                hitMaterial.SetFloat(ShaderScaleValue, currentScale);

            yield return null;
        }
    }

    private void ShowHitEffects()
    {
        if (customHitSound != null) customHitSound.Play();
        if (customHitEffect != null) customHitEffect.Play();
    }

    private void ShowHitVisuals(Vector2 hitPosition)
    {
        if (hitType == HitType.None) return;

        if (hitDamageVFXPrefab != null)
            Instantiate(hitDamageVFXPrefab, hitPosition, Quaternion.identity);

        if (hitType == HitType.Color)
            StartCoroutine(ShowSprite());
    }

    private IEnumerator ShowSprite()
    {
        if (hitMaterial == null)
        {
            canBeHit = true;
            yield break;
        }

        float duration = blinkTime / 2f; // Environ 3 frames

        float timer = 0f;
        while (timer <= duration)
        {
            timer += Time.deltaTime;
            hitMaterial.SetFloat(ShaderHitValue, Mathf.Lerp(0f, 1f, timer / duration));
            yield return null;
        }
        hitMaterial.SetFloat(ShaderHitValue, 1f);

        timer = 0f;
        while (timer <= duration)
        {
            timer += Time.deltaTime;
            hitMaterial.SetFloat(ShaderHitValue, Mathf.Lerp(1f, 0f, timer / duration));
            yield return null;
        }
        hitMaterial.SetFloat(ShaderHitValue, 0f);

        canBeHit = true;
    }
}

[System.Serializable]
public enum HitType
{
    None,
    Color
}