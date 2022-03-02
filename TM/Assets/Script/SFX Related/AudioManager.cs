using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    AudioSource bgm;

    public void changeMusic(string musicName)
    {
        StartCoroutine(soundDownAndUp(musicName));
    }

    /// <summary>
    /// 볼륨을 amount로 맞춥니다. 0.0~1.0f. 기본적으로는 0.7f입니다.
    /// </summary>
    /// <param name="amount"></param>
    public void changeVolume(float amount)
    {
        this.bgm.volume = amount;
    }

    public void changeMusicWithoutDelay(string musicName)
    {
        bgm.clip = Resources.Load<AudioClip>("SFX/Music/" + musicName);
        bgm.Play();
    }

    IEnumerator soundDownAndUp(string musicName)
    {
        while (this.bgm.volume > 0f)
        {
            this.bgm.volume -= Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }
        bgm.clip = Resources.Load<AudioClip>("SFX/Music/" + musicName);
        bgm.Play();
        while (this.bgm.volume < 0.7f)
        {
            this.bgm.volume += Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }
    }

}
