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
    public RectTransform trashArea;
    public Animator trashAnimator;

    [Header("Assets")]
    public InputActionAsset bindings;
    public GameObject stickerPrefab;

    private List<UISticker> placedStickers = new List<UISticker>();

    private bool _manipulatorControlsInUse = false;
    private Vector2 _dragPosition;
    private bool _wasDraggingSticker = false;

    public override void OnClickDown(Vector2 screenPoint)
    {
        _manipulatorControlsInUse = false;
        //Return if a drag control was touched on the sticker manipulator.
        if (stickerManipulator.PointInManipulatorControls(screenPoint))
        {
            _manipulatorControlsInUse = true;
            return;
        }

        TargetSticker(null);

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
                //Check if selected sticker was actually selected
                //(This ensures that a transparent pixel wasn't selected)
                if (placedSticker.ValidScreenPoint(screenPoint))
                {

                    //Target new sticker
                    TargetSticker(placedSticker);

                    //Set drag position to the current screen point.
                    //This allows for dragging without snapping
                    _dragPosition = screenPoint;

                    return;
                }
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

        DragSticker(screenPoint);
    }

    public override void OnClickUp(Vector2 screenPoint)
    {
        if(_wasDraggingSticker)
        {
            //Close trash animator
            trashAnimator.SetBool("Open", false);

            if(stickerManipulator.targetSticker != null && RectTransformUtility.RectangleContainsScreenPoint(trashArea, _dragPosition))
            {
                Destroy(stickerManipulator.targetSticker.gameObject);
                TargetSticker(null);
            }

            _wasDraggingSticker = false;
        }
    }

    private void TargetSticker(UISticker placedSticker)
    {
        if(placedSticker == null)
        {
            //Reset the sticker manipulator's target.
            stickerManipulator.targetSticker = null;

            //Reset trash animator
            trashAnimator.SetBool("Show", false);
            return;
        }

        //Move the sticker to the top.
        placedSticker.transform.SetAsLastSibling();

        //Update sticker manipulator with new target.
        stickerManipulator.targetSticker = placedSticker.GetComponent<RectTransform>();

        //Show trash Icon
        trashAnimator.SetBool("Show", true);
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

    private void DragSticker(Vector2 screenPoint)
    {
        _wasDraggingSticker = false;
        if (stickerManipulator.targetSticker != null)
        {
            //Get the relative change from the previous frame to the current for pointer and sticker position.
            Vector2 _dragDeltaPosition = screenPoint - _dragPosition;

            //Drag sticker to new position
            stickerManipulator.targetSticker.position += (Vector3)_dragDeltaPosition;

            //Update drag position
            _dragPosition = screenPoint;

            //Update trash animator
            trashAnimator.SetBool("Open", RectTransformUtility.RectangleContainsScreenPoint(trashArea, screenPoint));

            _wasDraggingSticker = true;
        }
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
