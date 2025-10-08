using System;
using UnityEngine;

public class FramerateChecker : MonoBehaviour
{
    public Action<float, float> OnFramerateUpdate;

    [SerializeField][Range(0.01f, 5f)] private float _refreshRate = 1.0f;

    private int _framesRendered = 0;
    private float _timeElapsed = 0;


    // Update is called once per frame
    void Update()
    {
        if (_timeElapsed < _refreshRate)
        {
            _timeElapsed += Time.deltaTime;
            _framesRendered++;
        }
        else
        {
            OnFramerateUpdate?.Invoke(_framesRendered / _timeElapsed, 1f / (_framesRendered / _timeElapsed));
            _timeElapsed = Time.deltaTime;
            _framesRendered = 1;
        }
    }
}
