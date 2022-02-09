using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private PlayerInput playerInput;
    private InputAction moveAction, escAction, attackAction, itemSelAction;
    private InputAction move_up, move_down, move_left, move_right, item_1, item_2, item_3, item_scroll;


    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        //moveAction = playerInput.actions["move"];
        attackAction = playerInput.actions["attack"];
        escAction = playerInput.actions["escape"];
        //itemSelAction = playerInput.actions["item_select"];
        move_up = playerInput.actions["move_up"];
        move_down = playerInput.actions["move_down"];
        move_left = playerInput.actions["move_left"];
        move_right = playerInput.actions["move_right"];
        item_1 = playerInput.actions["item_1"];
        item_2 = playerInput.actions["item_2"];
        item_3 = playerInput.actions["item_3"];
        item_scroll = playerInput.actions["item_scroll"];

    }
    //public bool isMove() { return this.moveAction.IsPressed(); }
    public bool isMove(char direction)
    {
        switch (direction)
        {
            case 'u': return move_up.IsPressed();
            case 'd': return move_down.IsPressed();
            case 'l': return move_left.IsPressed();
            case 'r': return move_right.IsPressed();
            case 'a': return move_up.IsPressed() || move_down.IsPressed() || move_left.IsPressed() || move_right.IsPressed();
        }
        return false;
    }
    public bool isAttack() { return this.attackAction.IsPressed(); }
    public bool isEscape() { return this.escAction.IsPressed(); }
    public bool isItemSelect(char slot)
    {
        switch (slot)
        {
            case '1': return item_1.IsPressed();
            case '2': return item_2.IsPressed();
            case '3': return item_3.IsPressed();
            default: return item_1.IsPressed() || item_2.IsPressed() || item_3.IsPressed();
        }
    }



}
