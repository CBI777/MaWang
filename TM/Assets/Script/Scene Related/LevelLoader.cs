using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public Animator transition;

    public float transitiontime = 1f;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.C))
        {
            switch(SceneManager.GetActiveScene().buildIndex)
            {
                case 0:
                    LoadNextLevel(1);
                    break;
                case 1:
                    LoadNextLevel(2);
                    break;
                case 2:
                    LoadNextLevel(0);
                    break;
            }
        }
    }

    public void LoadNextLevel(int levelIndex)
    {
        //StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex + 1));
        StartCoroutine(LoadLevel(levelIndex));
    }



    IEnumerator LoadLevel(int levelIndex)
    {
        //Transition Animation play
        transition.SetTrigger("Start");

        //wait until end
        yield return new WaitForSeconds(1);

        //Load the scene
        SceneManager.LoadScene(levelIndex);
    }
}
