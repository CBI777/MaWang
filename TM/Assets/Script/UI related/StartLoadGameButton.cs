using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartLoadGameButton : MonoBehaviour
{
    [SerializeField]
    private LevelManager levelManager;

    void Start()
    {
        //kill player�� �ϰ� Title������ �������� ���ٸ�, �� ������ �����ϰ� RoomType�� Title�̴�.
        //2022_02_13 ������ RoomType�񱳷�
        SaveManager sm = levelManager.GetComponent<SaveManager>();
        if(sm==null || sm.saving.roomType.Equals("Title"))
        {
            this.transform.GetComponent<Image>().color = Color.black;
            this.transform.GetComponent<Button>().interactable = false;
        }
    }
}
