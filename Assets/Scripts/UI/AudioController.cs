using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioController : MonoBehaviour
{
    public Slider _musicSlider, _sfxSlider;
    public bool _musicMuted, _sfxMuted;
    [SerializeField]
    private GameObject _music, _sfx;
    public void ToggleMusic()
    {
        AudioManager.Instance.ToggleMusic();
        if(_musicMuted == false)
        {
            _music.gameObject.SetActive(true);
            _musicMuted = true;
        } else
        {
            _music.gameObject.SetActive(false);
            _musicMuted = false;
        }
    }
    public void ToggleSFX()
    {
        AudioManager.Instance.ToggleSFX();
        if (_sfxMuted == false)
        {
            _sfx.gameObject.SetActive(true);
            _sfxMuted = true;
        }
        else
        {
            _sfx.gameObject.SetActive(false);
            _sfxMuted = false;
        }
    }
    public void MusicVolume()
    {
        AudioManager.Instance.MusicVolume(_musicSlider.value);
    }
    public void SFXVolume()
    {
        AudioManager.Instance.SFXVolume(_sfxSlider.value);
    }
}

