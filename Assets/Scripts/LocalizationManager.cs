using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using System.Collections.Generic;
using UnityEditor.Localization;
using System.Threading.Tasks;

public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager Instance;

    [SerializeField] private List<LocalizationData> localizationDatabase;

    [System.Serializable]
    public struct LocalizationData
    {
        public string tableReferenceID;
        public StringTable table;
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public async Task<string> GetTranslatedText(string referenceId, string tableId = "Dialogues")
    {
        // Charge la table localisée selon la langue active actuelle
        var tableLoading = LocalizationSettings.StringDatabase.GetTableAsync(tableId);
        await tableLoading.Task;

        var table = tableLoading.Result;
        if (table == null)
        {
            Debug.LogWarning($"Table '{tableId}' introuvable !");
            return $"[{referenceId}]";
        }

        var entry = table.GetEntry(referenceId);
        if (entry == null)
        {
            Debug.LogWarning($"Entrée '{referenceId}' introuvable dans la table '{tableId}' !");
            return $"[{referenceId}]";
        }

        return entry.GetLocalizedString();
    }
}
