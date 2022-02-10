using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchScene : MonoBehaviour
{
    public void changeScene(string sceneName)
    {
        GameObject.FindWithTag("LevelManager").GetComponent<PlayerSaveManager>().saveRoomEnd();
        GameObject.FindWithTag("LevelManager").GetComponent<LevelManager>().LoadScene(sceneName);
    }

    public void loadLatestScene()
    {
        string roomType = GameObject.FindWithTag("LevelManager").GetComponent<PlayerSaveManager>().saving.roomType;
        //������ - ���� else�ȿ� �͸� �־���.
        if (roomType == "Start")
        {
            GameObject.FindWithTag("LevelManager").GetComponent<PlayerSaveManager>().saveRoomEnd();
            GameObject.FindWithTag("LevelManager").GetComponent<LevelManager>().LoadScene("Stage1_Normal");
        }
        else
        {
            GameObject.FindWithTag("LevelManager").GetComponent<LevelManager>().LoadScene(roomType);
        }
        
    }
}
