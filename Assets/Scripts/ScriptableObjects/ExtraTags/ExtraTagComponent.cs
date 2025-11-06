using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class ExtraTagComponent : MonoBehaviour
{
    [SerializeField]
    private List<ExtraTag> _tags;

    public bool HasTag (ExtraTag tag)
    {
        return _tags.Contains (tag);
    }

    public bool HasTag(string tagName)
    {
        return _tags.Exists(tagInList => tagInList.Name.Equals(tagName, System.StringComparison.InvariantCultureIgnoreCase));
    }
}
