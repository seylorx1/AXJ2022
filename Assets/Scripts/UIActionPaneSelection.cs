using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class UIActionPaneSelection : MonoBehaviour
{
    public Image background;
    public Image foreground;

    public Sprite shape;
    public Texture2D mask;

    public Material maskedMaterial;

    private Material _mat;

    [Range(0.0f, 1.0f)]
    public float backgroundAlpha = 1.0f, foregroundAlpha = 1.0f;

    private void Awake()
    {
        UpdateTextures();
    }

    private void OnValidate()
    {
        UpdateTextures();
    }

    private void UpdateTextures()
    {
        //Change foreground sprite.
        foreground.sprite = shape;

        //Create new material, set mask, and reapply.
        _mat = Instantiate(maskedMaterial);
        _mat.name = "Foreground Material Instance";
        _mat.SetTexture("_MaskTex", mask);
        foreground.material = _mat;
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
        c = foreground.color;
        c.a = foregroundAlpha * alpha;
        foreground.color = c;
    }
    
}
