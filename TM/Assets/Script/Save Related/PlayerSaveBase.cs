using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSaveBase
{
    //현재 player의 정보를 나타낸다.
    public string characterName;
    public int moveDistance;
    public int moveSpeed;
    public int strength;
    public int attackSpeed;
    public int maxHp;
    public int hp;
    public int gold;
    public string artifact1;
    public string artifact2;
    public string artifact3;

    //현재 몇 스테이지인지를 나타내줌
    public int stageNumber;
    //현재 스테이지의 방 번호를 나타내줌
    public int prevRoomNumber;
    public int curRoomNumber;
    //방의 유형을 나타내줌. 이건 scene_Name과 동일하다.
    public string roomType;

    /* stageVar은 현재 stage의 상황을 의미한다.
     * Event의 경우에는 몇번째 이벤트인지를 stageVar에 저장하고, 그 이외는 -1으로.
     * 전투 stage의 경우에는 map의 variation을 stageVar에, enemy의 variation을 stageVar2에 저장, 그 이외는 -1.
     * Shop의 경우에는 (상품이 3개라는 가정하에) 3개의 물품들의 번호를 저장하면 된다.
     */
    public int stageVar1;
    public int stageVar2;
    public int stageVar3;
}
