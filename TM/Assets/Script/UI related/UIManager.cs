using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;


public class UIManager : MonoBehaviour
{
    [SerializeField] private RectTransform pausePanel, mainPanel, mapPanel;
    //모든 씬에서 있을 것으로 예상
    //이 외의 대화 진행이나 이벤트, 상점만의 UI 등은 이 클래스를 상속받아서 활용하는걸로? _02_07_06:42
    //아니면 얘네는 기본UI로 냅두고 새로운 UI에 대한 건 따로 스크립트를 만들어서 그것들로 조립하는 식으로? _02_07_06:48
    //결론 : 전부 구현하고, NULL체크만 하여 오류를 막자
    //어짜피 잘못 사용한 일이 아니라면 NULL인 Panel을 접근하는 메소드를 호출할 일이 없다. _02_07_08:16

    //2022_02_09 - 테스트를 위해서 LevelManager를 받아오는 부분을 생성
    //2022_02_09 - 테스트를 위해서 현재 위치를 알려주는 string인 whereAmI를 생성
    [SerializeField] private LevelManager levelManager;
    private string whereAmI;

    [SerializeField] private SpawnManager spawnManager;
    [SerializeField] private InputManager inputManager;
    [SerializeField] private Player player;

    private Stack<RectTransform> panelStack;
    //UI들은 큼직한 Panel 단위로 묶어서 관리할 것.
    //Main Panel을 제외하고 Input의 제어권을 가진 Panel은 단 하나만 존재할것.
    //하나를 연 상태에서 다른 하나를 또 열면 그 전 상태로 돌아가기 위해 ESC를 사용.
    //위와 같은 규칙을 준수하기 위해 만들어진 스택이라고 볼 수 있다.



    private bool pauseFlag = false;
    private bool attackFlag = true, moveFlag = true;


    public void Start()
    {
        panelStack = new Stack<RectTransform>(8);

        //2022_02_09 - 테스트를 위해 추가한 부분, 나중에 변경할 때 지우셔도 무방한 부분입니다.
        whereAmI = "Room" + levelManager.GetComponent<PlayerSaveManager>().saving.curRoomNumber + " / " + levelManager.GetComponent<LevelManager>().currentScene;
        GameObject.FindWithTag("Text").GetComponent<Text>().text = whereAmI;
        //2022_02_10 - hp바를 맞춰봤습니다.
        mainPanel.Find("hp_bar").GetComponent<Slider>().value = (float)player.getHp() / player.getMaxHp();
        mainPanel.Find("hp_bar").GetComponentInChildren<Text>().text = player.getHp() + "\n/\n" + player.getMaxHp();
    }

    public bool getAttackFlag() { return attackFlag; }
    public bool getMoveFlag() { return moveFlag; }

