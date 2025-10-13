using TMPro;
using UnityEngine;

public class PlayerPhaseManager : MonoBehaviour
{
    [SerializeField] private PlayerPhase currentPlayerPhase;
    [SerializeField] private TextMeshProUGUI phaseText;

    private void Start()
    {
        currentPlayerPhase = PlayerPhase.ARCANIQUE;
    }

    public void ChangePhase(PlayerPhase newPhase)
    {
        phaseText.text = newPhase.ToString();
        this.currentPlayerPhase = newPhase;
    }

    public PlayerPhase GetCurrentPhase()
    {
        return this.currentPlayerPhase;
    }
}


[System.Serializable]
public enum PlayerPhase
{
    ARCANIQUE,
    PALADIN,
    ECLAIREUR,
}
