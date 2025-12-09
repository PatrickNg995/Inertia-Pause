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

    private void OnEnable()
    {
        _gameManager.OnAnyBlockingMenuOpen += DisablePlayerInput;
        _gameManager.OnAnyBlockingMenuClose += EnablePlayerInput;

        _gameManager.OnLevelComplete += _ => DisablePlayerInput();
    }

    private void OnDisable()
    {
        _gameManager.OnAnyBlockingMenuOpen -= DisablePlayerInput;
        _gameManager.OnAnyBlockingMenuClose -= EnablePlayerInput;

        _gameManager.OnLevelComplete -= _ => DisablePlayerInput();
    }

    private void DisablePlayerInput()
    {
        SetComponentEnabled(_playerMovement, false);
        SetComponentEnabled(_playerInteract, false);
        SetComponentEnabled(_playerLook, false);
        _timePauseUnpause.DisableTimeUnpause();
    }

    private void EnablePlayerInput()
    {
        SetComponentEnabled(_playerMovement, true);
        SetComponentEnabled(_playerInteract, true);
        SetComponentEnabled(_playerLook, true);
        _timePauseUnpause.EnableTimeUnpause();
    }

    private void SetComponentEnabled(MonoBehaviour component, bool enabled)
    {
        component.enabled = enabled;
    }
}
