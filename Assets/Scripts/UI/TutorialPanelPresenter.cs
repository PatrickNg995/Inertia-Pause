using UnityEngine;

public class TutorialPanelPresenter : MonoBehaviour
{
    [Header("View")]
    [SerializeField] private TutorialPanelView _view;

    [Header("Models")]
    [SerializeField] private PlayerInteract _playerInteractModel;

    private const string PAGE_NUMBER_FORMAT = "{0} / {1}";

    private TutorialInfo _currentTutorial;
    private int _currentPageIndex;

    void Start()
    {
        CloseMenu();

        _view.BackButton.onClick.AddListener(OnBackClicked);
        _view.PrevPageButton.onClick.AddListener(OnPrevClicked);
        _view.NextPageButton.onClick.AddListener(OnNextClicked);

        _playerInteractModel.OnTutorialOpen += OnTutorialOpen;
    }

    public void OpenMenu()
    {
        _view.gameObject.SetActive(true);
    }

    public void CloseMenu()
    {
        _view.gameObject.SetActive(false);
    }

    public void SetTutorial(TutorialInfo tutorial)
    {
        _currentTutorial = tutorial;
        _view.Header.text = tutorial.ObjectName;

        DisplayPage(0);
    }

    private void OnTutorialOpen(TutorialInfo tutorial)
    {
        if (tutorial == null)
        {
            Debug.LogWarning("This object does not have a tutorial!");
            return;
        }
        SetTutorial(tutorial);
        OpenMenu();
    }

    private void DisplayPage(int index)
    {
        _currentPageIndex = index;
        _view.ContentText.text = _currentTutorial.Pages[index].Content;
        _view.ContentImage.sprite = _currentTutorial.Pages[index].Image;

        bool isFirstPage = index == 0;
        bool isLastPage = index == _currentTutorial.Pages.Count - 1;

        _view.PrevPageButton.gameObject.SetActive(!isFirstPage);
        _view.NextPageButton.gameObject.SetActive(!isLastPage);

        _view.PageText.text = string.Format(PAGE_NUMBER_FORMAT, _currentPageIndex + 1, _currentTutorial.Pages.Count);
    }

    private void OnPrevClicked()
    {
        if (_currentPageIndex > 0)
        {
            DisplayPage(_currentPageIndex - 1);
        }
    }

    private void OnNextClicked()
    {
        if (_currentPageIndex < _currentTutorial.Pages.Count - 1)
        {
            DisplayPage(_currentPageIndex + 1);
        }
    }

    private void OnBackClicked()
    {
        CloseMenu();
    }
}
