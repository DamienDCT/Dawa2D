using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogBoxUITK : BasedUITK
{
    // Singleton
    public static DialogBoxUITK Instance;

    [SerializeField] private VisualTreeAsset _textChoiceAsset;
    private PanelRenderer panelRenderer;
    [SerializeField] private string itemTableLocalizationName;

    private DialogNode startNode;

    private DialogNode currentNode;
    private bool waitingForNextBubble = false;
    private bool hasStartedDialog = false;

    // Current version of the UI displayed
    private int _uiVersion;
    // Variables for the UI Toolkit layout
    private VisualElement root;
    private Label dialogLabel;
    private VisualElement choiceContainer;
    private VisualElement[] choicesElements;


    // Current choice selected
    private VisualElement currentChoiceSelected;
    // List of choices
    private DialogChoice[] choices;

    private void Awake()
    {
        Instance = this;

        panelRenderer = GetComponent<PanelRenderer>();
        panelRenderer.RegisterUIReloadCallback(OnUIReload);

        _uiVersion = -1;
        currentChoiceSelected = null;
    }

    private void OnDestroy()
    {
        panelRenderer.UnregisterUIReloadCallback(OnUIReload);
    }

    private void OnUIReload(PanelRenderer panelRenderer, VisualElement rootElement, int version)
    {
        // Si la version est identique, on évite de reload les valeurs
        if (_uiVersion == version)
            return;

        currentChoiceSelected = null;
        root = rootElement;

        menuElement = root.Q<VisualElement>("Root");

        dialogLabel = menuElement.Q<Label>("DialogLabel");

        // Element where the choices are stored
        choiceContainer = menuElement.Q<VisualElement>("ChoiceContent");
    }

    public async void StartDialog(DialogNode startNode)
    {
        // Open menu if not opened
        Open();

        hasStartedDialog = true;
        currentNode = startNode;

        // On récupère la traduction avant d'entrer dans la coroutine
        string localizedText = await LocalizationManager.Instance.GetTranslatedText(
            startNode.key,
            startNode.tableId
        );

        // Adding class hidden to not show in case of, the choice menu.
        choiceContainer.AddToClassList("hidden");

        StartCoroutine(PlayNode(startNode, localizedText));
    }

    private IEnumerator PlayNode(DialogNode node, string localizedText)
    {
        string[] bubbles = localizedText.Split(new string[] { "<split>" }, System.StringSplitOptions.None);

        foreach (string bubble in bubbles)
        {
            dialogLabel.text = "";
            yield return StartCoroutine(TypeText(bubble.Trim()));

            waitingForNextBubble = true;
            if(!node.hasChoices)
                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E));

            waitingForNextBubble = false;
        }

        //Debug.Log(node.hasChoices);
        //Debug.Log(node.choices.Count);
        if (node.hasChoices)
        {
            ShowChoices(node);
        }
        else if (node.nextNodeIfNoChoice != null)
        {
            yield return new WaitForSeconds(0.2f);
            StartDialog(node.nextNodeIfNoChoice);
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
            dialogLabel.text += c;
            yield return new WaitForSeconds(0.02f);
        }
    }

    private async void ShowChoices(DialogNode node)
    {
        ClearChoices();

        choices = new DialogChoice[node.choices.Count];
        choicesElements = new VisualElement[node.choices.Count];

        int i = 0;
        foreach (var choice in node.choices)
        {
            //Debug.Log(choice);
            //var btn = Instantiate(choiceButtonPrefab, choiceContainer);
            string choiceText = await LocalizationManager.Instance.GetTranslatedText(choice.key, node.tableId);
            //btn.GetComponentInChildren<TextMeshProUGUI>().text = choiceText;

            //btn.onClick.AddListener(() =>
            //{
            //    ClearChoices();
            //    choice.actionIfSelected?.PerformAction();

            //    if (choice.nextNode != null)
            //        StartDialog(choice.nextNode);
            //    else
            //        HideDialog();
            //});

            choices[i] = choice;
            choicesElements[i] = SetupChoice(choiceText, choice);
            // Add the choice
            choiceContainer?.Add(choicesElements[i]);
            i++;
        }

        // Select the first choice
        if(choicesElements.Length > 0)
            choicesElements[0].Focus();
    }

    private VisualElement SetupChoice(string choiceText, DialogChoice choice)
    {
        TemplateContainer slotInstance = _textChoiceAsset.Instantiate();
        Label choiceLabel = slotInstance.Q<Label>("SelectedChoice");
        choiceLabel.text = choiceText;

        slotInstance.focusable = true;
        slotInstance.tabIndex = 0;

        // Curseur visuel (souris OU clavier/manette, peu importe la source)
        slotInstance.RegisterCallback<FocusInEvent>(evt =>
        {
            //UpdateDetailPanel(item);
            slotInstance.AddToClassList("selected-choice-text");
        });
        slotInstance.RegisterCallback<FocusOutEvent>(evt =>
            slotInstance.RemoveFromClassList("selected-choice-text"));

        // Activation : clic souris OU bouton Submit manette/clavier
        //slotInstance.RegisterCallback<ClickEvent>(evt => ActivateInventorySlot(item, slotInstance));
        slotInstance.RegisterCallback<NavigationSubmitEvent>(evt =>
        {
            evt.StopPropagation();
            slotInstance.Blur();

            var actionToPerform = choice.actionIfSelected;
            var next = choice.nextNode;

            ClearChoices();
            actionToPerform?.PerformAction(); // fait Close() du dialog, Open() du shop -> maintenant sûr

            if (next != null)
                StartDialog(next);
            else
                HideDialog();
        });

        return slotInstance;
    }

    private void ClearChoices()
    {
        // TODO : Destroy elements on UI before cleaning
        choiceContainer?.Clear();
        choiceContainer.RemoveFromClassList("hidden");


        choices = null;
        choicesElements = null;
    }

    private void HideDialog()
    {
        dialogLabel.text = "";
        ClearChoices();
        currentNode = null;
        hasStartedDialog = false;
    }

    //protected override void OnMenuClosed()
    //{
    //    ClearChoices();
    //}

    //protected override void OnMenuOpened()
    //{
    //    ClearChoices();
    //}
}
