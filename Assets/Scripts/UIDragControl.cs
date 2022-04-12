using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(RectTransform))]
public class UIDragControl : ClickHandler
{
    [HideInInspector]
    public RectTransform ControlRectTransform { get; private set; }

    private Vector2 _firstTouchPoint;
    private Vector2 _pointerScreenPoint;

    private bool _pointerCaptured = false;

    private void Start()
    {
        ControlRectTransform = GetComponent<RectTransform>();
        ResetPointer();
    }

    public override void OnClickDown(Vector2 screenPoint)
    {
        if (RectTransformUtility.RectangleContainsScreenPoint(ControlRectTransform, screenPoint))
        {
            _firstTouchPoint = screenPoint;

            _pointerCaptured = true;
        }
    }

    public override void OnClick(Vector2 screenPoint)
    {
        if(_pointerCaptured)
        {
            _pointerScreenPoint = screenPoint;
        }
    }

    public override void OnClickUp(Vector2 screenPoint)
    {
        ResetPointer();
    }

    /// <summary>
    /// Resets pointer data. 
    /// </summary>
    private void ResetPointer()
    {
        _pointerCaptured = false;
        _pointerScreenPoint = ControlRectTransform.position;
    }

    /// <summary>
    /// Gets the pointer position relative to the marquee element.
    /// </summary>
    /// <param name="relativePointerPosition"></param>
    /// <returns>UI element is selected.</returns>
    public bool GetRelativeDrag (out Vector2 relativeScreenPoint)
    {
        relativeScreenPoint = _pointerScreenPoint - _firstTouchPoint;
        return _pointerCaptured;
    }

    /// <summary>
    /// Gets the pointer position.
    /// </summary>
    /// <param name="relativePointerPosition"></param>
    /// <returns>UI element is selected.</returns>
    public bool GetDrag(out Vector2 screenPoint)
    {
        screenPoint = _pointerScreenPoint;
        return _pointerCaptured;
    }
}
