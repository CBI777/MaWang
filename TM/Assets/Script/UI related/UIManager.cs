using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;


public class UIManager : MonoBehaviour
{
    //22_03_02 audioManager �߰�
    [SerializeField] private AudioManager audioManager;
    //22_03_01 gameOver�� ���� �г��� �߰�
    [SerializeField] private RectTransform gameOverPanel;
    //22_03_01 levelClear�� ���� �г��� �߰�
    [SerializeField] private RectTransform levelClearPanel;
    [SerializeField] private RectTransform pausePanel, mainPanel, mapPanel, dialogPanel, settingPanel;
    //��� ������ ���� ������ ����
    //�� ���� ��ȭ �����̳� �̺�Ʈ, �������� UI ���� �� Ŭ������ ��ӹ޾Ƽ� Ȱ���ϴ°ɷ�? _02_07_06:42
    //�ƴϸ� ��״� �⺻UI�� ���ΰ� ���ο� UI�� ���� �� ���� ��ũ��Ʈ�� ���� �װ͵�� �����ϴ� ������? _02_07_06:48
    //���� �����ϰ�, NULLüũ�� �Ͽ� ������ ����
    //��¥�� �߸� ����� ���� �ƴ϶�� NULL�� Panel�� �����ϴ� �޼ҵ带 ȣ���� ���� ����. _02_07_08:16

    //2022_02_09 - �׽�Ʈ�� ���ؼ� LevelManager�� �޾ƿ��� �κ��� ����
    //2022_02_09 - �׽�Ʈ�� ���ؼ� ���� ��ġ�� �˷��ִ� string�� whereAmI�� ����
    [SerializeField] private LevelManager levelManager;
    private string whereAmI;
    [SerializeField] private InputManager inputManager;
    [SerializeField] private park.MapUI mapUI;
    [SerializeField] private Player player;

    //2022_02_28 
    [SerializeField] private List<string> dialog;
    private IEnumerator NewDialogEnumerator;

    private Stack<RectTransform> panelStack;
    //UI���� ŭ���� Panel ������ ��� ������ ��.
    //Main Panel�� �����ϰ� Input�� ������� ���� Panel�� �� �ϳ��� �����Ұ�.
    //�ϳ��� �� ���¿��� �ٸ� �ϳ��� �� ���� �� �� ���·� ���ư��� ���� ESC�� ���.
    //���� ���� ��Ģ�� �ؼ��ϱ� ���� ������� �����̶�� �� �� �ִ�.

    private bool pauseFlag = false;
    private bool attackFlag = true, moveFlag = true;
    //22_03_01 ���ӿ����� ���� flag �߰�
    private bool gameOverFlag = false;
    //22_03_01 ������ ���� flag �߰�
    private bool clearFlag = false;
    //22_03_02
    private bool dialogFlag = false;

    //22_03_01 Ÿ�̹� ������ awake�߰�
    public void Awake()
    {
        panelStack = new Stack<RectTransform>(8);
        Time.timeScale = 1f; // 2022_02_20 �� ��ȯ �� paused ���¿��� �����ϸ� �ð��� ���缭 �� ���۽� �ڵ����� Ǯ���ִ� ����
    }

