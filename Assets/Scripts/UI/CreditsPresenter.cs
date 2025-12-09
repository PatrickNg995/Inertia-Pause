using System;
using System.Collections;
using UnityEngine;

public class CreditsPresenter : MonoBehaviour
{
    public Action OnCreditsComplete;
    public Action OnMenuClose;

    [SerializeField] private CreditsView _view;

    private const float SHOW_SLIDE_DURATION = 2f;
    private const float SLIDE_ONE_WAY_FADE_DURATION = 0.5f;

    private Coroutine _slideshowCoroutine;
    private WaitForSeconds _slideDelay = new (SHOW_SLIDE_DURATION);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _view.gameObject.SetActive(false);

        if (_view.BackButton != null)
        {
            _view.BackButton.Button.onClick.AddListener(CloseMenu);
        }
    }

    public void OpenMenu()
    {
        foreach (CanvasGroup slide in _view.Slides)
        {
            slide.alpha = 0f;
        }

        _view.gameObject.SetActive(true);
    }

    public void CloseMenu()
    {
        OnMenuClose?.Invoke();
        _view.gameObject.SetActive(false);
    }

    public void RunSlideshow()
    {
        if (_slideshowCoroutine != null)
        {
            StopCoroutine(_slideshowCoroutine);
        }

        OpenMenu();
        _slideshowCoroutine = StartCoroutine(Slideshow());
    }

    private IEnumerator Slideshow()
    {
        for (int i = 0; i < _view.Slides.Count; i++)
        {
            CanvasGroup currentSlide = _view.Slides[i];

            yield return CrossfadeSlide(currentSlide, isFadeIn: true);
            yield return _slideDelay;
            yield return CrossfadeSlide(currentSlide, isFadeIn: false);
        }

        OnCreditsComplete?.Invoke();
    }

    private IEnumerator CrossfadeSlide(CanvasGroup slide, bool isFadeIn)
    {
        float startingAlpha = isFadeIn ? 0f : 1f;
        float endingAlpha = isFadeIn ? 1f : 0f;
        float time = 0f;

        slide.alpha = startingAlpha;

        while (time < SLIDE_ONE_WAY_FADE_DURATION)
        {
            time += Time.deltaTime;
            slide.alpha = Mathf.Lerp(startingAlpha, endingAlpha, time / SLIDE_ONE_WAY_FADE_DURATION);
            yield return null;
        }

        slide.alpha = endingAlpha;
    }
}
