using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartStoryUI : MonoBehaviour
{
    /*
     * 2022_0224
     */

    [SerializeField] private List<RectTransform> backgrounds; //���丮 �����̵�.
    [SerializeField] private RectTransform newGamePanel, skipBtn;
    [SerializeField] private LevelManager levelManager;
    private int i; //���̵�� ����
    private int index=0;//���� ��� �����̵�
    public void StartButtonClicked() //�������� ������ �ٽ� ����� ������ ���丮 �ѱ�
    {
        if (levelManager.GetComponent<SaveManager>().saving.roomType != "Title")
        {
            newGamePanel.gameObject.SetActive(true);
        }
        else
        {
            skipBtn.gameObject.SetActive(true);
            StartCoroutine(BackGroundFadeChange());
        }
    }
    public void StartButtonConfirmed() //���� ���� ����ᵵ �Ǵ��� ���� �� �� Ȯ�� ��ư ������ ��
    {
        newGamePanel.gameObject.SetActive(false);
        skipBtn.gameObject.SetActive(true);
        StartCoroutine(BackGroundFadeChange());
    }
    public void SkipStory() //���丮 ��ŵ
    {
        SwitchScene.loadNewGame();
    }
    public void ImageClicked() //��⺯ȭ�� ��� Ŭ���ϸ� ȿ�� ��ŵ ����, ȿ�� ���� �� Ŭ���ϸ� ���� �����̵�
    {
        if (i >= 100)
        {
            if (index < backgrounds.Count) StartCoroutine(BackGroundFadeChange());
            else SwitchScene.loadNewGame(); 
        }
        else
        {
            Debug.Log(index + " " + backgrounds.Count);
            i = 99;
        }
    }
    IEnumerator BackGroundFadeChange() //��⺯ȭ �� ���丮 �����̵�(?) �ѱ��
    {
        RectTransform prev, cur;
        if (index > -1)
        {
            prev = backgrounds[index];
            prev.transform.SetAsLastSibling();
        }
        cur = backgrounds[index++];
        cur.transform.SetAsLastSibling();
        cur.gameObject.SetActive(true);
        Color c = new Color(0f, 0f, 0f, 0f);
        Color tc = new Color(1f, 1f, 1f, 0f);
        for (i = 0; i <= 100; i++)
        {
            tc.a = i / 100f;
            c.a = i / 100f;
            cur.gameObject.GetComponent<Image>().color = c;
            cur.GetChild(0).GetComponent<Text>().color = tc;
            yield return new WaitForSeconds(0.01f);
        }
    }
}
