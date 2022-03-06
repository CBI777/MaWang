using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;


public class UIManager : MonoBehaviour
{
    //22_03_02 audioManager 추가
    [SerializeField] private AudioManager audioManager;
    //22_03_01 gameOver를 위한 패널의 추가
    [SerializeField] private RectTransform gameOverPanel;
    //22_03_01 levelClear를 위한 패널의 추가
    [SerializeField] private RectTransform levelClearPanel;
    [SerializeField] private RectTransform pausePanel, mainPanel, mapPanel, dialogPanel, settingPanel;
    //모든 씬에서 있을 것으로 예상
    //이 외의 대화 진행이나 이벤트, 상점만의 UI 등은 이 클래스를 상속받아서 활용하는걸로? _02_07_06:42
    //아니면 얘네는 기본UI로 냅두고 새로운 UI에 대한 건 따로 스크립트를 만들어서 그것들로 조립하는 식으로? _02_07_06:48
    //전부 구현하고, NULL체크만 하여 오류를 막자
    //어짜피 잘못 사용한 일이 아니라면 NULL인 Panel을 접근하는 메소드를 호출할 일이 없다. _02_07_08:16

    //2022_02_09 - 테스트를 위해서 LevelManager를 받아오는 부분을 생성
    //2022_02_09 - 테스트를 위해서 현재 위치를 알려주는 string인 whereAmI를 생성
    [SerializeField] private LevelManager levelManager;
    private string whereAmI;
    [SerializeField] private InputManager inputManager;
    [SerializeField] private park.MapUI mapUI;
    [SerializeField] private Player player;

    //2022_02_28 
    [SerializeField] private List<string> dialog;
    private IEnumerator NewDialogEnumerator;

    private Stack<RectTransform> panelStack;
    //UI들은 큼직한 Panel 단위로 묶어서 관리할 것.
    //Main Panel을 제외하고 Input의 제어권을 가진 Panel은 단 하나만 존재할것.
    //하나를 연 상태에서 다른 하나를 또 열면 그 전 상태로 돌아가기 위해 ESC를 사용.
    //위와 같은 규칙을 준수하기 위해 만들어진 스택이라고 볼 수 있다.

    private bool pauseFlag = false;
    private bool attackFlag = true, moveFlag = true;
    //22_03_01 게임오버를 위한 flag 추가
    private bool gameOverFlag = false;
    //22_03_01 보상을 위한 flag 추가
    private bool clearFlag = false;
    //22_03_02
    private bool dialogFlag = false;

    //22_03_01 타이밍 문제로 awake추가
    public void Awake()
    {
        panelStack = new Stack<RectTransform>(8);
        Time.timeScale = 1f; // 2022_02_20 씬 전환 전 paused 상태에서 진행하면 시간이 멈춰서 씬 시작시 자동으로 풀어주는 역할
    }

    public void Start()
    {

        //2022_02_11 - hp바 부분을 함수로 변경
        changeHpBar();
        //2022_02_14
        changeArtifact(1, player.getArtifact(0).getRealArtifactName());
        changeArtifact(2, player.getArtifact(1).getRealArtifactName());
        changeArtifact(3, player.getArtifact(2).getRealArtifactName());
        //22_03_01
        changeATKSPD(player.getAttackSpd());
        changeMOVSPD(player.getMoveSpd());
        changeSTR(player.getStr());
        changeGold(player.getGold());
        //2022_02_16
        //mapUI.debugMapInfos();
        mapUI.MapDraw();
        //22_03_01
        if (levelManager.GetComponent<SaveManager>().saving.stageFlag)
        {
            displayClearAward(levelManager.GetComponent<SaveManager>().saving.stageVar3);
        }

        //2022_02_28
        if (dialogPanel != null)
        {
            dialogFlag = true;
            pauseFlag = true;
            Time.timeScale = 0f;
            NewDialogEnumerator = DialogProgress();
            NewDialogEnumerator.MoveNext();
        }
    }

    public bool getAttackFlag() { return attackFlag; }
    public bool getMoveFlag() { return moveFlag; }

    public void Escape(InputAction.CallbackContext context)
    {
        //22_03_01 게임오버가 되면 ESC도 안먹히게 하고 싶다.
        //그래서 gameoverflag도 검사를 하도록 바꾸었다.
        //또한, clear화면과 stack의 문제점을 해결하려고 clearFlag의 검사를 추가하였다.
        if (context.performed && !gameOverFlag)
        {
            if (clearFlag || dialogFlag)
            {
                if (panelStack.Count == 1)
                {
                    Pause();
                }
                else
                {
                    BackUI();
                }
            }
            else
            {
                if (panelStack.Count == 0)
                {
                    Pause();
                }
                else if (panelStack.Count == 1)
                {
                    Resume();
                }
                else
                {
                    BackUI();
                }
            }
        }
    }
    public void BackUI()
    {
        //03_03
        panelStack.Pop().gameObject.SetActive(false);
        if (panelStack.Count > 0) panelStack.Peek().gameObject.SetActive(true);
    }

