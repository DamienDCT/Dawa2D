using UnityEngine;
using UnityEngine.UI;

public class PlayerManaUI : MonoBehaviour
{
    [SerializeField] private Image fillImage;

    public void UpdateUI(float amount)
    {
        float target = Mathf.Clamp01(amount); // toujours entre 0 et 1

        // Stoppe un tween précédent si besoin (évite les doublons)
        LeanTween.cancel(fillImage.gameObject);

        // Lance le tween
        LeanTween.value(
            fillImage.gameObject,
            fillImage.fillAmount,
            target,
            0.3f // durée
        )
        .setEaseOutQuint()
        .setOnUpdate((float value) =>
        {
            fillImage.fillAmount = value;
        });
    }
}
