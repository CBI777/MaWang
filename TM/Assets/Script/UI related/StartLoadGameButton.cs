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
        if(levelManager.GetComponent<SaveManager>().saving.roomType == "Title")
        {
            this.transform.GetComponent<Image>().color = Color.black;
            this.transform.GetComponent<Button>().interactable = false;
        }
    }
}
