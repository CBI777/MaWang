using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 2022_02_09 - SwitchScene의 사용방법 :
 * 이 script는 버튼에서 사용합니다.
 * 1. 버튼에 SwitchScene을 추가.
 * 2. 버튼의 Button의 On_Click()의 List를 추가.
 * 3. Runtime Only() 밑에 있는 곳에 attach된 SwitchScene script를 끌어다가 넣음.
 * 4. Runtime Only() 옆에 있는 곳에서 SwitchScene -> 원하는 함수 지정
 * 5. changeScene처럼 매개변수가 있으면 넣어주면 끝.
 * 
 * 
 */

public class SwitchScene : MonoBehaviour
{
    //통상적으로 다음 방으로 진행할 때 사용되는 함수.
    //saveRoomEnd를 통해서 방이 끝났을 때 저장함수를 부르고, sceneName에 따라 다음 scene으로 이동한다.
    public static void changeScene(string sceneName)
    {
        GameObject.FindWithTag("LevelManager").GetComponent<SaveManager>().saveRoomEnd();
        GameObject.FindWithTag("LevelManager").GetComponent<LevelManager>().LoadScene(sceneName);
    }
    //2022_02_11 테스트를 위한 임시적인 함수
    public static void testchangeScene(string sceneName)
    {
        GameObject.FindWithTag("LevelManager").GetComponent<SaveManager>().saveRoomClear();
        GameObject.FindWithTag("LevelManager").GetComponent<LevelManager>().LoadScene(sceneName);
    }

    //load를 하는 경우 사용한다.
    //원래 있었던 방이 어디었는지를 확인하기 위해서 saving내의 roomType을 확인한다.
    public static void loadLatestScene()
    {
        string roomType = GameObject.FindWithTag("LevelManager").GetComponent<SaveManager>().saving.roomType;
        GameObject.FindWithTag("LevelManager").GetComponent<LevelManager>().LoadScene(roomType);
    }

    //임시. 새로하기를 누르면 이게 진행된다.
    public static void loadNewGame()
    {
        //이어하기 data가 있는데도 새로하기를 누르는 경우를 대비하여 killPlayer를 넣어둠.
        GameObject.FindWithTag("LevelManager").GetComponent<SaveManager>().killPlayer();
        changeScene("Stage1_Start");
    }
}
