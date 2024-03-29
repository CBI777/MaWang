using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public enum Directions { E, W, S, N, O } //O = origin

[System.Serializable]
public class CharacterBase : MonoBehaviour
{
    //22_2_9 - 보호수준 문제로 private -> protected
    [SerializeField] protected string characterName;
    [SerializeField] protected int moveDistance;
    [SerializeField] protected int moveSpeed;
    [SerializeField] protected int strength;
    [SerializeField] protected int attackSpeed;

    //220306 - protected 변경
    [SerializeField] protected GameObject hpBackground;
    [SerializeField] protected Image hpForeground;
    [SerializeField] protected GameObject directionArrow;

    [SerializeField] protected int maxHp;
    [SerializeField] protected int hp;
    [SerializeField] protected bool isDestroyable;
    [SerializeField] protected Directions direction;

    //22_02_10 - hud를 위한 변수.
    /*
     * 1. player는 hp바는 필요없고 방향은 필요함. 따라서 false, true
     * 2. 몬스터는 둘 다 필요함. 따라서 true, true
     * 3. 부술 수 있는 장애물은 hp바는 필요하고 방향은 필요 없음. 따라서 true, false
     * 4. 부술 수 없는 장애물은 둘 다 필요없음. 따라서 false, false
     */
    //220306 - protected 변경
    [SerializeField] protected bool hpHud;
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

    //220306
    public virtual void setDirection(Directions dir, Vector3 loc)
    {
        if (hpHud) { hpBackground.transform.position = loc + (new Vector3(0, 0.5f)); }
        this.direction = dir;

        //22_02_25 flip기능 추가
        if (this.direction == Directions.S || this.direction == Directions.W)
        {
            gameObject.GetComponent<SpriteRenderer>().flipX = false;
        }
        else { gameObject.GetComponent<SpriteRenderer>().flipX = true; }

        if (dirHud) { directionArrow.transform.position = loc + (new Vector3(0, -0.4f)); rotateArrow(); }
    }


    //이쪽이 player용
    public void setDirection(Vector3 dir, Vector3 loc)
    {
        if (hpHud) { hpBackground.transform.position = loc + (new Vector3(0, 0.5f)); }

        if (dir.x == 0 && dir.y == 1) { this.direction = Directions.N; }
        else if (dir.x == 1 && dir.y == 0) { this.direction = Directions.E; }
        else if (dir.x == -1 && dir.y == 0) { this.direction = Directions.W; }
        else { this.direction = Directions.S; }

        //22_02_25 flip기능 추가
        if (this.direction == Directions.S || this.direction == Directions.W)
        {
            gameObject.GetComponent<SpriteRenderer>().flipX = false;
        }
        else { gameObject.GetComponent<SpriteRenderer>().flipX = true; }

        if (dirHud) { directionArrow.transform.position = loc + (new Vector3(0, -0.4f)); rotateArrow(); }
    }


    //22_03_01 virtual로 변경
    /// <summary>
    /// 지정된 값만큼 힘을 변경해주는 함수. 1이하로는 내려갈 수 없다.
    /// </summary>
    /// <param name="newstr">힘의 변화도</param>
    public virtual void changeStr(int plusStr)
    {
        this.strength += plusStr;
        if (this.strength <= 0)
        {
            this.strength = 1;
        }
    }

    //22_02_25
    /// <summary>
    /// 지정된 값으로 힘을 변경해주는 함수. 1이하로는 내려갈 수 없다.
    /// </summary>
    /// <param name="newStr">새로운 힘의 int</param>
    public void setStr(int newStr)
    {
        if (newStr <= 0) { this.strength = 1; }
        else { this.strength = newStr; }
    }


    //22_02_10 - hud를 위해서 추가
    void Start()
    {
        this.transform.GetComponentInChildren<Canvas>().renderMode = RenderMode.WorldSpace;
        this.transform.GetComponentInChildren<Canvas>().worldCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        if (hpHud)
        {
            hpBackground.SetActive(true);
            hpForeground.fillAmount = 1f;
            //0222_02_10 - 사실 여기서 localScale을 넣어줄 수도 있는데, inspector에서 넣어주면 코드 한 줄 줄어서 inspector에서 넣었음.
            //hpBackground.transform.localScale = new Vector3(0.85f, 0.15f);
            hpBackground.transform.position = this.transform.position + (new Vector3(0, 0.5f));
        }
        if (dirHud)
        {
            directionArrow.SetActive(true);
            rotateArrow();
            directionArrow.transform.position = this.transform.position + (new Vector3(0, -0.4f));
            //0222_02_10 - 사실 여기서 localScale을 넣어줄 수도 있는데, inspector에서 넣어주면 코드 한 줄 줄어서 inspector에서 넣었음.
            //directionArrow.transform.localScale = new Vector3(0.25f, 0.25f);
        }
    }
    //2022_02_10 - 코드로 넣어주면 새로운 적이나 장애물 생성시에 아주 편함. 다만 그러면 코드가 꽤 많아짐.


    //2022_02_11 - player에서 override를 위해서 virtual로 변경
    public virtual bool hpDamage(int damage)
    {
        if (this.hp <= damage)
        {
            Destroy(gameObject);
            return true;
        }
        this.hp -= damage;
        //2022_02_10 - hp hud용
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
        //2022_02_10 - hp hud용
        if (hpHud) { hpForeground.fillAmount = (float)hp / (float)maxHp; }
    }

    //static 함수를 이용하여 길이를 확 줄임.
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
