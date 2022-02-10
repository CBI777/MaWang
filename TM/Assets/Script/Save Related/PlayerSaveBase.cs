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
    /* 현재 스테이지의 방 번호를 나타내줌
    * 이 둘의 main purpose는 random map을 생성할지 말지의 여부를 결정해주는 것이다.
    * CurRoomNumber는 매번 room이 바뀔때마다 1씩 올라간다.
    * 즉, prev와 cur이 같지 않은 상태에서 scene이 시작된다면, 이건 그 room에 처음으로 들어갔다는 것이다.
    *  = random을 돌려야한다. 이렇게 random을 돌리고 난 다음엔 prev를 1올려준다.
    * 만약 prev == cur이면 load를 했다는 거니까 random을 돌리지말고 읽어온다.
    */
    public int prevRoomNumber;
    public int curRoomNumber;
    //방의 유형을 나타내줌. 이건 scene_Name과 동일하다.
    public string roomType;

    /* stageVar은 현재 stage의 상황을 의미한다.
     * Event의 경우에는 몇번째 이벤트인지를 stageVar에 저장하고, 그 이외는 -1으로.
     * 전투 stage의 경우에는 map의 variation을 stageVar에, enemy의 variation을 stageVar2에 저장, 그 이외는 -1.
     * Shop의 경우에는 (상품이 3개라는 가정하에) 3개의 물품들의 번호를 저장하면 된다.
     * 
     * 1. 상점
     * 현재 생각을 하고 있는 것은 모든 artifact를 json이나 xls로 저장하여 <번호(int), 물품(string)>으로 저장을 해 두는 것이다.
     * 이러면 stageVar를 이용하여 번호를 넣는 것 만으로 artifact의 물품 string으로 접근을 하는 것이 가능하고, 단순히 resources에서 불러오면 될 것.
     * 
     * 2. 이벤트
     * 어떻게 구현을 하는가에 달렸겠지만, 각 이벤트 번호를 통해서 이벤트를 찾는 방식을 이용하면 stageVar을 사용할 수 있을 것으로 생각한다.
     */
    public int stageVar1;
    public int stageVar2;
    public int stageVar3;
}
