using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventUI : MonoBehaviour
{
    [SerializeField] Player player;
    // Start is called before the first frame update
    public void Heal()
    {
        player.hpHeal(6);
        SoundEffecter.playSFX("Heal");
    }
}
