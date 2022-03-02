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
    /// ������ amount�� ����ϴ�. 0.0~1.0f. �⺻�����δ� 0.7f�Դϴ�.
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
