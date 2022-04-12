using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIActionPaneDesignerPage : UIActionPanePage
{
    [Header("UI Elements")]
    public RectTransform touchArea;
    public Transform stickersParent;
    public UIStickerManipulator stickerManipulator;

    [Header("Assets")]
    public InputActionAsset bindings;
    public GameObject stickerPrefab;

    private List<UISticker> placedStickers = new List<UISticker>();

    private bool _manipulatorControlsInUse = false;
    private Vector2 _dragPosition;

    public override void OnClickDown(Vector2 screenPoint)
    {
        _manipulatorControlsInUse = false;
        //Return if a drag control was touched on the sticker manipulator.
        if (stickerManipulator.PointInManipulatorControls(screenPoint))
        {
            _manipulatorControlsInUse = true;
            return;
        }

        //Reset the sticker manipulator's target.
        stickerManipulator.targetSticker = null;

        //Return if the sticker is outside the frame
        if (!RectTransformUtility.RectangleContainsScreenPoint(touchArea, screenPoint))
        {
            return;
        }

        foreach (UISticker placedSticker in placedStickers)
        {
            //Check if the point is touching ANY other sticker.
            if (RectTransformUtility.RectangleContainsScreenPoint(placedSticker.sticker.rectTransform, screenPoint))
            {
                //Move the sticker to the top.
                placedSticker.sticker.transform.SetAsLastSibling();

                //Set drag position to the current screen point.
                //This allows for dragging without snapping
                _dragPosition = screenPoint;

                //Update sticker manipulator with new target.
                stickerManipulator.targetSticker = placedSticker.sticker.rectTransform;
                return;
            }
        }

        PlaceSticker(screenPoint);
    }

    public override void OnClick(Vector2 screenPoint)
    {
        //Return if the sticker is outside the frame
        if (!RectTransformUtility.RectangleContainsScreenPoint(touchArea, screenPoint))
        {
            return;
        }

        //Don't move the element if a control is being used.
        if(_manipulatorControlsInUse)
        {
            return;
        }

        if (stickerManipulator.targetSticker != null)
        {
            //Get the relative change from the previous frame to the current for pointer and sticker position.
            Vector2 _dragDeltaPosition = screenPoint - _dragPosition;

            //Drag sticker to new position
            stickerManipulator.targetSticker.position += (Vector3)_dragDeltaPosition;

            //Update drag position
            _dragPosition = screenPoint;
        }
    }

    private void PlaceSticker(Vector2 point)
    {
        //Check the sticker doesn't already exist
        //Instantiate sticker!
        GameObject stickerInstance =
            Instantiate(stickerPrefab, new Vector3(point.x, point.y, 0.0f), Quaternion.identity, stickersParent);

        //Apply for the current sticker.
        UISticker sticker = stickerInstance.GetComponent<UISticker>();
        sticker.shape = actionPane.stickerPreview.shape;
        sticker.mask = actionPane.stickerPreview.mask;
        sticker.color = actionPane.stickerPreview.color;
        sticker.UpdateStickerInstance();

        //Add it to the list
        placedStickers.Add(sticker);
    }

    protected override void OnPageActiveChanged()
    {
        foreach(UISticker placedSticker in placedStickers)
        {
            Color c = placedSticker.sticker.color;
            c.a = PageActive ? 1.0f : 0.0f;
            placedSticker.sticker.color = c;

            placedSticker.UpdateStickerInstance();
        }

        base.OnPageActiveChanged();
    }
}
