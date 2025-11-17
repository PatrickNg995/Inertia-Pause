using System;
using UnityEngine;

public class LevelSelectPresenter : MonoBehaviour
{
    public Action OnMenuClose;

    [Header("View")]
    [SerializeField] private LevelSelectView _view;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void OpenMenu()
    {
        _view.gameObject.SetActive(true);
    }

    public void CloseMenu()
    {
        _view.gameObject.SetActive(false);
        OnMenuClose?.Invoke();
    }
}
