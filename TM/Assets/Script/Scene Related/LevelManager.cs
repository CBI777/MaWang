using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    //2022_02_09 - loadingCanvas는 로딩창을 의미하고, currentScene은 말 그대로 현재 scene을 의미합니다.
    //이것도 inspector가 편하다고 하셔서 그렇게 쓸 수 있게 했습니다.
    public GameObject loadingCanvas;
    public string currentScene;

    //2022_02_09 - scene이 시작되면 로딩창은 disable한다.
    private void Awake()
    {
        loadingCanvas.SetActive(false);
    }

    //2022_02_09 - async function을 사용. 이유는 밑의 delay때문인데,
    public async void LoadScene(string sceneName)
    {
        var scene = SceneManager.LoadSceneAsync(sceneName);
        scene.allowSceneActivation = false;

        loadingCanvas.SetActive(true);

        do
        {
            await Task.Delay(2000);
            //2022_02_09 - 일부러 delay를 2초정도 줌. 이건 빼도 무방. debug / 너무 빠른 transition방지용이다.
        } while (scene.progress < 0.9f);
        ////2022_02_09 - Unity는 scene이 0.9가 되었을 때 load가 되기 때문에 0.9로 두어야함.

        scene.allowSceneActivation = true;
    }

    public void levelClear()
    {
        
    }

    public void gameOver()
    {
        transform.GetComponent<SaveManager>().killPlayer();
        GameObject.FindWithTag("UIManager").GetComponent<UIManager>().gameOver();
    }
}