    public void Escape(InputAction.CallbackContext context)
    {
        if (context.performed) // 이 조건 없으면 세번 호출됨 (started/performed/canceled)
        {
            if (panelStack.Count == 0) //esc 첨 누르세요? 그럼 pause
            {
                Pause();
            }
            else if (panelStack.Count == 1) //이미 pause가 떠잇어요? 그럼 resume
            {
                Resume();
            }
            else //Pause 말고도 뭔가 많이 떠있네요? : 하나씩 닫기
            {
                BackUI();
            }
        }
    }
    public void BackUI()
    {
        panelStack.Pop().gameObject.SetActive(false);
        panelStack.Peek().gameObject.SetActive(true);
    }
    public void Pause() //시간을 멈추고! pauseUI를 띄움
    {
        if (pausePanel != null)
        {
            pauseFlag = true;
            Time.timeScale = 0f;
            // Update는 못막음. Couroutine의 yield return waitforseconds는 막음. FixedUpdate 막음.
            // : player를 직접 조작하는 (InputManager <-> Player) 이벤트를 막을 수 없음.
            // 따라서 pauseFlag를 통해 공격&이동처리를 못하게 함.
            AudioListener.pause = true;
            pausePanel.gameObject.SetActive(true);
            panelStack.Push(pausePanel);
        }
        else { Debug.Log("PauseUI Null"); }
    }
    public void Resume() //시간을 재개하고! PauseUI를 Disable
    {
        if (pausePanel != null)
        {
            pauseFlag = false;
            Time.timeScale = 1f;
            AudioListener.pause = false;
            pausePanel.gameObject.SetActive(false);
            panelStack.Pop();
        }
        else { Debug.Log("PauseUI Null"); }
    }
    public void MapActive() //맵 UI 전체를 띄움
    {
        if (mapPanel != null)
        {
            mapPanel.gameObject.SetActive(true);
            panelStack.Peek().gameObject.SetActive(false);
            panelStack.Push(mapPanel);
        }
        else { Debug.Log("MapUI Null"); }
    }
    public void panel_AccessCheck_Active(RectTransform rectTransform)  //2022_02_12_08:37 추가 : 다음 맵 고를 때 확인해주는 UIPanel. 굳이 이걸 UIManager에 해야 됐나 싶지만 esc로 끌 수 있으면 편할 것 같아서 했다
    {
        //매개변수 rectTransform은 MapUIBtn(임시) 스크립트에서 이벤트 실행 주체(맵 버튼)를 보내면 그 좌표 옆에 panel_AccessCheck(확인/취소)를 띄운다
        if (rectTransform != null)
        {
            RectTransform panel_AccessCheck = rectTransform.transform.parent.Find("Panel_AccessCheck").GetComponent<RectTransform>();
            if (panelStack.Peek().Equals(panel_AccessCheck)) //이미 열려있는 상태에서 다른 레벨을 골랐을 때 panelStack에 같은 오브젝트가 두번 쌓이는 걸 방지
            {
                panel_AccessCheck.SetAsLastSibling();
                panel_AccessCheck.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x + rectTransform.sizeDelta.x, rectTransform.anchoredPosition.y);
            }
            else
            {
                panel_AccessCheck.SetAsLastSibling();
                panel_AccessCheck.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x + rectTransform.sizeDelta.x, rectTransform.anchoredPosition.y);
                panel_AccessCheck.gameObject.SetActive(true);
                panelStack.Push(panel_AccessCheck);
            }
        }
        else { Debug.Log("MapUIButton Null"); }
    }

    public void ItemSelect_key(InputAction.CallbackContext context)
    {
        if (context.performed && inputManager.isItemSelect('0') && !pauseFlag)
        {
            RectTransform img_item_sel = mainPanel.Find("panel_main").Find("Img_item_sel").GetComponent<RectTransform>();
            if (inputManager.isItemSelect('1'))
            {
                img_item_sel.anchoredPosition = mainPanel.Find("panel_main").Find("Img_item1").GetComponent<RectTransform>().anchoredPosition;
            }
            else if (inputManager.isItemSelect('2'))
            {
                img_item_sel.anchoredPosition = mainPanel.Find("panel_main").Find("Img_item2").GetComponent<RectTransform>().anchoredPosition;
            }
            else if (inputManager.isItemSelect('3'))
            {
                img_item_sel.anchoredPosition = mainPanel.Find("panel_main").Find("Img_item3").GetComponent<RectTransform>().anchoredPosition;
            }
        }
    }

    public void Attack(InputAction.CallbackContext context)
    {
        if (inputManager.isAttack() && !pauseFlag)
        {
            if (getAttackFlag())
            {
                player.attack();
                StartCoroutine(AttackCooltime());
            }
            else { Debug.Log("공격쿨"); }
        }
    }
    public void Move(InputAction.CallbackContext context)
    {
        if (inputManager.isMove('a') && !pauseFlag)
        {
            if (getMoveFlag())
            {
                Vector2 direction;
                if (inputManager.isMove('u'))
                {
                    direction = new Vector2(0, 1);
                }else if (inputManager.isMove('d'))
                {
                    direction = new Vector2(0, -1);
                }
                else if(inputManager.isMove('l'))
                {
                    direction = new Vector2(-1,0);
                }
                else if(inputManager.isMove('r'))
                {
                    direction = new Vector2(1,0);
                }
                else
                {
                    direction = new Vector2(0, 0);
                }
                    player.move(direction);
                StartCoroutine(MoveCooltime());
            }
            else { Debug.Log("이동쿨"); }
        }
    }

    public IEnumerator AttackCooltime() // 공격이 성공했을 시 쿨타임 돌리기 -> 다시 공격 호출(키를 누르고 있는 경우 연속적으로)
    {
        if (mainPanel != null)
        {
            attackFlag = false;
            Image img_attack_cooltime = mainPanel.Find("panel_main").Find("Img_attack_clt").GetComponent<Image>();
            float clt = 10f/player.getAttackSpd();
            img_attack_cooltime.fillAmount = 1;
            for (int i = 0; i < clt*100; i++)
            {
                img_attack_cooltime.fillAmount = 1 - i / (clt*100);
                yield return new WaitForSeconds(0.01f);
            }
            img_attack_cooltime.fillAmount = 0;
            attackFlag = true;
            Attack(new InputAction.CallbackContext());
        }
    }
    public IEnumerator MoveCooltime() // 이동이 성공했을 시 ~~
    {
        if (mainPanel != null)
        {
            moveFlag = false;
            Image img_move_cooltime= mainPanel.Find("panel_main").Find("Img_move_clt").GetComponent<Image>();
            float clt = 10f/player.getMoveSpd();
            img_move_cooltime.fillAmount = 1;
            for (int i = 0; i < clt*100; i++)
            {
                img_move_cooltime.fillAmount = 1 - i / (clt*100);
                yield return new WaitForSeconds(0.01f);
            }
            img_move_cooltime.fillAmount = 0;
            moveFlag = true;
            Move(new InputAction.CallbackContext());
        }
    }
}
