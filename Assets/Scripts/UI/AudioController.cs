using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioController : MonoBehaviour
{
    public Slider _musicSlider, _sfxSlider;  
    [SerializeField]
    private GameObject _music, _sfx;
    private AudioManager _am;

    private void Awake()
    {
        _am = AudioManager.Instance;
        if (_am == null)
        {
            Debug.Log("Tile could not find GameManager");
        }
    }
    public void ToggleMusic()
    {
        AudioManager.Instance.ToggleMusic();
        if(_am._musicMuted == false)
        {
            _am._musicMuted = true;
        } else
        {
            _am._musicMuted = false;
        }
    }
    public void ToggleSFX()
    {
        AudioManager.Instance.ToggleSFX();
        if (_am._sfxMuted == false)
        {
            _am._sfxMuted = true;
        }
        else
        {
            _am._sfxMuted = false;
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
    public void Update()
    {
        if (_am._musicMuted == false)
        {
            _music.gameObject.SetActive(false);
        }
        else
        {
            _music.gameObject.SetActive(true);
        }
        if (_am._sfxMuted == false)
        {
            _sfx.gameObject.SetActive(false);
        }
        else
        {
            _sfx.gameObject.SetActive(true);
        }
    }
  
}

