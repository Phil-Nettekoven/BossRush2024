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
    private Player _playerObject;

    [SerializeField] private Text _HP, _MP, _Soul, _DMG, _ITurns;
    void Start()
    {
        _debugMenu.SetActive(false);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (!_showMenu)
            {
                _debugMenu.SetActive(true);
                _showMenu = true;
            }
            else
            {
                _debugMenu.SetActive(false);
                _showMenu = false;
            }
        }
        _HP.text = "Player HP: " + _playerObject._playerHP;
        _MP.text = "Player MP: " + _playerObject._playerMP;
        _Soul.text = "Player SOUL: " + _playerObject._playerSoul;
        _DMG.text = "Player DMG: " + _playerObject.getDamage();
        _ITurns.text = "ITurns: " + _playerObject._invincibleCounter;
    }
}
