using UnityEngine;
using UnityEditor; // Attention, c'est indispensable pour l'éditeur

// On indique à Unity que ce script modifie l'inspecteur de "HitEffect"
[CustomEditor(typeof(Destructable))]
public class DestructableEditor : Editor
{
    // On déclare des propriétés sérialisées pour lier les variables
    SerializedProperty hitTypeProp;
    SerializedProperty hitColorProp;
    SerializedProperty blinkTimeProp;

    void OnEnable()
    {
        // On connecte les variables de l'éditeur à celles du script HitEffect
        hitTypeProp = serializedObject.FindProperty("hitType");
        hitColorProp = serializedObject.FindProperty("hitColor");
        blinkTimeProp = serializedObject.FindProperty("blinkTime");
    }

    public override void OnInspectorGUI()
    {
      //  base.OnInspectorGUI();
        // On met à jour l'objet pour récupérer ses dernières valeurs
        serializedObject.Update();

        // 1. On affiche le menu déroulant (l'Enum)
        EditorGUILayout.PropertyField(hitTypeProp);

        // 2. On vérifie si la valeur sélectionnée est "Color"
        // L'index 1 correspond à "Color" dans notre enum (0=None, 1=Color, 2=OtherEffect)
        if (hitTypeProp.enumValueIndex == (int)HitType.Color)
        {
            // On ajoute une petite tabulation pour faire plus joli
            EditorGUI.indentLevel++;

            // On affiche les variables conditionnelles
            EditorGUILayout.PropertyField(hitColorProp);
            EditorGUILayout.PropertyField(blinkTimeProp);

            // On retire la tabulation
            EditorGUI.indentLevel--;
        }
        DrawPropertiesExcluding(serializedObject, "m_Script", "hitType", "hitColor", "blinkTime");
        // On applique les modifications (sauvegarde quand on change une valeur)
        serializedObject.ApplyModifiedProperties();
    }
}