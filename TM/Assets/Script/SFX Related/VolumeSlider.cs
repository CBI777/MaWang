using UnityEngine;
using UnityEngine.UI;
public class VolumeSlider : MonoBehaviour
{
    private void Awake()
    {
        this.GetComponent<Slider>().value = SoundEffecter.getVolume();
    }
}
