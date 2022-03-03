using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    //2022_02_09 - loadingCanvas�� �ε�â�� �ǹ��ϰ�, currentScene�� �� �״�� ���� scene�� �ǹ��մϴ�.
    //�̰͵� inspector�� ���ϴٰ� �ϼż� �׷��� �� �� �ְ� �߽��ϴ�.
    public GameObject loadingCanvas;
    public string currentScene;

    //2022_02_09 - scene�� ���۵Ǹ� �ε�â�� disable�Ѵ�.
    private void Awake()
    {
        loadingCanvas.SetActive(false);
    }

    //2022_02_09 - async function�� ���. ������ ���� delay�����ε�,
    public async void LoadScene(string sceneName)
    {
        var scene = SceneManager.LoadSceneAsync(sceneName);
        scene.allowSceneActivation = false;

        loadingCanvas.SetActive(true);

        do
        {
            await Task.Delay(2000);
            //2022_02_09 - �Ϻη� delay�� 2������ ��. �̰� ���� ����. debug / �ʹ� ���� transition�������̴�.
        } while (scene.progress < 0.9f);
        ////2022_02_09 - Unity�� scene�� 0.9�� �Ǿ��� �� load�� �Ǳ� ������ 0.9�� �ξ����.

        scene.allowSceneActivation = true;
    }

    //22_03_01
    public void levelClear()
    {
        if(currentScene.Equals("Stage1_Boss"))
        {
            //���̾�α׸� ���⼭ �ߵ��ϵ��� ó��
            //���̾�α����� ������� ������ �ʿ��� ������ ���δ�.
            //�ϴ�, ������ ���ڸ��� ���� ����δ� �� �� ���� �ǹǷ�...
            //�־��� ��쿡�� if���� ������������ ���� ���ɼ��� ũ��.
        }
        transform.GetComponent<SaveManager>().saveRoomClear();
    }
    //22_03_01
    public void gameOver()
    {
        transform.GetComponent<SaveManager>().killPlayer();
        GameObject.FindWithTag("UIManager").GetComponent<UIManager>().gameOver();
    }
}
