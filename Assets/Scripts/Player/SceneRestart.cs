using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class SceneRestart : MonoBehaviour
{
    // For player inputs
    private PlayerActions inputActions;
    private InputAction restart;
    
    void Awake()
    {
        // Bind restart input to function
        inputActions = new PlayerActions();
        restart = inputActions.Ingame.Restart;
    }

    // Enable & disable input actions
    private void OnEnable()
    {
        restart.performed += RestartScene;
        restart.Enable();
    }

    private void OnDisable()
    {
        restart.performed -= RestartScene;
        restart.Disable();
    }

    private void RestartScene(InputAction.CallbackContext context)
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