    public void Start()
    {

        //2022_02_11 - hp�� �κ��� �Լ��� ����
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
        //22_03_01 ���ӿ����� �Ǹ� ESC�� �ȸ����� �ϰ� �ʹ�.
        //�׷��� gameoverflag�� �˻縦 �ϵ��� �ٲپ���.
        //����, clearȭ��� stack�� �������� �ذ��Ϸ��� clearFlag�� �˻縦 �߰��Ͽ���.
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

    //22_03_01 ���ӿ����� ���� �Լ�
    public void gameOver()
    {
        //22_03_03 gameOver ����� ���� �߰�
        audioManager.ChangeMusicWithoutDelay("GameOver");
        //gameOverFlag�� true�� �ٲ���, player�� �⺻���� �������� ���°��� pauseFlag.
        gameOverFlag = true;
        pauseFlag = true;
        Time.timeScale = 0f;
        gameOverPanel.gameObject.SetActive(true);
        panelStack.Push(gameOverPanel);
    }

    //22_03_01 clear�� ���� �Լ�
    public void displayClearAward(int award)
    {
        mapUI.SelectableButtonsActive();
        clearFlag = true;
        pauseFlag = true;
        int gold = award % 1000;
        int artifactNum = (award / 1000) % 1000;
        int statusChange = award / 1000000;
        print(gold + ", " + artifactNum + ", " + statusChange);
        //22_03_02 ������� ���� �߰�
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
            // Update�� ������. Couroutine�� yield return waitforseconds�� ����. FixedUpdate ����.
            // : player�� ���� �����ϴ� (InputManager <-> Player) �̺�Ʈ�� ���� �� ����.
            // ���� pauseFlag�� ���� InputManager�� �������� �Է��� ���� ������.

            //22_03_02 ��������� pausePanel ���� ������ �׳� ���������... ������ ���̴� ������ Ÿ���߽��ϴ�
            //audioManager.changeVolume(0.2f);
            //22_03_03 ���� �������� ���� ���������� �׳� ���� ���̴� �͵� ���ݽ��ϴ�.
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
        //2022_02_20 ������ ����
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
                //2022_02_13 player�� ������ ����
                player.setCurArtifact(0);
                img_item_sel.anchoredPosition = mainPanel.Find("panel_main").Find("Img_item1").GetComponent<RectTransform>().anchoredPosition;
            }
            else if (inputManager.isItemSelect('2'))
            {
                //2022_02_13 player�� ������ ����
                player.setCurArtifact(1);
                img_item_sel.anchoredPosition = mainPanel.Find("panel_main").Find("Img_item2").GetComponent<RectTransform>().anchoredPosition;
            }
            else if (inputManager.isItemSelect('3'))
            {
                //2022_02_13 player�� ������ ����
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
                //2022_02_13 ����� ������ ���缭 ���� �� �ֵ��� ����
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
    public IEnumerator AttackCooltime() // ������ �������� �� ��Ÿ�� ������ -> �ٽ� ���� ȣ��(Ű�� ������ �ִ� ��� ����������)
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
    public IEnumerator MoveCooltime() // �̵��� �������� �� ~~
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
    //2022_02_11 - hp�ٸ� �����ϴ� �Լ��� �߰�
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
    /// Artifact�� �ش��ϴ� �̹����� ��Ÿ���ִ� �Լ�
    /// </summary>
    /// <param name="num"> ��ü�� ���ϴ� ��Ƽ��Ʈ ��ȣ, 1, 2, 3 ������ ���� </param>
    /// <param name="artifactName"> ��ü�� ���ϴ� ��Ƽ��Ʈ�� img �̸�</param>
    public void changeArtifact(int num, string artifactName)
    {
        mainPanel.Find("panel_main").Find("Img_Art" + num.ToString()).GetComponent<Image>().sprite =
            Resources.Load<Sprite>("Artifacts/ArtifactImage/" + artifactName);
    }
    //22_03_01 gold �ٲ��ִ� �Լ�
    public void changeGold(int num)
    {
        mainPanel.Find("panel_main").Find("text_gold").GetComponent<Text>().text = (num.ToString() + "G");
    }
    //22_03_01 �� �ٲ��ִ� �Լ�
    public void changeSTR(int num)
    {
        mainPanel.Find("panel_main").Find("text_dmg").GetComponent<Text>().text = num.ToString();
    }
    //22_03_01 �̼� �ٲ��ִ� �Լ�
    public void changeMOVSPD(int num)
    {
        mainPanel.Find("panel_main").Find("text_spd").GetComponent<Text>().text = num.ToString();
    }
    //22_03_01 ���� �ٲ��ִ� �Լ�
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
