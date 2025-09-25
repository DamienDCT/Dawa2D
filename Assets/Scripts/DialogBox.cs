using System.Collections;
using TMPro;
using UnityEngine;

public class DialogBox : MonoBehaviour
{
    public static DialogBox Instance;


    [SerializeField] private TextMeshProUGUI textToDisplay;

    private Coroutine currentDisplayCoroutine;

    private void Awake()
    {
        Instance = this;
    }

    public void ShowDialog(string text)
    {
        if(currentDisplayCoroutine != null)
        {
            currentDisplayCoroutine = null;
            textToDisplay.text = "";
        }

        currentDisplayCoroutine = StartCoroutine(DialogAnimation(text));
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            ShowDialog("Quare hoc quidem praeceptum, cuiuscumque est, ad tollendam amicitiam valet; illud potius praecipiendum fuit, ut eam diligentiam adhiberemus in amicitiis comparandis, ut ne quando amare inciperemus eum, quem aliquando odisse possemus. Quin etiam si minus felices in diligendo fuissemus, ferendum id Scipio potius quam inimicitiarum tempus cogitandum putabat.");
        }
    }

    private IEnumerator DialogAnimation(string text)
    {
        char[] characters = text.ToCharArray();

        for(int i = 0; i < characters.Length; i++)
        {
            textToDisplay.text = textToDisplay.text + characters[i];
            yield return null;
        }
    }
}
