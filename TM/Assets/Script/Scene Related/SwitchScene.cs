using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 2022_02_09 - SwitchScene�� ����� :
 * �� script�� ��ư���� ����մϴ�.
 * 1. ��ư�� SwitchScene�� �߰�.
 * 2. ��ư�� Button�� On_Click()�� List�� �߰�.
 * 3. Runtime Only() �ؿ� �ִ� ���� attach�� SwitchScene script�� ����ٰ� ����.
 * 4. Runtime Only() ���� �ִ� ������ SwitchScene -> ���ϴ� �Լ� ����
 * 5. changeSceneó�� �Ű������� ������ �־��ָ� ��.
 * 
 * 
 */

public class SwitchScene : MonoBehaviour
{
    //��������� ���� ������ ������ �� ���Ǵ� �Լ�.
    //saveRoomEnd�� ���ؼ� ���� ������ �� �����Լ��� �θ���, sceneName�� ���� ���� scene���� �̵��Ѵ�.
    public static void changeScene(string sceneName)
    {
        GameObject.FindWithTag("LevelManager").GetComponent<SaveManager>().saveRoomEnd();
        GameObject.FindWithTag("LevelManager").GetComponent<LevelManager>().LoadScene(sceneName);
    }
    //2022_02_11 �׽�Ʈ�� ���� �ӽ����� �Լ�
    public static void testchangeScene(string sceneName)
    {
        GameObject.FindWithTag("LevelManager").GetComponent<SaveManager>().saveRoomClear();
        GameObject.FindWithTag("LevelManager").GetComponent<LevelManager>().LoadScene(sceneName);
    }

    //load�� �ϴ� ��� ����Ѵ�.
    //���� �־��� ���� ���������� Ȯ���ϱ� ���ؼ� saving���� roomType�� Ȯ���Ѵ�.
    public static void loadLatestScene()
    {
        string roomType = GameObject.FindWithTag("LevelManager").GetComponent<SaveManager>().saving.roomType;
        GameObject.FindWithTag("LevelManager").GetComponent<LevelManager>().LoadScene(roomType);
    }

    //�ӽ�. �����ϱ⸦ ������ �̰� ����ȴ�.
    public static void loadNewGame()
    {
        //�̾��ϱ� data�� �ִµ��� �����ϱ⸦ ������ ��츦 ����Ͽ� killPlayer�� �־��.
        GameObject.FindWithTag("LevelManager").GetComponent<SaveManager>().killPlayer();
        changeScene("Stage1_Start");
    }
}
