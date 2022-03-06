using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SmallUIManager : MonoBehaviour
{
    //22_03_02 audioManager Ãß°¡
    [SerializeField] private AudioManager audioManager;

    [SerializeField] private LevelManager levelManager;
    [SerializeField] private InputManager inputManager;

    [SerializeField] private RectTransform dialogPanel;
    //2022_02_28 
    [SerializeField] private List<string> dialog;

    [SerializeField] private Image img;
    [SerializeField] private GameObject background;
    [SerializeField] private GameObject inst;
    [SerializeField] private Text t;

    private IEnumerator NewDialogEnumerator;

    private Stack<RectTransform> panelStack;

    public void Start()
    {
        panelStack = new Stack<RectTransform>(8);
        Time.timeScale = 1f;
        NewDialogEnumerator = DialogProgress();
        NewDialogEnumerator.MoveNext();
    }

    public IEnumerator DialogProgress()
    {
        int index = 0;
        panelStack.Push(dialogPanel);
        dialogPanel.gameObject.SetActive(true);
        while (index < dialog.Count)
        {
            string[] tempStr;
            tempStr = dialog[index].Split('_');
            foreach (Image r in dialogPanel.Find("Background").Find("Img_Character").GetComponentsInChildren<Image>())
            {
                if (r.gameObject.name.Equals(tempStr[0])) r.enabled = true;
                else r.enabled = false;
            }
            dialogPanel.Find("Background").Find("Img_Name").Find("Text").GetComponent<Text>().text = tempStr[0];
            dialogPanel.Find("Background").Find("Text").GetComponent<Text>().text = tempStr[1];
            yield return index;
            index++;
        }
        dialogPanel.gameObject.SetActive(false);
        StartCoroutine(fadeInImg());
    }

    IEnumerator fadeInImg()
    {
        float fadeCount = 0f;
        while (fadeCount < 1.0f)
        {
            fadeCount += 0.003f;
            yield return new WaitForSeconds(0.01f);
            img.color = new Color(1f, 1f, 1f, fadeCount);
        }
        fadeCount = 0f;
        while (fadeCount < 1.0f)
        {
            fadeCount += 0.003f;
            yield return new WaitForSeconds(0.01f);
            t.color = new Color(1f, 1f, 1f, fadeCount);
        }
        background.SetActive(true);
        background.GetComponent<Button>().enabled = true;
        inst.SetActive(true);
    }

    public void DialogNext()
    {
        NewDialogEnumerator.MoveNext();
    }
}
