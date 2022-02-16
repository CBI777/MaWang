using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapUIBtn : MonoBehaviour
{

    UIManager uiManager;
    park.MapUI mapUI;
    public int xIndex, yIndex; //0216 무슨 정보를 가진 셀을 클릭했는지의 정보를 전달하기 위함

    private void Start()
    {
        uiManager = GameObject.FindWithTag("UIManager").GetComponent<UIManager>();
        mapUI = GameObject.FindWithTag("UIManager").GetComponent<park.MapUI>();
    }
    public void MapButtonActive()
    {
        if (uiManager != null)
        {
            mapUI.SelectDraw(transform, true);
            uiManager.MapPanelNextActive(transform.parent.GetComponent<RectTransform>(), true);
        }
        else
            Debug.Log("UIManager null");
    }

    public void MapNextButtonAccept()
    {
        if (uiManager != null)
        {
            //SwitchScene.changeScene();
        }
        else
            Debug.Log("UIManager null");
    }
    public void MapNextButtonCancel()
    {
        if (uiManager != null)
        {
            uiManager.MapPanelNextActive(null,false);
        }
        else
            Debug.Log("UIManager null");
    }
}
