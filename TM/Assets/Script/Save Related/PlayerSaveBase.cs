using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSaveBase
{
    //���� player�� ������ ��Ÿ����.
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

    //���� �� �������������� ��Ÿ����
    public int stageNumber;
    //���� ���������� �� ��ȣ�� ��Ÿ����
    public int prevRoomNumber;
    public int curRoomNumber;
    //���� ������ ��Ÿ����. �̰� scene_Name�� �����ϴ�.
    public string roomType;

    /* stageVar�� ���� stage�� ��Ȳ�� �ǹ��Ѵ�.
     * Event�� ��쿡�� ���° �̺�Ʈ������ stageVar�� �����ϰ�, �� �ܴ̿� -1����.
     * ���� stage�� ��쿡�� map�� variation�� stageVar��, enemy�� variation�� stageVar2�� ����, �� �ܴ̿� -1.
     * Shop�� ��쿡�� (��ǰ�� 3����� �����Ͽ�) 3���� ��ǰ���� ��ȣ�� �����ϸ� �ȴ�.
     */
    public int stageVar1;
    public int stageVar2;
    public int stageVar3;
}
