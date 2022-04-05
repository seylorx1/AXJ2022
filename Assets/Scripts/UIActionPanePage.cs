using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIActionPanePage : MonoBehaviour
{
    public Transform selectionsParent;

    [Header("Transition Settings")]
    public float appearSpeed = 10.0f;

    [Range(0.0f, 1.0f)]
    public float appearOverlap = 0.5f;

    [HideInInspector]
    public bool pageSelected = false;

    private UIActionPaneSelection[] _pageSelections;
    private float _alphaProgress = 0.0f;

    private void Start()
    {
        _pageSelections = new UIActionPaneSelection[selectionsParent.childCount];
        for(int i = 0; i < selectionsParent.childCount; i++)
        {
            _pageSelections[i] = selectionsParent.GetChild(i).GetComponent<UIActionPaneSelection>();

            //Make all available selections invisible.
            _pageSelections[i].SetOpacity(0.0f);
        }
    }
    private void Update()
    {
        _alphaProgress = Mathf.Clamp(
            _alphaProgress + (Time.deltaTime * appearSpeed * (pageSelected ? 1.0f : -1.0f)),
            0.0f,
            GetOffsetStart(_pageSelections.Length) + 1.0f);
        
        for(int i = 0; i < _pageSelections.Length; i++)
        {
            _pageSelections[i].SetOpacity(_alphaProgress - GetOffsetStart(i));
        }
    }

    private float GetOffsetStart(int index)
    {
        return Mathf.Max(index - 1.0f, 0.0f) * appearOverlap;
    }
}
