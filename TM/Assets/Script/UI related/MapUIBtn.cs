using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapUIBtn : MonoBehaviour
{

    UIManager uiManager;
    park.MapUI mapUI;
    SaveBase saving;
    public int xIndex, yIndex; //2022_02_16 무슨 정보를 가진 셀을 클릭했는지의 정보를 전달하기 위함 : Accept메소드에서 사용

    private void Start()
    {
        saving = GameObject.FindWithTag("LevelManager").GetComponent<SaveManager>().saving;
        uiManager = GameObject.FindWithTag("UIManager").GetComponent<UIManager>();
        mapUI = GameObject.FindWithTag("UIManager").GetComponent<park.MapUI>();
    }
    public void MapButtonActive() //2022_02_16 맵 UI의 버튼을 눌렀을 때 -> 작은 패널
    {
        if (uiManager != null)
        {
            mapUI.SelectDraw(transform, true);
            uiManager.MapPanelNextActive(transform.parent.GetComponent<RectTransform>(), true);
        }
        else
            Debug.Log("UIManager null");
    }

    public void MapNextButtonAccept() //2022_02_19 작은 패널의 체크버튼 눌렀을 때 (주의 : 패널의 버튼이 실행할 메소드임)
    {
        if (uiManager != null)
        {//보스나 Start로 가는건 아직 구현 안함
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
                Debug.Log("이건 무슨 오류일까..?");
            }
            mapUI.UpdateMapData(yIndex,xIndex);
            SwitchScene.changeScene(scene);
        }
        else
            Debug.Log("UIManager null");
    }
    public void MapNextButtonCancel() //2022_02_19 작은 패널의 X버튼 눌렀을 때
    {
        if (uiManager != null)
        {
            uiManager.MapPanelNextActive(transform.GetComponent<RectTransform>(), false);
        }
        else
            Debug.Log("UIManager null");
    }
    public void SetXY(int x, int y) //2022_02_19 패널에게 현재 선택된 셀의 좌표를 저장.
    {
        //public변수를 가지고 뭐하나 싶겠지만 다 사정이...
        //굳이 이거 때문에 오류가 생길일은 없길 빕니다
        //매우 특수한 상황에서만 호출할 메소드라 무시하셔도 큰 무리는 없을듯
        xIndex = x;
        yIndex = y;
    }
}
