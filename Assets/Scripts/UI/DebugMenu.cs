using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class DebugMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject _debugMenu;
    private bool _showMenu;
    [SerializeField]
    private PlayerStats _playerStats;
    [SerializeField]
    private Text _HP, _MP, _Soul, _DMG;
    void Start()
    {
        _debugMenu.SetActive(false);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (!_showMenu) {
                _debugMenu.SetActive(true);
                _showMenu = true;
        } else
            {
                _debugMenu.SetActive(false);
                _showMenu = false;
            }
        }
        _HP.text = "Player HP: " + _playerStats._playerHP;
        _MP.text = "Player MP: " + _playerStats._playerMP; 
        _Soul.text = "Player SOUL: " + _playerStats._playerSoul;
        _DMG.text = "Player DMG: " + _playerStats._playerDmg;
    }
}
