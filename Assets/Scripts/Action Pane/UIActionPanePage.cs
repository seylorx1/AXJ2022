using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIActionPanePage : ClickHandler
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

            //Only poll clicks if the page is active.
            pollClicks = _pageActive;
        }
    }

    protected virtual void OnPageActiveChanged()
    {

    }
}
