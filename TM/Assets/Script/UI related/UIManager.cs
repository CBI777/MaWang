using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;


public class UIManager : MonoBehaviour
{
    [SerializeField] private RectTransform pausePanel, mainPanel, mapPanel;
    //��� ������ ���� ������ ����
    //�� ���� ��ȭ �����̳� �̺�Ʈ, �������� UI ���� �� Ŭ������ ��ӹ޾Ƽ� Ȱ���ϴ°ɷ�? _02_07_06:42
    //�ƴϸ� ��״� �⺻UI�� ���ΰ� ���ο� UI�� ���� �� ���� ��ũ��Ʈ�� ���� �װ͵�� �����ϴ� ������? _02_07_06:48
    //��� : ���� �����ϰ�, NULLüũ�� �Ͽ� ������ ����
    //��¥�� �߸� ����� ���� �ƴ϶�� NULL�� Panel�� �����ϴ� �޼ҵ带 ȣ���� ���� ����. _02_07_08:16

    //2022_02_09 - �׽�Ʈ�� ���ؼ� LevelManager�� �޾ƿ��� �κ��� ����
    //2022_02_09 - �׽�Ʈ�� ���ؼ� ���� ��ġ�� �˷��ִ� string�� whereAmI�� ����
    [SerializeField] private LevelManager levelManager;
    private string whereAmI;

    [SerializeField] private SpawnManager spawnManager;
    [SerializeField] private InputManager inputManager;
    [SerializeField] private Player player;

    private Stack<RectTransform> panelStack;
    //UI���� ŭ���� Panel ������ ��� ������ ��.
    //Main Panel�� �����ϰ� Input�� ������� ���� Panel�� �� �ϳ��� �����Ұ�.
    //�ϳ��� �� ���¿��� �ٸ� �ϳ��� �� ���� �� �� ���·� ���ư��� ���� ESC�� ���.
    //���� ���� ��Ģ�� �ؼ��ϱ� ���� ������� �����̶�� �� �� �ִ�.



    private bool pauseFlag = false;
    private bool attackFlag = true, moveFlag = true;


    public void Start()
    {
        panelStack = new Stack<RectTransform>(8);

        //2022_02_09 - �׽�Ʈ�� ���� �߰��� �κ�, ���߿� ������ �� ����ŵ� ������ �κ��Դϴ�.
        whereAmI = "Room" + levelManager.GetComponent<PlayerSaveManager>().saving.curRoomNumber + " / " + levelManager.GetComponent<LevelManager>().currentScene;
        GameObject.FindWithTag("Text").GetComponent<Text>().text = whereAmI;
        //2022_02_10 - hp�ٸ� ����ý��ϴ�.
        mainPanel.Find("hp_bar").GetComponent<Slider>().value = (float)player.getHp() / player.getMaxHp();
        mainPanel.Find("hp_bar").GetComponentInChildren<Text>().text = player.getHp() + "\n/\n" + player.getMaxHp();
    }

    public bool getAttackFlag() { return attackFlag; }
    public bool getMoveFlag() { return moveFlag; }

    public void Escape(InputAction.CallbackContext context)
    {
        if (context.performed) // �� ���� ������ ���� ȣ��� (started/performed/canceled)
        {
            if (panelStack.Count == 0) //esc ÷ ��������? �׷� pause
            {
                Pause();
            }
            else if (panelStack.Count == 1) //�̹� pause�� ���վ��? �׷� resume
            {
                Resume();
            }
            else //Pause ���� ���� ���� ���ֳ׿�? : �ϳ��� �ݱ�
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
    public void Pause() //�ð��� ���߰�! pauseUI�� ���
    {
        if (pausePanel != null)
        {
            pauseFlag = true;
            Time.timeScale = 0f;
            // Update�� ������. Couroutine�� yield return waitforseconds�� ����. FixedUpdate ����.
            // : player�� ���� �����ϴ� (InputManager <-> Player) �̺�Ʈ�� ���� �� ����.
            // ���� pauseFlag�� ���� ����&�̵�ó���� ���ϰ� ��.
            AudioListener.pause = true;
            pausePanel.gameObject.SetActive(true);
            panelStack.Push(pausePanel);
        }
        else { Debug.Log("PauseUI Null"); }
    }
    public void Resume() //�ð��� �簳�ϰ�! PauseUI�� Disable
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
    public void MapActive() //�� UI ��ü�� ���
    {
        if (mapPanel != null)
        {
            mapPanel.gameObject.SetActive(true);
            panelStack.Peek().gameObject.SetActive(false);
            panelStack.Push(mapPanel);
        }
        else { Debug.Log("MapUI Null"); }
    }
    public void panel_AccessCheck_Active(RectTransform rectTransform)  //2022_02_12_08:37 �߰� : ���� �� �� �� Ȯ�����ִ� UIPanel. ���� �̰� UIManager�� �ؾ� �Ƴ� ������ esc�� �� �� ������ ���� �� ���Ƽ� �ߴ�
    {
        //�Ű����� rectTransform�� MapUIBtn(�ӽ�) ��ũ��Ʈ���� �̺�Ʈ ���� ��ü(�� ��ư)�� ������ �� ��ǥ ���� panel_AccessCheck(Ȯ��/���)�� ����
        if (rectTransform != null)
        {
            RectTransform panel_AccessCheck = rectTransform.transform.parent.Find("Panel_AccessCheck").GetComponent<RectTransform>();
            if (panelStack.Peek().Equals(panel_AccessCheck)) //�̹� �����ִ� ���¿��� �ٸ� ������ ����� �� panelStack�� ���� ������Ʈ�� �ι� ���̴� �� ����
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
            else { Debug.Log("������"); }
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
            else { Debug.Log("�̵���"); }
        }
    }

    public IEnumerator AttackCooltime() // ������ �������� �� ��Ÿ�� ������ -> �ٽ� ���� ȣ��(Ű�� ������ �ִ� ��� ����������)
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
    public IEnumerator MoveCooltime() // �̵��� �������� �� ~~
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
