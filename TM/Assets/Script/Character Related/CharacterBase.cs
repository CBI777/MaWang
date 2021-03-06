using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public enum Directions { E, W, S, N }

[System.Serializable]
public class CharacterBase : MonoBehaviour
{
    /*22-1-19 변동사항
    전 변수 private화
    전 변수의 get함수 작성
    maxHp 변수의 생성
    hpDamage와 hpHeal함수의 생성
    */
    [SerializeField] private string characterName;
    [SerializeField] private int moveDistance;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float strength;
    [SerializeField] private float attackSpeed;

    [SerializeField]
    private List<Vector3Int> attackRange;

    [SerializeField] private int maxHp;
    [SerializeField] private int hp;
    [SerializeField] private bool isDestroyable;
    [SerializeField] private Directions direction;

    public string getName() { return this.name; }
    public int getMoveDist() { return this.moveDistance; }
    public float getMoveSpd() { return this.moveSpeed; }
    public float getStr() { return this.strength; }
    public float getAttackSpd() { return this.attackSpeed; }
    public int getHp() { return this.hp; }
    public int getMaxHp() { return this.maxHp; }
    public bool getIsDestroyable() { return this.isDestroyable; }
    public Directions getDirections() { return this.direction; }
    public List<Vector3Int> getAttackRange()
    {
        return this.attackRange;
    }

    public bool hpDamage(int damage)
    {
        if(this.hp <= damage)
        {
            Destroy(gameObject);
            return true;
        }
        this.hp -= damage;
        return false;
    }
    public void hpHeal(int heal)
    {
        if(this.maxHp >= (this.hp + heal))
        {
            this.hp = maxHp;
            return;
        }
        this.hp += heal;
    }
}
