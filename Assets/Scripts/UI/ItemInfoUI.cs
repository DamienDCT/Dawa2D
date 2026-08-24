using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemInfoUI : MonoBehaviour
{
    public static ItemInfoUI Instance;

    [Header("Localization")]
    [SerializeField] private string itemTableLocaliaztionName;


    [Header("Display References")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Image itemSprite;

    private void Awake()
    {
        Instance = this;
    }

    public async void UpdateInformations(ItemInfo itemInfo)
    {
        if(!string.IsNullOrEmpty(itemInfo.itemNameLocalization))
        {
            string title = await LocalizationManager.Instance.GetTranslatedText(itemInfo.itemNameLocalization, itemTableLocaliaztionName);
            titleText.text = title;
        }

        if (!string.IsNullOrEmpty(itemInfo.itemDescriptionLocalization))
        {
            string description = await LocalizationManager.Instance.GetTranslatedText(itemInfo.itemDescriptionLocalization, itemTableLocaliaztionName);
            descriptionText.text = description;
        }

        if(itemInfo.itemSprite != null)
        {
            itemSprite.sprite = itemInfo.itemSprite;
        }
    }

}
