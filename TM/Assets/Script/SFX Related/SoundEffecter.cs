using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SoundEffecter
{
    static float volume=0.7f;
    static float volumeBeforeMute=volume;
    /// <summary>
    /// ������ amount�� ����ϴ�. 0.0~1.0f. �⺻�����δ� 0.7f�Դϴ�.
    /// </summary>
    /// <param name="amount"></param>
    public static void setVolume(float amount)
    {
        volume =  amount;
    }
    public static float getVolume() { return volume; }
    public static float mute()
    {
        if (volume > 0f)
        {
            volumeBeforeMute = volume;
            setVolume(0f);
        }
        else
        {
            setVolume(volumeBeforeMute);
        }
        return volume;
    }
    public static void playSFX(string name)
    {
        GameObject soundEffect = GameObject.Instantiate(Resources.Load<GameObject>("SFX/SoundEffecter"));
        AudioSource audios = soundEffect.GetComponent<AudioSource>();
        audios.clip = Resources.Load<AudioClip>("SFX/SoundEffects/" + name);
        audios.volume = volume;
        audios.Play();
        GameObject.Destroy(soundEffect, audios.clip.length);

    }



}
