using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlockButton : MonoBehaviour
{
    [SerializeField]
    private LevelManager levelManager;

    void Start()
    {
        //kill player�� �ϰ� Title������ �������� ���ٸ�, �� �����������ϰ� prevRoomNumber�� 0�̴�.
        if(levelManager.GetComponent<PlayerSaveManager>().saving.prevRoomNumber == 0)
        {
            this.transform.GetComponent<Image>().color = Color.black;
            this.transform.GetComponent<Button>().interactable = false;
        }
    }
}
