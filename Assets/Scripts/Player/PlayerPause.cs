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
        _gameManager.OnAnyBlockingMenuOpen += DisablePlayerInput;
        _gameManager.OnAnyBlockingMenuClose += EnablePlayerInput;

        _gameManager.OnLevelComplete += _ => DisablePlayerInput();
    }

    private void DisablePlayerInput()
    {
        SetComponentEnabled(_playerMovement, false);
        SetComponentEnabled(_playerInteract, false);
        SetComponentEnabled(_playerLook, false);
        SetComponentEnabled(_timePauseUnpause, false);
    }

    private void EnablePlayerInput()
    {
        SetComponentEnabled(_playerMovement, true);
        SetComponentEnabled(_playerInteract, true);
        SetComponentEnabled(_playerLook, true);
        SetComponentEnabled(_timePauseUnpause, true);
    }

    private void SetComponentEnabled(MonoBehaviour component, bool enabled)
    {
        // We don't really need a null check here but since some components are missing in testing,
        // this is only to prevent NREs until the Player prefab is up.
        if (component != null)
        {
            component.enabled = enabled;
        }
    }
}
