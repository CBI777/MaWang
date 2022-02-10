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
    [SerializeField] private UIManager UIManager;

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
        if (UIManager.getAttackFlag())
        {
            spawnManager.AttackCharacter(transform.position, this.getAttackRange(), this.getStr());
        }
        else { Debug.Log("공격쿨"); }
    }
    public void move(Vector2 direction)
    {
        if (UIManager.getMoveFlag())
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
    
}
