using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class UIStickerManipulator : MonoBehaviour
{
    [Header("UI Settings")]
    public float padding = 20.0f;
    public float fadeSpeed = 5.0f;
    public List<Image> manipulatorImages;

    [Header("Drag Controls")]
    public UIDragControl rotateDragControl;
    public UIDragControl scaleDragControlBL;
    public UIDragControl scaleDragControlBR;
    public UIDragControl scaleDragControlTL;
    public UIDragControl scaleDragControlTR;

    [HideInInspector]
    public RectTransform targetSticker = null;

    private RectTransform _rectTransform;

    private Vector3 _previousPosition;
    private Vector2 _previousSize;

    public bool PointInManipulatorControls(Vector2 screenPoint)
    {
        return
            RectTransformUtility.RectangleContainsScreenPoint( rotateDragControl.ControlRectTransform, screenPoint) ||
            RectTransformUtility.RectangleContainsScreenPoint(scaleDragControlBL.ControlRectTransform, screenPoint) ||
            RectTransformUtility.RectangleContainsScreenPoint(scaleDragControlBR.ControlRectTransform, screenPoint) ||
            RectTransformUtility.RectangleContainsScreenPoint(scaleDragControlTL.ControlRectTransform, screenPoint) ||
            RectTransformUtility.RectangleContainsScreenPoint(scaleDragControlTR.ControlRectTransform, screenPoint);
    }

    // Start is called before the first frame update
    void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        if(targetSticker != null)
        {
            Vector2 screenPoint = Vector2.zero;
            if(rotateDragControl.GetDrag(out screenPoint))
            {
                targetSticker.rotation = Quaternion.Euler(
                    0.0f,
                    0.0f,
                    Mathf.Atan2(screenPoint.y - targetSticker.position.y, screenPoint.x - targetSticker.position.x) * Mathf.Rad2Deg - 90.0f);
            }

            Vector2 relativeScreenPoint = Vector2.zero;
            Vector2 scaleInvert = -Vector2.one;
            bool scaled = false;

            if (scaleDragControlBL.GetRelativeDrag(out relativeScreenPoint))
            {
                scaled = true;
            }
            else if (scaleDragControlBR.GetRelativeDrag(out relativeScreenPoint))
            {
                scaleInvert.x *= -1.0f;
                scaled = true;
            }
            else if (scaleDragControlTL.GetRelativeDrag(out relativeScreenPoint))
            {
                scaleInvert.y *= -1.0f;
                scaled = true;
            }
            else if (scaleDragControlTR.GetRelativeDrag(out relativeScreenPoint))
            {
                scaleInvert *= -1.0f;
                scaled = true;
            }

            if(scaled)
            {
                targetSticker.position = _previousPosition + (Vector3)relativeScreenPoint * 0.5f;

                relativeScreenPoint *= scaleInvert;

                targetSticker.sizeDelta = _previousSize + (relativeScreenPoint * 1.5f);
            }
            else
            {
                _previousPosition = targetSticker.position;
                _previousSize = targetSticker.rect.size;
            }

            _rectTransform.position = targetSticker.position;
            _rectTransform.sizeDelta = targetSticker.rect.size + new Vector2(padding, padding);
            _rectTransform.rotation = targetSticker.rotation;
        }

        foreach (Image image in manipulatorImages)
        {
            Color c = image.color;
            c.a = Mathf.Lerp(c.a, targetSticker == null ? 0.0f : 1.0f, Time.deltaTime * fadeSpeed);
            image.color = c;
        }
    }
}