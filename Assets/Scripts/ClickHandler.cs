using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class ClickHandler : MonoBehaviour
{
    public InputActionAsset inputBindings;

    /// <summary>
    /// Should clicks be polled for?
    /// </summary>
    protected bool pollClicks = true;

    private InputAction _point, _click;

    private bool _clickDown = false;

    public virtual void Awake()
    {
        UpdateActions();
    }

    public virtual void Update()
    {
        if (!pollClicks)
        {
            return;
        }

        Vector2 screenPoint = _point.ReadValue<Vector2>();

        if (_click.ReadValue<float>() == 0.0f)
        {
            if(_clickDown)
            {
                OnClickUp(screenPoint);
            }
            _clickDown = false;
        }
        else
        {
            if (!_clickDown)
            {
                OnClickDown(screenPoint);
                //Disable this from being called every frame!
                _clickDown = true;
            }
            OnClick(screenPoint);
        }
    }

    public virtual void OnClick(Vector2 screenPoint)
    {
    }

    public virtual void OnClickDown(Vector2 screenPoint)
    {
    }

    public virtual void OnClickUp(Vector2 screenPoint)
    {
    }

    /// <summary>
    /// Updates internal references to point and click actions.
    /// Needs to be called whenever the input configuration is changed.
    /// </summary>
    public void UpdateActions()
    {
        if (inputBindings != null)
        {
            _point = inputBindings.FindAction("UI/Point", true);
            _click = inputBindings.FindAction("UI/Click", true);
        }
    }
}
