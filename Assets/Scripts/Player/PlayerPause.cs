using UnityEngine;

public class PlayerPause : MonoBehaviour
{
    [Header("Game Manager")]
    [SerializeField] private GameManager _gameManager;

    [Header("Player Components")]
    [SerializeField] private NewPlayerMovement _playerMovement;
    [SerializeField] private PlayerInteract _playerInteract;
    [SerializeField] private PlayerLook _playerLook;
    [SerializeField] private TimePauseUnpause _timePauseUnpause;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _gameManager.OnGamePause += OnGamePause;
        _gameManager.OnGameResume += OnGameResume;
    }

    private void OnGamePause()
    {
        _playerMovement.enabled = false;
        _playerInteract.enabled = false;
        _playerLook.enabled = false;
        _timePauseUnpause.enabled = false;
    }

    private void OnGameResume()
    {
        _playerMovement.enabled = true;
        _playerInteract.enabled = true;
        _playerLook.enabled = true;
        _timePauseUnpause.enabled = true;
    }
}
