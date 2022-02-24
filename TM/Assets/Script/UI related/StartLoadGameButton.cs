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
        //kill player를 하고 Title밖으로 나간적이 없다면, 그 순간만 유일하게 RoomType이 Title이다.
        //2022_02_13 수정됨 RoomType비교로
        if(levelManager.GetComponent<SaveManager>().saving.roomType == "Title")
        {
            this.transform.GetComponent<Image>().color = Color.black;
            this.transform.GetComponent<Button>().interactable = false;
        }
    }
}
