using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InputIconDatabase", menuName = "Input/IconDatabase")]
public class InputIconDatabase : ScriptableObject
{
    [System.Serializable]
    public class IconEntry
    {
        public string controlPath; // Exemple: "Keyboard/e", "Keyboard/space", "Gamepad/buttonSouth"
        public Sprite icon;
    }

    [SerializeField] private List<IconEntry> icons = new();

    public Sprite GetIconForControlPath(string controlPath)
    {
        if (string.IsNullOrEmpty(controlPath))
            return null;

        // 1. cherche correspondance exacte
        foreach (var entry in icons)
        {
            if (controlPath.Equals(entry.controlPath, System.StringComparison.OrdinalIgnoreCase))
                return entry.icon;
        }

        // 2. essaie une correspondance générique (ex: Keyboard/space → Key/space)
        if (controlPath.StartsWith("Keyboard/"))
        {
            string keyPart = controlPath.Replace("Keyboard/", "Key/");
            foreach (var entry in icons)
            {
                if (keyPart.Equals(entry.controlPath, System.StringComparison.OrdinalIgnoreCase))
                    return entry.icon;
            }
        }

        return null;
    }
}
