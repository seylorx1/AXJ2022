using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIActionPanePage : MonoBehaviour
{
    [HideInInspector]
    public UIActionPane actionPane;

    private bool _pageActive = false;

    /// <summary>
    /// If set, updates all page selections to allow interactions.
    /// </summary>
    public bool PageActive
    {
        get
        {
            return _pageActive;
        }

        set
        {
            _pageActive = value;
            OnPageActiveChanged();
        }
    }

    protected virtual void OnPageActiveChanged()
    {

    }
}
