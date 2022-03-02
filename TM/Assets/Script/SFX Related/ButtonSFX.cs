using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Button의 onClick에서는 아예 static인 class를 부를 수 없습니다. 따라서 이렇게 MonoBehaviour를 상속받는 일반적인 class를 만들었습니다.
public class ButtonSFX : MonoBehaviour
{
    //효과음 내기
    public static void playSFX(string soundName)
    {
        SoundEffecter.playSFX(soundName);
    }
    //음악 바꾸기
    public static void changeBGM(string musicName)
    {
        GameObject.FindWithTag("AudioManager").GetComponent<AudioManager>().ChangeMusic(musicName);
    }
}
