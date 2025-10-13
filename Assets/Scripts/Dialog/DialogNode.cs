using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "NewDialogNode", menuName = "Dialog/Node")]
public class DialogNode : ScriptableObject
{
    public string tableId = "Dialogues";
    public string key; 

    public bool hasChoices;
    public List<DialogChoice> choices;

    public DialogNode nextNodeIfNoChoice; 
}

[System.Serializable]
public class DialogChoice
{
    public string key;                     
    public DialogNode nextNode;
    public DialogChoiceAction actionIfSelected;

}