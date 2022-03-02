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
// 2022_02_19 SaveManager�� �ּ����� ����ߵ� saveRoom~���ñ⸦ �׳� savePlayer�� �ٲ���ϴ�


public class SwitchScene : MonoBehaviour
{
    //��������� ���� ������ ������ �� ���Ǵ� �Լ�.
    //saveRoomEnd�� ���ؼ� ���� ������ �� �����Լ��� �θ���, sceneName�� ���� ���� scene���� �̵��Ѵ�.
    public static void changeScene(string sceneName)
    {
        //22_03_02
        GameObject.FindWithTag("LevelManager").GetComponent<SaveManager>().savePlayer(true,true);
        GameObject.FindWithTag("LevelManager").GetComponent<SaveManager>().saving.stageFlag = false;
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
        //changeScene("Stage1_Start");
        GameObject.FindWithTag("LevelManager").GetComponent<LevelManager>().LoadScene("Stage1_Start");
    }

    //22_03_01
    public static void toTitle()
    {
        GameObject.FindWithTag("LevelManager").GetComponent<LevelManager>().LoadScene("Title");
    }
    //22_03_01
    public static void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
