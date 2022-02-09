using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : CharacterBase
{

    /* 
     * 2022_02_07
     * CharacterBase를 상속받은 Player. (의문 : 왜 MonsterController는 상속받지 않는가?)
     * 
     * 
     */

    [SerializeField] private int gold;
    [SerializeField] private SpawnManager spawnManager;
    [SerializeField] private UIManager UIManager;
    //물론 spawnmanager정도는 부모로 접근하면 되겠지만 안예쁘고 귀찮아서 그냥 인스펙터~

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
            }
        }
    }
    
}
