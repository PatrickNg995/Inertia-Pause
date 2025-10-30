using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "NewTutorialInfo", menuName = "Inertia Pause/Tutorial Info")]
public class TutorialInfo : ScriptableObject
{
    public string ObjectName;
    public List<TutorialPage> Pages;
}

[Serializable]
public struct TutorialPage
{
    [TextArea(4, 10)] public string Content;
    public Sprite Image;
}