using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SpellCircleSlot : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image iconImage;
    [SerializeField] private Sprite transparentSprite;
    [SerializeField] private Selectable selectable;

    private ItemSO currentSpell;
    private SpellCircleUI parentUI;
    private bool isEnabled = false;

    public void Initialize(SpellCircleUI circleUI)
    {
        parentUI = circleUI;
    }

    public void EnableSlotForSelection(SpellCircleUI parent)
    {
        isEnabled = true;
        parentUI = parent;
        // optionnel : changer la couleur / surbrillance
        iconImage.color = Color.yellow;
    }

    public void DisableSlot()
    {
        isEnabled = false;
        iconImage.color = Color.white;
    }

    public void SetSpell(ItemSO spell)
    {
        currentSpell = spell;
        iconImage.sprite = spell != null ? spell.sprite : transparentSprite;
    //    selectable.enabled = spell != null;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("coucou");
        if (!isEnabled)
        {

            if(currentSpell != null)
            {
                SetSpell(null);
            } else
            {
                parentUI.StartSelectionOnInventory(this);
            }
            return;
        }

        parentUI.AssignToSlot(this);
    }

    public ItemSO GetCurrentSpell()
    {
        return this.currentSpell;
    }
}