    //22_03_01 게임오버를 위한 함수
    public void gameOver()
    {
        //22_03_03 gameOver 배경음 변경 추가
        audioManager.ChangeMusicWithoutDelay("GameOver");
        //gameOverFlag가 true로 바뀌었어도, player의 기본적인 움직임을 막는것은 pauseFlag.
        gameOverFlag = true;
        pauseFlag = true;
        Time.timeScale = 0f;
        gameOverPanel.gameObject.SetActive(true);
        panelStack.Push(gameOverPanel);
    }

    //22_03_01 clear를 위한 함수
    public void displayClearAward(int award)
    {
        mapUI.SelectableButtonsActive();
        clearFlag = true;
        pauseFlag = true;
        int gold = award % 1000;
        int artifactNum = (award / 1000) % 1000;
        int statusChange = award / 1000000;
        print(gold + ", " + artifactNum + ", " + statusChange);
        //22_03_02 배경음악 변경 추가
        audioManager.ChangeMusic("Result");
        levelClearPanel.gameObject.SetActive(true);
        panelStack.Push(levelClearPanel);
        levelClearPanel.GetComponent<ClearPanel>().setAwards(gold, artifactNum, statusChange);
    }

    public void resumebutton()
    {
        if (!gameOverFlag)
        {
            if (clearFlag || dialogFlag)
            {
                if (panelStack.Count == 1)
                {
                    Pause();
                }
                else
                {
                    BackUI();
                }
            }
            else
            {
                if (panelStack.Count == 0)
                {
                    Pause();
                }
                else if (panelStack.Count == 1)
                {
                    Resume();
                }
                else
                {
                    BackUI();
                }
            }
        }
    }
    public void Pause()
    {
        if (pausePanel != null)
        {
            pauseFlag = true;
            Time.timeScale = 0f;
            // Update는 못막음. Couroutine의 yield return waitforseconds는 막음. FixedUpdate 막음.
            // : player를 직접 조작하는 (InputManager <-> Player) 이벤트를 막을 수 없음.
            // 따라서 pauseFlag를 통해 InputManager의 전투관련 입력을 막을 생각임.

            //22_03_02 배경음악이 pausePanel 들어갔다 나오면 그냥 멈춰버려서... 볼륨을 줄이는 것으로 타협했습니다
            //audioManager.changeVolume(0.2f);
            //22_03_03 볼륨 조절에서 뭔가 복잡해져서 그냥 볼륨 줄이는 것도 없앴습니다.
            //AudioListener.pause = true;
            pausePanel.gameObject.SetActive(true);
            panelStack.Push(pausePanel);
        }
    }
    public void Resume()
    {
        if (pausePanel != null)
        {
            pauseFlag = false;
            Time.timeScale = 1f;
            //audioManager.changeVolume(audioManager.getVolume()*0.7f);
            //AudioListener.pause = false;
            pausePanel.gameObject.SetActive(false);
            panelStack.Pop();
        }
    }
    public void MapActive(bool cleared)
    {
        if (cleared)
        {
            clearFlag = true;
            mapUI.SelectableButtonsActive();
        }
        mapPanel.gameObject.SetActive(true);
        if (panelStack.Count > 0) panelStack.Peek().gameObject.SetActive(false);
        panelStack.Push(mapPanel);
    }
    public void MapPanelNextActive(RectTransform button, bool flag) //2022_02_16 
    {
        //2022_02_20 간단한 수정
        if (flag)
        {
            RectTransform mapPanelNext = button.parent.Find("MapPanel_Next").GetComponent<RectTransform>();
            mapPanelNext.anchoredPosition = new Vector2(button.anchoredPosition.x + 160, button.anchoredPosition.y - 10);
            mapPanelNext.SetAsLastSibling();
            mapPanelNext.GetComponent<MapUIBtn>().SetXY(button.GetChild(0).GetComponent<MapUIBtn>().xIndex, button.GetChild(0).GetComponent<MapUIBtn>().yIndex);
            if (!panelStack.Peek().Equals(mapPanelNext))
            {
                mapPanelNext.gameObject.SetActive(true);
                panelStack.Push(mapPanelNext);
            }

        }
        else
        {
            if (panelStack.Peek().Equals(button))
                panelStack.Pop().gameObject.SetActive(false);
        }
    }

