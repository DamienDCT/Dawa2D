using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogBox : MonoBehaviour
{
    public static DialogBox Instance;

    [SerializeField] private TextMeshProUGUI textToDisplay;
    [SerializeField] private Transform choiceContainer;
    [SerializeField] private Button choiceButtonPrefab;

    [SerializeField] private DialogNode startNode;

    private DialogNode currentNode;
    private bool waitingForNextBubble = false;
    private bool hasStartedDialog = false;

    private void Awake() => Instance = this;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.U))
        {
            if (hasStartedDialog)
                return;
            StartDialog(startNode);
        }
    }

    public async void StartDialog(DialogNode startNode)
    {
        hasStartedDialog = true;
        currentNode = startNode;

        // On récupère la traduction avant d'entrer dans la coroutine
        string localizedText = await LocalizationManager.Instance.GetTranslatedText(
            startNode.key,
            startNode.tableId
        );

        StartCoroutine(PlayNode(startNode, localizedText));
    }

    private IEnumerator PlayNode(DialogNode node, string localizedText)
    {
        string[] bubbles = localizedText.Split(new string[] { "<split>" }, System.StringSplitOptions.None);

        foreach (string bubble in bubbles)
        {
            textToDisplay.text = "";
            yield return StartCoroutine(TypeText(bubble.Trim()));

            waitingForNextBubble = true;
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E));
            waitingForNextBubble = false;
        }

        if (node.hasChoices)
        {
            ShowChoices(node);
        }
        else if (node.nextNodeIfNoChoice != null)
        {
            yield return new WaitForSeconds(0.2f);
            StartDialog(node.nextNodeIfNoChoice); // et pas StartCoroutine !
        }
        else
        {
            HideDialog();
        }
    }

    private IEnumerator TypeText(string text)
    {
        foreach (char c in text)
        {
            textToDisplay.text += c;
            yield return new WaitForSeconds(0.02f);
        }
    }

    private async void ShowChoices(DialogNode node)
    {
        ClearChoices();

        foreach (var choice in node.choices)
        {
            Debug.Log(choice);
            var btn = Instantiate(choiceButtonPrefab, choiceContainer);
            string choiceText = await LocalizationManager.Instance.GetTranslatedText(choice.key, node.tableId);
            btn.GetComponentInChildren<TextMeshProUGUI>().text = choiceText;

            btn.onClick.AddListener(() =>
            {
                ClearChoices();
                choice.actionIfSelected?.PerformAction();

                if (choice.nextNode != null)
                    StartDialog(choice.nextNode);
                else
                    HideDialog();
            });
        }
    }

    private void ClearChoices()
    {
        foreach (Transform child in choiceContainer)
            Destroy(child.gameObject);
    }

    private void HideDialog()
    {
        textToDisplay.text = "";
        ClearChoices();
        currentNode = null;
        hasStartedDialog = false;
    }
}
