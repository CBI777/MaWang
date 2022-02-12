using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : CharacterBase
{
    [SerializeField] private int gold;
    //22_2_9 - artifacts 추가
    [SerializeField]
    private string[] artifacts = new string[3];
    [SerializeField] private SpawnManager spawnManager;

    //2022_02_11 - UIManager를 UIManager로 하니까 제대로 인식이 안되는 문제가 있어서, uiManager로 변경하였습니다.
    [SerializeField] private UIManager uiManager;

    //22_2_9 - getGold랑 getArtifact추가
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

    //22_2_9 - 세이브파일에서 정보 가져오는 함수 추가
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
        else { Debug.Log("공격쿨"); }
    }
    public void move(Vector2 direction)
    {
        if (uiManager.getMoveFlag())
        {
            if (spawnManager.MoveCharacter(transform.position, direction))
            {
                Vector3 targetLoc = new Vector3(direction.x,direction.y, 0) + transform.position;
                this.transform.position = targetLoc;
                //2022_02_09 방향을 보여주기 위해서 추가
                setDirection(direction, this.transform.position);
            }
        }
    }

    //2022_02_11 - player는 hp바가 따로 있기 때문에 override하였음.
    public override bool hpDamage(int damage)
    {
        if (this.hp <= damage)
        {
            this.hp = 0;
            uiManager.changeHpBar();
            Destroy(gameObject);
            //>>>>>>>>>>>>>LevelManager한테 killPlayer 실행하고 gameover시키는 코드 필요
            return true;
        }
        this.hp -= damage;
        uiManager.changeHpBar();

        return false;
    }
    //2022_02_11 - 순수 테스트용.
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
        //2022_02_11 - hp바 용
        uiManager.changeHpBar();
    }

}
