using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : CharacterBase
{

    /* 
     * 2022_02_07
     * CharacterBase�� ��ӹ��� Player. (�ǹ� : �� MonsterController�� ��ӹ��� �ʴ°�?)
     * 
     * 
     */

    [SerializeField] private int gold;
    [SerializeField] private SpawnManager spawnManager;
    [SerializeField] private UIManager UIManager;
    //���� spawnmanager������ �θ�� �����ϸ� �ǰ����� �ȿ��ڰ� �����Ƽ� �׳� �ν�����~

    public void attack() 
    {
        
            if (UIManager.getAttackFlag())
            {
                spawnManager.AttackCharacter(transform.position, this.getAttackRange(), this.getStr());
            }
            else { Debug.Log("������"); }
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
