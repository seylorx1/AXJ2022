using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class UIActionPaneSelection : UISticker
{
    #region Visible Variables
    public Image background;

    [Range(0.0f, 1.0f)]
    public float backgroundAlpha = 1.0f, foregroundAlpha = 1.0f;
    #endregion

    #region Hidden Variables
    public UIActionPanePage Page { private get; set; }

    private bool _allowInteraction = false;
    #endregion

    /// <summary>
    /// Reflects allow interactions with the sprite raycast target.
    /// </summary>
    public bool AllowInteractions
    {
        set
        {
            _allowInteraction = value;
            background.raycastTarget = value;
        }
    }

    /// <summary>
    /// Sets opacity, based on target alpha values.
    /// </summary>
    /// <param name="alpha">Automatically clamped between 0 and 1.</param>
    public void SetOpacity(float alpha)
    {
        //Clamp alpha between 0 and 1.
        alpha = Mathf.Clamp01(alpha);

        //Update background color.
        Color c = background.color;
        c.a = backgroundAlpha * alpha;
        background.color = c;

        //Update foreground color.
        c = sticker.color;
        c.a = foregroundAlpha * alpha;
        sticker.color = c;
    }

    public void Interact()
    {
        //If a page has been set, update the page's active selection with this component.
        if(Page != null && _allowInteraction)
        {
            Page.activeSelection = this;
            Page.actionPane.UpdateSticker();
        }
    }
}
