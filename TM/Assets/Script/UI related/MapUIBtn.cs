using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapUIBtn : MonoBehaviour
{

    UIManager uiManager;
    park.MapUI mapUI;
    SaveBase saving;
    public int xIndex, yIndex; //2022_02_16 ���� ������ ���� ���� Ŭ���ߴ����� ������ �����ϱ� ���� : Accept�޼ҵ忡�� ���

    private void Start()
    {
        saving = GameObject.FindWithTag("LevelManager").GetComponent<SaveManager>().saving;
        uiManager = GameObject.FindWithTag("UIManager").GetComponent<UIManager>();
        mapUI = GameObject.FindWithTag("UIManager").GetComponent<park.MapUI>();
    }
    public void MapButtonActive() //2022_02_16 �� UI�� ��ư�� ������ �� -> ���� �г�
    {
        if (uiManager != null)
        {
            mapUI.SelectDraw(transform, true);
            uiManager.MapPanelNextActive(transform.parent.GetComponent<RectTransform>(), true);
        }
        else
            Debug.Log("UIManager null");
    }

    public void MapNextButtonAccept() //2022_02_19 ���� �г��� üũ��ư ������ �� (���� : �г��� ��ư�� ������ �޼ҵ���)
    {
        if (uiManager != null)
        {//������ Start�� ���°� ���� ���� ����
            string scene = "Stage";
            scene += saving.stageNumber + "_";
            if ((mapUI.mapInfos[yIndex][xIndex] & ~park.cell.ClrType) == park.cell.Normal)
            {
                scene = "Stage";
                scene += saving.stageNumber + "_";
                scene += "Normal";
            }
            else if ((mapUI.mapInfos[yIndex][xIndex] & ~park.cell.ClrType) == park.cell.Elite)
            {
                scene = "Stage";
                scene += saving.stageNumber + "_";
                scene += "Elite";
            }
            else if ((mapUI.mapInfos[yIndex][xIndex] & ~park.cell.ClrType) == park.cell.Shop)
            {
                scene = "Shop";
            }
            else if ((mapUI.mapInfos[yIndex][xIndex] & ~park.cell.ClrType) == park.cell.Event)
            {
                scene = "Stage";
                scene += saving.stageNumber + "_";
                scene += "Event";
            }
            else if (mapUI.mapInfos[yIndex][xIndex] == park.cell.Boss)
            {
                scene = "Stage";
                scene += saving.stageNumber + "_";
                scene += "Boss";
            }
            else
            {
                scene = "Title";
                Debug.Log("�̰� ���� �����ϱ�..?");
            }
            mapUI.UpdateMapData(yIndex,xIndex);
            SwitchScene.changeScene(scene);
        }
        else
            Debug.Log("UIManager null");
    }
    public void MapNextButtonCancel() //2022_02_19 ���� �г��� X��ư ������ ��
    {
        if (uiManager != null)
        {
            uiManager.MapPanelNextActive(transform.GetComponent<RectTransform>(), false);
        }
        else
            Debug.Log("UIManager null");
    }
    public void SetXY(int x, int y) //2022_02_19 �гο��� ���� ���õ� ���� ��ǥ�� ����.
    {
        //public������ ������ ���ϳ� �Ͱ����� �� ������...
        //���� �̰� ������ ������ �������� ���� ���ϴ�
        //�ſ� Ư���� ��Ȳ������ ȣ���� �޼ҵ�� �����ϼŵ� ū ������ ������
        xIndex = x;
        yIndex = y;
    }
}
