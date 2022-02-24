using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartStoryUI : MonoBehaviour
{
    /*
     * 2022_0224
     */

    [SerializeField] private List<RectTransform> backgrounds; //스토리 슬라이드.
    [SerializeField] private RectTransform newGamePanel, skipBtn;
    [SerializeField] private LevelManager levelManager;
    private int i; //페이드용 변수
    private int index=0;//현재 몇번 슬라이드
    public void StartButtonClicked() //저장파일 있으면 다시 물어보고 없으면 스토리 켜기
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
    public void StartButtonConfirmed() //정말 새로 덮어써도 되는지 질문 한 후 확인 버튼 눌렀을 시
    {
        newGamePanel.gameObject.SetActive(false);
        skipBtn.gameObject.SetActive(true);
        StartCoroutine(BackGroundFadeChange());
    }
    public void SkipStory() //스토리 스킵
    {
        SwitchScene.loadNewGame();
    }
    public void ImageClicked() //밝기변화중 배경 클릭하면 효과 스킵 가능, 효과 끝난 후 클릭하면 다음 슬라이드
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
    IEnumerator BackGroundFadeChange() //밝기변화 및 스토리 슬라이드(?) 넘기기
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
