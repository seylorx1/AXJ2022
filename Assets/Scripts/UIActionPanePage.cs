using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIActionPanePage : MonoBehaviour
{
    #region Visible Variables
    [Header("Selection Settings")]
    public Transform selectionsParent;

    public UIActionPaneSelection activeSelection = null;


    [Header("Transition Settings")]
    public float appearSpeed = 10.0f;

    [Range(0.0f, 1.0f)]
    public float appearOverlap = 0.5f;
    #endregion

    #region Hidden Variables

    [HideInInspector]
    public UIActionPane actionPane;

    private UIActionPaneSelection[] _pageSelections = null;

    private bool _pageActive = false;

    public float AlphaProgress { get; private set; } = 0.0f;
    private float _previousAlphaProgress = 0.0f;

    #endregion

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

            //Iterate through all selections and update AllowInteractions.
            if (_pageSelections != null)
            {
                for (int i = 0; i < _pageSelections.Length; i++)
                {
                    _pageSelections[i].AllowInteractions = _pageActive;
                }
            }
        }
    }

    public void ReflectStickerInChildren(Sprite shape, Texture2D pattern, Color color, bool updateColor)
    {
        for (int i = 0; i < _pageSelections.Length; i++)
        {
            if (shape != null)
            {
                _pageSelections[i].shape = shape;
            }

            if (pattern != null)
            {
                _pageSelections[i].mask = pattern;
            }

            if (updateColor)
            {
                _pageSelections[i].color = color;
            }

            _pageSelections[i].UpdateStickerInstance();
        }
    }
    private void Start()
    {
        _pageSelections = new UIActionPaneSelection[selectionsParent.childCount];
        for (int i = 0; i < selectionsParent.childCount; i++)
        {
            _pageSelections[i] = selectionsParent.GetChild(i).GetComponent<UIActionPaneSelection>();

            //Update selection reference.
            _pageSelections[i].Page = this;

            //Make all available selections invisible.
            _pageSelections[i].SetOpacity(0.0f);
        }
    }
    private void Update()
    {
        //Calculate alpha to allow for overlap...
        //This value is not bound between 0 and 1. Clamping occurs inside of SetOpacity method.
        AlphaProgress = Mathf.Clamp(
            AlphaProgress + (Time.deltaTime * appearSpeed * (PageActive && !actionPane.pageTransitionLock ? 1.0f : -1.0f)),
            0.0f,
            GetOffsetStart(_pageSelections.Length) + 1.0f);

        //Set transition lock if fading out, but disable on the frame alpha becomes 0.
        if (!PageActive)
        {
            if (AlphaProgress > 0.0f) //Every frame when visible.
            {
                actionPane.pageTransitionLock = true;
            }
            else if (AlphaProgress == 0.0f && _previousAlphaProgress > 0.0f) //One frame on disappear
            {
                actionPane.pageTransitionLock = false;
            }
        }

        //Track alpha progress per frame
        _previousAlphaProgress = AlphaProgress;

        //Apply opacities.
        for (int i = 0; i < _pageSelections.Length; i++)
        {
            //Auto-clamped in this method...
            _pageSelections[i].SetOpacity(AlphaProgress - GetOffsetStart(i));
        }
    }

    private float GetOffsetStart(int index)
    {
        return Mathf.Max(index - 1.0f, 0.0f) * appearOverlap;
    }
}
