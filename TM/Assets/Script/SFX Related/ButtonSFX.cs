using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Button�� onClick������ �ƿ� static�� class�� �θ� �� �����ϴ�. ���� �̷��� MonoBehaviour�� ��ӹ޴� �Ϲ����� class�� ��������ϴ�.
public class ButtonSFX : MonoBehaviour
{
    //ȿ���� ����
    public static void playSFX(string soundName)
    {
        SoundEffecter.playSFX(soundName);
    }
    //���� �ٲٱ�
    public static void changeBGM(string musicName)
    {
        GameObject.FindWithTag("AudioManager").GetComponent<AudioManager>().ChangeMusic(musicName);
    }
}
