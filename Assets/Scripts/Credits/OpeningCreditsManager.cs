using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OpeningCreditsManager : MonoBehaviour
{
    [SerializeField] private CreditsPresenter _presenter;

    private const string ADDITIVE_UI_SCENE_NAME = "AdditiveUI";

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(ShowOpeningCreditsAfterDelay());
        _presenter.OnCreditsComplete += LoadAdditiveUI;
    }

    private IEnumerator ShowOpeningCreditsAfterDelay()
    {
        // Wait one frame for the presenter to initialize.
        yield return null;
        _presenter.RunSlideshow();
    }

    private void LoadAdditiveUI()
    {
        SceneManager.LoadSceneAsync(ADDITIVE_UI_SCENE_NAME, LoadSceneMode.Single);
    }
}
