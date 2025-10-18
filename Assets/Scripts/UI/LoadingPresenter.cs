using UnityEngine;

public class LoadingPresenter : MonoBehaviour
{
    [Header("View")]
    [SerializeField] private LoadingView _view;

    [Header("Models")]
    [SerializeField] private AdditiveSceneManager _sceneManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _sceneManager.OnStartLoad += ShowMenu;
        _sceneManager.OnEndLoad += HideMenu;
    }

    private void ShowMenu()
    {
        _view.gameObject.SetActive(true);
    }

    private void HideMenu()
    {
        _view.gameObject.SetActive(false);
    }
}
