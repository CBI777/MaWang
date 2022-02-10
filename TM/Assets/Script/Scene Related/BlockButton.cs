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
        //kill player를 하고 Title밖으로 나간적이 없다면, 그 순간만유일하게 prevRoomNumber가 0이다.
        if(levelManager.GetComponent<PlayerSaveManager>().saving.prevRoomNumber == 0)
        {
            this.transform.GetComponent<Image>().color = Color.black;
            this.transform.GetComponent<Button>().interactable = false;
        }
    }
}
