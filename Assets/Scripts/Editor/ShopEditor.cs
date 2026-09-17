using UnityEditor;

[CustomEditor(typeof(Shop))]
public class ShopEditor : Editor
{
    SerializedProperty useDialogBeforeProp;
    SerializedProperty dialogNodeProp;

    private void OnEnable()
    {
        useDialogBeforeProp = serializedObject.FindProperty("hasDialogueBeforeShopOpens");
        dialogNodeProp      = serializedObject.FindProperty("startDialogNode");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawPropertiesExcluding(serializedObject, "hasDialogueBeforeShopOpens", "startDialogNode");

        EditorGUILayout.PropertyField(useDialogBeforeProp);

        if (useDialogBeforeProp.boolValue)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(dialogNodeProp);

            EditorGUI.indentLevel--;
        }



        // On applique les modifications (sauvegarde quand on change une valeur)
        serializedObject.ApplyModifiedProperties();
    }
}
