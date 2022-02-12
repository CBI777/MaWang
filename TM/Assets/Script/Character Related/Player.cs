using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : CharacterBase
{
    [SerializeField] private int gold;
    //22_2_9 - artifacts �߰�
    [SerializeField]
    private string[] artifacts = new string[3];
    [SerializeField] private SpawnManager spawnManager;

    //2022_02_11 - UIManager�� UIManager�� �ϴϱ� ����� �ν��� �ȵǴ� ������ �־, uiManager�� �����Ͽ����ϴ�.
    [SerializeField] private UIManager uiManager;

    //22_2_9 - getGold�� getArtifact�߰�
    public int getGold() { return this.gold; }
    public string getArtifact(int num)
    {
        if (num >= 0 && num <= 2)
        {
            return artifacts[num];
        }
        else
        {
            return "Invalid";
        }
    }

    //22_2_9 - ���̺����Ͽ��� ���� �������� �Լ� �߰�
    public void updatePlayer(PlayerSaveBase player)
    {
        this.characterName = player.characterName;
        this.moveSpeed = player.moveSpeed;
        this.moveDistance = player.moveDistance;
        this.strength = player.strength;
        this.attackSpeed = player.attackSpeed;
        this.maxHp = player.maxHp;
        this.hp = player.hp;
        this.gold = player.gold;
        this.artifacts[0] = player.artifact1;
        this.artifacts[1] = player.artifact2;
        this.artifacts[2] = player.artifact3;
    }

    public void attack() 
    {
        if (uiManager.getAttackFlag())
        {
            spawnManager.AttackCharacter(transform.position, this.getAttackRange(), this.getStr());
        }
        else { Debug.Log("������"); }
    }
    public void move(Vector2 direction)
    {
        if (uiManager.getMoveFlag())
        {
            if (spawnManager.MoveCharacter(transform.position, direction))
            {
                Vector3 targetLoc = new Vector3(direction.x,direction.y, 0) + transform.position;
                this.transform.position = targetLoc;
                //2022_02_09 ������ �����ֱ� ���ؼ� �߰�
                setDirection(direction, this.transform.position);
            }
        }
    }

    //2022_02_11 - player�� hp�ٰ� ���� �ֱ� ������ override�Ͽ���.
    public override bool hpDamage(int damage)
    {
        if (this.hp <= damage)
        {
            this.hp = 0;
            uiManager.changeHpBar();
            Destroy(gameObject);
            //>>>>>>>>>>>>>LevelManager���� killPlayer �����ϰ� gameover��Ű�� �ڵ� �ʿ�
            return true;
        }
        this.hp -= damage;
        uiManager.changeHpBar();

        return false;
    }
    //2022_02_11 - ���� �׽�Ʈ��.
    public void testhpDamage()
    {
        if (hp < 0) { hp = 1; }
        else { this.hp--; }
        uiManager.changeHpBar();
    }
    public override void hpHeal(int heal)
    {
        if (this.maxHp >= (this.hp + heal))
        {
            this.hp = maxHp;
            return;
        }
        this.hp += heal;
        //2022_02_11 - hp�� ��
        uiManager.changeHpBar();
    }

}
