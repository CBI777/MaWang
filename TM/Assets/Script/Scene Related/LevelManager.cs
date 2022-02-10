using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    /*
    public static LevelManager levManager;
    public string currentScene;

    [SerializeField] private GameObject loadingCanvas;

    private void Awake()
    {
        loadingCanvas.SetActive(false);
        if (levManager == null)
        {
            levManager = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
*/
    public GameObject loadingCanvas;
    public string currentScene;

    private void Awake()
    {
        //loadingCanvas = GameObject.FindWithTag("Transition");
        //currentScene = SceneManager.GetActiveScene().name;
        loadingCanvas.SetActive(false);
    }

    public async void LoadScene(string sceneName)
    {
        //StartCoroutine(LoadLevel(sceneName));
        var scene = SceneManager.LoadSceneAsync(sceneName);
        scene.allowSceneActivation = false;

        loadingCanvas.SetActive(true);

        do
        {
            await Task.Delay(2000);
            //일부러 delay를 2초정도 줌. 이건 빼도 무방. debug / 너무 빠른 transition방지용이다.
        } while (scene.progress < 0.9f);

        scene.allowSceneActivation = true;
    }

    /*
    IEnumerator LoadLevel(string sceneName)
    {
        yield return new WaitForSeconds(1);

        SceneManager.LoadScene(sceneName);
    }
    */
}
