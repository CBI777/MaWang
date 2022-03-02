using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SoundEffecter
{

    public static void playSFX(string name)
    {
        if(Resources.Load<GameObject>("SFX/SoundEffecter") == null) { Debug.Log("wwwerjfefvef"); }
        GameObject soundEffect = GameObject.Instantiate(Resources.Load<GameObject>("SFX/SoundEffecter"));
        AudioSource audios = soundEffect.GetComponent<AudioSource>();
        audios.clip = Resources.Load<AudioClip>("SFX/SoundEffects/" + name);
        audios.Play();
        GameObject.Destroy(soundEffect, audios.clip.length);

    }



}
