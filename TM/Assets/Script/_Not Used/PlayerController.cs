using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{
    /*22-1-19 변동사항
     characterbase에서 직접적으로 가져오던 변수들을 전부 get함수로 수정 
     Attack함수 구조 변경
     Move함수 변경
     Awake내에 있는 자기 스스로 위치 변경하는 것도 제거
     map과 tileManager없이 전부 spawnManager가 담당하도록 구조 변경
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

    /*Unity의 input system을 이용하여 input을 읽는 방법은 여러가지가 있다.
     * 여기서는 action map을 이용할 때, 전부 press only를 이용하였다.
     * action map에서 변경을 하게되면 press only / release only / press and release로 변경이 가능하다.
     * 
    1. Update를 이용하는 방식
    눌렀을 때 특정 value를 pass하는 것이 아니라면, triggered로 읽을 수 있다.
    만약 value를 읽는다면 ReadValue등을 사용할 수 있다.
    triggered는 키가 눌린 frame에서만 true를 return한다.
    이렇게 update를 이용하는 방식은 기존의 getkey나 getkeydown을 이용하는 방법과 유사하다.


    2. Start를 이용하는 방식
    Unity의 input system은 총 5가지의 state를 제공한다 : disabled, waiting, started, performed, canceled.
    disabled는 말 그대로 disabled되어 input을 받을 수 없는 상태, waiting은 enabled되어 input을 기다리고 있는 상태
    started는 input system이 input을 받아 action과 interaction을 시작한 바로 그 순간.
    performed는 action과 interaction이 끝난 순간(=performed)
    canceled는 action이 cancel된 순간을 말한다.

    Started에서는 이 중 started, performed, canceled를 이용할 수 있다.
    이 세 callback들은 Input.CallbackContext라는 structure로 받는다. 이게 ctx고, 이것은 현재 action의 context info를 가지고 있다.
    우리는 이를 이용하여 current state of action을 query할 수 있게 된다.

    += ctx => ???을 이용하여 우리가 원하는 함수를 발동시킬 수도 있고,
    move처럼 ctx에서 readvalue를 가져와서 input을 통해서 return된 value를 읽어올 수도 있다.
    using UnityEngine.InputSystem;을 사용하면 += ctx없이도 이 callbackContext를 매개변수로 가지는 함수를 바로 추가할 수도 있다.

    private void attack(InputAction.CallbackContext context)
    {
        //Debug.Log(...);
        //context.ReadValue...
        //context.ReadValueAsButton...
        //...
    }
    처럼 함수를 작성하면, ctx로 할 수 있는 것을 다 context로 확인 가능하며,
    controls.Main.Attack.performed += attack;
    처럼 작성하여 진행할 수도 있다.

    ctx를 사용하는 방법은 callbackContext를 subscribe하는 방법이고,
    후자는 그 value를 directly read하는 방식이다.

    Start를 이용하면 딱 한 번만 발동하게 하거나,
    필요한 때만 코루틴을 발생시켜 매 순간 update를 괴롭히지 않고도 처리를 할 수 있다.


    3. Unity의 built-in component인 Player Input을 이용
    inspector에서 player Input을 찾아서 집어넣고, action map을 넣어서 사용한다.
    player input manager을 사용하게 되면 multi-player에서도 사용할 수 있도록 도와준다.
    오히려 코드를 만지는 것 보다 이게 'Unity 스러운 방식'이라고도 할 수 있겠다.
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
