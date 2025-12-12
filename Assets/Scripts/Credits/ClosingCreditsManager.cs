using System.Collections;
using UnityEngine;

public class ClosingCreditsManager : MonoBehaviour
{
    [SerializeField] private CreditsPresenter _presenter;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(ShowClosingCreditsAfterDelay());
        _presenter.OnCreditsComplete += LoadMainMenu;
        _presenter.OnMenuClose += LoadMainMenu;
    }

    private IEnumerator ShowClosingCreditsAfterDelay()
    {
        // Wait one frame for the presenter to initialize.
        yield return null;
        _presenter.RunSlideshow();
    }

    private void LoadMainMenu()
    {
        AdditiveSceneManager.Instance.LoadMainMenu();
    }
}
