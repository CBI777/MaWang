using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{
    /*22-1-19 ��������
     characterbase���� ���������� �������� �������� ���� get�Լ��� ���� 
     Attack�Լ� ���� ����
     Move�Լ� ����
     Awake���� �ִ� �ڱ� ������ ��ġ �����ϴ� �͵� ����
     map�� tileManager���� ���� spawnManager�� ����ϵ��� ���� ����
    */

    private PlayerMovement controls;

    private void Awake()
    {
        controls = new PlayerMovement();
    }

    private void OnEnable()
    {
        controls.Enable();
    }
    private void OnDisable()
    {
        controls.Disable();
    }

    /*Unity�� input system�� �̿��Ͽ� input�� �д� ����� ���������� �ִ�.
     * ���⼭�� action map�� �̿��� ��, ���� press only�� �̿��Ͽ���.
     * action map���� ������ �ϰԵǸ� press only / release only / press and release�� ������ �����ϴ�.
     * 
    1. Update�� �̿��ϴ� ���
    ������ �� Ư�� value�� pass�ϴ� ���� �ƴ϶��, triggered�� ���� �� �ִ�.
    ���� value�� �д´ٸ� ReadValue���� ����� �� �ִ�.
    triggered�� Ű�� ���� frame������ true�� return�Ѵ�.
    �̷��� update�� �̿��ϴ� ����� ������ getkey�� getkeydown�� �̿��ϴ� ����� �����ϴ�.


    2. Start�� �̿��ϴ� ���
    Unity�� input system�� �� 5������ state�� �����Ѵ� : disabled, waiting, started, performed, canceled.
    disabled�� �� �״�� disabled�Ǿ� input�� ���� �� ���� ����, waiting�� enabled�Ǿ� input�� ��ٸ��� �ִ� ����
    started�� input system�� input�� �޾� action�� interaction�� ������ �ٷ� �� ����.
    performed�� action�� interaction�� ���� ����(=performed)
    canceled�� action�� cancel�� ������ ���Ѵ�.

    Started������ �� �� started, performed, canceled�� �̿��� �� �ִ�.
    �� �� callback���� Input.CallbackContext��� structure�� �޴´�. �̰� ctx��, �̰��� ���� action�� context info�� ������ �ִ�.
    �츮�� �̸� �̿��Ͽ� current state of action�� query�� �� �ְ� �ȴ�.

    += ctx => ???�� �̿��Ͽ� �츮�� ���ϴ� �Լ��� �ߵ���ų ���� �ְ�,
    moveó�� ctx���� readvalue�� �����ͼ� input�� ���ؼ� return�� value�� �о�� ���� �ִ�.
    using UnityEngine.InputSystem;�� ����ϸ� += ctx���̵� �� callbackContext�� �Ű������� ������ �Լ��� �ٷ� �߰��� ���� �ִ�.

    private void attack(InputAction.CallbackContext context)
    {
        //Debug.Log(...);
        //context.ReadValue...
        //context.ReadValueAsButton...
        //...
    }
    ó�� �Լ��� �ۼ��ϸ�, ctx�� �� �� �ִ� ���� �� context�� Ȯ�� �����ϸ�,
    controls.Main.Attack.performed += attack;
    ó�� �ۼ��Ͽ� ������ ���� �ִ�.

    ctx�� ����ϴ� ����� callbackContext�� subscribe�ϴ� ����̰�,
    ���ڴ� �� value�� directly read�ϴ� ����̴�.

    Start�� �̿��ϸ� �� �� ���� �ߵ��ϰ� �ϰų�,
    �ʿ��� ���� �ڷ�ƾ�� �߻����� �� ���� update�� �������� �ʰ� ó���� �� �� �ִ�.


    3. Unity�� built-in component�� Player Input�� �̿�
    inspector���� player Input�� ã�Ƽ� ����ְ�, action map�� �־ ����Ѵ�.
    player input manager�� ����ϰ� �Ǹ� multi-player������ ����� �� �ֵ��� �����ش�.
    ������ �ڵ带 ������ �� ���� �̰� 'Unity ������ ���'�̶�� �� �� �ְڴ�.
    */

    private void Start()
    {
        controls.Main.Movement.performed += ctx => Move(ctx.ReadValue<Vector2>());
        controls.Main.Attack.performed += ctx => Attack();
    }

    

    /*
    private void Update()
    {
        
        if(controls.Main.Attack.triggered)
        {
            Attack();
        }
    }*/

    private void Attack()
    {
        transform.GetComponentInParent<SpawnManager>().AttackCharacter(
            transform.position,
            transform.GetComponent<CharacterBase>().getAttackRange(),
            (int)transform.GetComponent<CharacterBase>().getStr()
            );

    }

    private void Move(Vector2 direction)
    {
        if (transform.GetComponentInParent<SpawnManager>().MoveCharacter(transform.position, direction))
        {
            transform.position += (Vector3)direction;
            print("Moving to " + transform.position);
            return;
        }
        print("Moving Denied, tile is not movable");
    }
}
