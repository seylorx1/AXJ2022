using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[ExecuteInEditMode]
public class UIActionPaneDesignerPage : UIActionPanePage
{
    public InputActionAsset bindings;
    public RectTransform touchArea;
    public GameObject stickerPrefab;

    //Input actions handling touch input.
    [HideInInspector]
    public InputAction _point, _click;

    private float _opacity = 1.0f;

    private List<UISticker> placedStickers = new List<UISticker>();

    private bool _clickDown = false;

#if UNITY_EDITOR
    private void OnEnable()
    {
        FindActions();
    }
    private void OnValidate()
    {
        FindActions();
    }

    private void FindActions()
    {
        if (bindings != null)
        {
            _point = bindings.FindAction("UI/Point", true);
            _click = bindings.FindAction("UI/Click", true);
        }
    }
#endif

    // Update is called once per frame
    void Update()
    {
        //Poll actions if this page is active.
        if (PageActive)
        {
            if (_click.ReadValue<float>() == 0.0f)
            {
                _clickDown = false;
            }
            else
            {
                if (!_clickDown)
                {
                    //Disable this from being called every frame!
                    _clickDown = true;

                    HandleStickerInput();
                }
            }
        }
    }

    private void HandleStickerInput()
    {
        Vector2 point = _point.ReadValue<Vector2>();
        //Return if the sticker is outside the frame
        if (!RectTransformUtility.RectangleContainsScreenPoint(touchArea, point))
        {
            return;
        }

        foreach(UISticker placedSticker in placedStickers)
        {
            //Check if the point is touching ANY other sticker.
            if(RectTransformUtility.RectangleContainsScreenPoint(placedSticker.sticker.rectTransform, point))
            {
                return;
            }
        }

        PlaceSticker(point);
    }

    private void PlaceSticker(Vector2 point)
    {
        //Check the sticker doesn't already exist
        //Instantiate sticker!
        GameObject stickerInstance =
            Instantiate(stickerPrefab, new Vector3(point.x, point.y, 0.0f), Quaternion.identity, transform);

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
