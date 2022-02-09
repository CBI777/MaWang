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
    //���� �����ϰ�, NULLüũ�� �Ͽ� ������ ����
    //��¥�� �߸� ����� ���� �ƴ϶�� NULL�� Panel�� �����ϴ� �޼ҵ带 ȣ���� ���� ����. _02_07_08:16

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
    }

    public bool getAttackFlag() { return attackFlag; }
    public bool getMoveFlag() { return moveFlag; }

    public void Escape(InputAction.CallbackContext context)
    {
        if (context.performed)
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
    public void BackUI()
    {
        panelStack.Pop().gameObject.SetActive(false);
        panelStack.Peek().gameObject.SetActive(true);
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
            AudioListener.pause = true;
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
            AudioListener.pause = false;
            pausePanel.gameObject.SetActive(false);
            panelStack.Pop();
        }
    }
    public void MapActive()
    {
        mapPanel.gameObject.SetActive(true);
        panelStack.Peek().gameObject.SetActive(false);
        panelStack.Push(mapPanel);
    }

    public void ItemSelect_key(InputAction.CallbackContext context)
    {
        if (context.performed && inputManager.isItemSelect('0') && !pauseFlag)
        {
            RectTransform img_item_sel = mainPanel.Find("panel_main").Find("Img_item_sel").GetComponent<RectTransform>();
            if (inputManager.isItemSelect('1'))
            {
                //���� �÷��̾� 1�� ������ ���� �ڵ�
                img_item_sel.anchoredPosition = mainPanel.Find("panel_main").Find("Img_item1").GetComponent<RectTransform>().anchoredPosition;
            }
            else if (inputManager.isItemSelect('2'))
            {
                //���� �÷��̾� 2�� ������ ���� �ڵ�
                img_item_sel.anchoredPosition = mainPanel.Find("panel_main").Find("Img_item2").GetComponent<RectTransform>().anchoredPosition;
            }
            else if (inputManager.isItemSelect('3'))
            {
                //���� �÷��̾� 3�� ������ ���� �ڵ�
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