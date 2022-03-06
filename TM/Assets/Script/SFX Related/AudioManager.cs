using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    AudioSource bgm;

    private void Start()
    {
        bgm.volume = SoundEffecter.getVolume();
    }

    public float GetVolume() { return this.bgm.volume; }

    public void ChangeMusic(string musicName)
    {
        StartCoroutine(soundDownAndUp(musicName));
    }
    public void ChangeMusicWithoutDelay(string musicName)
    {
        bgm.clip = Resources.Load<AudioClip>("SFX/Music/" + musicName);
        bgm.Play();
    }
 
    //03_03 º¼·ý ÀÏ°ý Á¶Á¤À» À§ÇÏ¿©..!
    public void UpdateMasterVolume(float amount)
    {
        SoundEffecter.setVolume(amount);
        bgm.volume=SoundEffecter.getVolume();
    }

    //03_03 staticÀº ÀÎ½ºÆåÅÍ¿¡ ¾È¶ß³×¿ä..?
    public void Mute()
    {
        bgm.volume = SoundEffecter.mute();
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
        while (this.bgm.volume < SoundEffecter.getVolume())
        {
            this.bgm.volume += Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }
    }

}
