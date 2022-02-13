using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public enum Directions { E, W, S, N }

[System.Serializable]
public class CharacterBase : MonoBehaviour
{
    //22_2_9 - ��ȣ���� ������ private -> protected
    [SerializeField] protected string characterName;
    [SerializeField] protected int moveDistance;
    [SerializeField] protected int moveSpeed;
    [SerializeField] protected int strength;
    [SerializeField] protected int attackSpeed;

    //22_02_10 - hud�� ���� ���� �߰� inspector���� ���̵��� �߽��ϴ�.
    [SerializeField] private GameObject hpBackground;
    [SerializeField] private Image hpForeground;
    [SerializeField] protected GameObject directionArrow;

    [SerializeField]
    protected List<Vector3Int> attackRange;

    [SerializeField] protected int maxHp;
    [SerializeField] protected int hp;
    [SerializeField] protected bool isDestroyable;
    [SerializeField] protected Directions direction;

    //22_02_10 - hud�� ���� ����.
    /*
     * 1. player�� hp�ٴ� �ʿ���� ������ �ʿ���. ���� false, true
     * 2. ���ʹ� �� �� �ʿ���. ���� true, true
     * 3. �μ� �� �ִ� ��ֹ��� hp�ٴ� �ʿ��ϰ� ������ �ʿ� ����. ���� true, false
     * 4. �μ� �� ���� ��ֹ��� �� �� �ʿ����. ���� false, false
     */
    [SerializeField] private bool hpHud;
    [SerializeField] protected bool dirHud;

    public string getName() { return this.name; }
    public int getMoveDist() { return this.moveDistance; }
    public int getMoveSpd() { return this.moveSpeed; }
    public int getStr() { return this.strength; }
    public int getAttackSpd() { return this.attackSpeed; }
    public int getHp() { return this.hp; }
    public int getMaxHp() { return this.maxHp; }
    public bool getIsDestroyable() { return this.isDestroyable; }
    public Directions getDirections() { return this.direction; }
    public List<Vector3Int> getAttackRange()
    {
        return this.attackRange;
    }
    //22_2_9 - setDirection�߰�
    public void setDirection(Directions dir, Vector3 loc)
    {
        if (hpHud) { hpBackground.transform.position = loc + (new Vector3(0, 0.5f)); }
        this.direction = dir;
        if (dirHud) { directionArrow.transform.position = loc + (new Vector3(0, -0.4f)); rotateArrow(); }
    }
    //2022_02_10 - ������ player�� ����� ��.
    public void setDirection(Vector3 dir, Vector3 loc)
    {
        if (hpHud) { hpBackground.transform.position = loc + (new Vector3(0, 0.5f)); }

        if (dir.x == 0 && dir.y == 1) { this.direction = Directions.N; }
        else if (dir.x == 1 && dir.y == 0) { this.direction = Directions.E; }
        else if (dir.x == -1 && dir.y == 0) { this.direction = Directions.W; }
        else { this.direction = Directions.S; }

        if (dirHud) { directionArrow.transform.position = loc + (new Vector3(0, -0.4f)); rotateArrow(); }
    }


    //22_02_10 - hud�� ���ؼ� �߰�
    void Start()
    {
        this.transform.GetComponentInChildren<Canvas>().renderMode = RenderMode.WorldSpace;
        this.transform.GetComponentInChildren<Canvas>().worldCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        if (hpHud)
        {
            hpBackground.SetActive(true);
            hpForeground.fillAmount = 1f;
            //0222_02_10 - ��� ���⼭ localScale�� �־��� ���� �ִµ�, inspector���� �־��ָ� �ڵ� �� �� �پ inspector���� �־���.
            //hpBackground.transform.localScale = new Vector3(0.85f, 0.15f);
            hpBackground.transform.position = this.transform.position + (new Vector3(0, 0.5f));
        }
        if (dirHud)
        {
            directionArrow.SetActive(true);
            rotateArrow();
            directionArrow.transform.position = this.transform.position + (new Vector3(0, -0.4f));
            //0222_02_10 - ��� ���⼭ localScale�� �־��� ���� �ִµ�, inspector���� �־��ָ� �ڵ� �� �� �پ inspector���� �־���.
            //directionArrow.transform.localScale = new Vector3(0.25f, 0.25f);
        }
    }
    //2022_02_10 - �ڵ�� �־��ָ� ���ο� ���̳� ��ֹ� �����ÿ� ���� ����. �ٸ� �׷��� �ڵ尡 �� ������.


    //2022_02_11 - player���� override�� ���ؼ� virtual�� ����
    public virtual bool hpDamage(int damage)
    {
        if (this.hp <= damage)
        {
            Destroy(gameObject);
            return true;
        }
        this.hp -= damage;
        //2022_02_10 - hp hud��
        if (hpHud) { hpForeground.fillAmount = (float)hp / (float)maxHp; }
        return false;
    }
    public virtual void hpHeal(int heal)
    {
        if (this.maxHp >= (this.hp + heal))
        {
            this.hp = maxHp;
            return;
        }
        this.hp += heal;
        //2022_02_10 - hp hud��
        if (hpHud) { hpForeground.fillAmount = (float)hp / (float)maxHp; }
    }

    //static �Լ��� �̿��Ͽ� ���̸� Ȯ ����.
    public void rotateArrow()
    {
        directionArrow.transform.rotation = 
            Quaternion.Euler(DirectionChange.dirToRotation(direction));
        /*switch (direction)
        {
            case Directions.E:
                directionArrow.transform.rotation = Quaternion.Euler(DirectionChange.);
                break;
            case Directions.N:
                directionArrow.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
                break;
            case Directions.S:
                directionArrow.transform.rotation = Quaternion.Euler(0f, 0f, -90f);
                break;
            case Directions.W:
                directionArrow.transform.rotation = Quaternion.Euler(0f, 0f, 180f);
                break;
        }*/
    }
}
