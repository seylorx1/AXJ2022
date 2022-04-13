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

    private Vector2 _previousPosition;
    private Vector2 _previousScale;

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
            Vector2 selectedPoint = _previousScale / 2.0f;

            bool scaled = false;

            if (scaleDragControlBL.GetRelativeDrag(out relativeScreenPoint))
            {
                selectedPoint *= -1.0f;
                scaled = true;
            }
            else if (scaleDragControlTL.GetRelativeDrag(out relativeScreenPoint))
            {
                selectedPoint.x *= -1.0f;
                scaled = true;
            }
            else if (scaleDragControlTR.GetRelativeDrag(out relativeScreenPoint))
            {
                scaled = true;
            }
            else if (scaleDragControlBR.GetRelativeDrag(out relativeScreenPoint))
            {
                selectedPoint.y *= -1.0f;
                scaled = true;
            }

            if (scaled)
            {
                //Find the opposite point to the current.
                Vector2 oppositePoint = selectedPoint * -1.0f;

                //Rotate screen input based on the opposite sticker rotation.
                //This should orient the input from world to local space.
                Vector2 rotatedScreenPoint = relativeScreenPoint.Rotate(-targetSticker.rotation.eulerAngles.z);

                //Calculate new scale
                Vector2 scale = (selectedPoint * 2.0f) + rotatedScreenPoint;


                targetSticker.position = _previousPosition + (-selectedPoint + (scale / 2.0f)).Rotate(targetSticker.rotation.eulerAngles.z);

                //Constrain scale using absolute values and limit to a range.
                scale.x = Mathf.Max(Mathf.Abs(scale.x), 50.0f);
                scale.y = Mathf.Max(Mathf.Abs(scale.y), 50.0f);

                //Apply scale
                targetSticker.sizeDelta = scale;
            }
            else
            {
                _previousPosition = targetSticker.position;
                _previousScale = targetSticker.sizeDelta;
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