    public void ItemSelect_key(InputAction.CallbackContext context)
    {
        if (context.performed && inputManager.isItemSelect('0') && !pauseFlag)
        {
            RectTransform img_item_sel = mainPanel.Find("panel_main").Find("Img_item_sel").GetComponent<RectTransform>();
            if (inputManager.isItemSelect('1'))
            {
                //2022_02_13 player의 유물에 대응
                player.setCurArtifact(0);
                img_item_sel.anchoredPosition = mainPanel.Find("panel_main").Find("Img_item1").GetComponent<RectTransform>().anchoredPosition;
            }
            else if (inputManager.isItemSelect('2'))
            {
                //2022_02_13 player의 유물에 대응
                player.setCurArtifact(1);
                img_item_sel.anchoredPosition = mainPanel.Find("panel_main").Find("Img_item2").GetComponent<RectTransform>().anchoredPosition;
            }
            else if (inputManager.isItemSelect('3'))
            {
                //2022_02_13 player의 유물에 대응
                player.setCurArtifact(2);
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
                //2022_02_13 변경된 구조에 맞춰서 사용될 수 있도록 변경
                player.useArtifact();
                StartCoroutine(AttackCooltime());
            }
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
                }
                else if (inputManager.isMove('d'))
                {
                    direction = new Vector2(0, -1);
                }
                else if (inputManager.isMove('l'))
                {
                    direction = new Vector2(-1, 0);
                }
                else if (inputManager.isMove('r'))
                {
                    direction = new Vector2(1, 0);
                }
                else
                {
                    direction = new Vector2(0, 0);
                }
                player.move(direction);
                StartCoroutine(MoveCooltime());
            }
        }
    }
    public IEnumerator AttackCooltime() // 공격이 성공했을 시 쿨타임 돌리기 -> 다시 공격 호출(키를 누르고 있는 경우 연속적으로)
    {
        if (mainPanel != null)
        {
            attackFlag = false;
            Image img_attack_cooltime = mainPanel.Find("panel_main").Find("Img_attack_clt").GetComponent<Image>();
            float clt = 10f / player.getAttackSpd();
            img_attack_cooltime.fillAmount = 1;
            for (int i = 0; i < clt * 100; i++)
            {
                img_attack_cooltime.fillAmount = 1 - i / (clt * 100);
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
            Image img_move_cooltime = mainPanel.Find("panel_main").Find("Img_move_clt").GetComponent<Image>();
            float clt = 10f / player.getMoveSpd();
            img_move_cooltime.fillAmount = 1;
            for (int i = 0; i < clt * 100; i++)
            {
                img_move_cooltime.fillAmount = 1 - i / (clt * 100);
                yield return new WaitForSeconds(0.01f);
            }
            img_move_cooltime.fillAmount = 0;
            moveFlag = true;
            Move(new InputAction.CallbackContext());
        }
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
        dialogFlag = false;
        pauseFlag = false;
        Time.timeScale = 1f;
        //AudioListener.pause = false;
        //dialogPanel.gameObject.SetActive(false);
        BackUI();
        if (levelManager.currentScene == "Stage1_Start")
        {
            MapActive(true);
        }
    }
    public void DialogNext()
    {
        NewDialogEnumerator.MoveNext();
    }
    //2022_02_11 - hp바를 변경하는 함수의 추가
    public void SettingActive()
    {
        if (settingPanel != null)
        {
            settingPanel.gameObject.SetActive(true);
            panelStack.Peek().gameObject.SetActive(false);
            panelStack.Push(settingPanel);
        }
        else
        {
            Debug.Log("UIManager -> settingPanel Reference error");
        }
    }

    public void changeHpBar()
    {
        mainPanel.Find("hp_bar").GetComponent<Slider>().value = (float)player.getHp() / player.getMaxHp();
        mainPanel.Find("hp_bar").GetComponentInChildren<Text>().text = player.getHp() + "\n/\n" + player.getMaxHp();
    }
    //2022_02_14 
    /// <summary>
    /// Artifact에 해당하는 이미지를 나타내주는 함수
    /// </summary>
    /// <param name="num"> 교체를 원하는 아티팩트 번호, 1, 2, 3 순서로 센다 </param>
    /// <param name="artifactName"> 교체를 원하는 아티팩트의 img 이름</param>
    public void changeArtifact(int num, string artifactName)
    {
        mainPanel.Find("panel_main").Find("Img_Art" + num.ToString()).GetComponent<Image>().sprite =
            Resources.Load<Sprite>("Artifacts/ArtifactImage/" + artifactName);
    }
    //22_03_01 gold 바꿔주는 함수
    public void changeGold(int num)
    {
        mainPanel.Find("panel_main").Find("text_gold").GetComponent<Text>().text = (num.ToString() + "G");
    }
    //22_03_01 힘 바꿔주는 함수
    public void changeSTR(int num)
    {
        mainPanel.Find("panel_main").Find("text_dmg").GetComponent<Text>().text = num.ToString();
    }
    //22_03_01 이속 바꿔주는 함수
    public void changeMOVSPD(int num)
    {
        mainPanel.Find("panel_main").Find("text_spd").GetComponent<Text>().text = num.ToString();
    }
    //22_03_01 공속 바꿔주는 함수
    public void changeATKSPD(int num)
    {
        mainPanel.Find("panel_main").Find("text_agl").GetComponent<Text>().text = num.ToString();
    }


    public void something(GameObject g)
    {
        SaveManager sm = levelManager.GetComponent<SaveManager>();
        if (g.GetComponentInChildren<Text>().text.Equals("Restart"))
        {
            g.GetComponentInChildren<Text>().text = "Sure?";
        }
        else if (g.GetComponentInChildren<Text>().text.Equals("Sure?"))
        {
            sm.killPlayer();
            SwitchScene.toTitle();
        }
    }
}
