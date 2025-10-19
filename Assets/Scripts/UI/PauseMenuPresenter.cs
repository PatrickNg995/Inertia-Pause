using UnityEngine;

public class PauseMenuPresenter : MonoBehaviour
{
    [Header("View")]
    [SerializeField] private PauseMenuView _view;

    [Header("Models")]
    [SerializeField] private GameManager _gameManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CloseMenu();

        // TODO: Get scenario objectives here

        _view.ResumeButton.onClick.AddListener(OnResumePressed);
        _view.RestartButton.onClick.AddListener(OnRestartPressed);
        _view.OptionsButton.onClick.AddListener(OnOptionsPressed);
        _view.QuitScenarioButton.onClick.AddListener(OnQuitPressed);
        _view.BackButton.onClick.AddListener(OnBackPressed);

        _gameManager.OnPause += OpenMenu;
    }

    public void OpenMenu()
    {
        gameObject.SetActive(true);
        _view.ActionsTakenText.text = _gameManager.ActionCount.ToString();
    }

    public void CloseMenu()
    {
        gameObject.SetActive(false);
    }

    private void OnResumePressed()
    {
        CloseMenu();
    }

    private void OnRestartPressed()
    {
        // TODO: Add a popup here.
        AdditiveSceneManager.Instance.ReloadScenario();
    }

    private void OnOptionsPressed()
    {
        // TODO: Add options here.
    }

    private void OnQuitPressed()
    {
        // TODO: Add a popup here.
        AdditiveSceneManager.Instance.LoadMainMenu();
    }

    private void OnBackPressed()
    {
        OnResumePressed();
    }
}
