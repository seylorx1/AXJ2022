using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class UISticker : MonoBehaviour
{
    [Header("Sticker Instance Settings")]
    public Sprite shape;
    public Texture2D mask;
    public Color color;

    [Header("UI Settings")]
    public Material maskedMaterial;
    public Image sticker;

    private Material _mat;

    private void Awake()
    {
        UpdateStickerInstance();
    }

    private void OnValidate()
    {
        UpdateStickerInstance();
    }

    public void UpdateStickerInstance()
    {
        //Change foreground sprite.
        sticker.sprite = shape;

        //Change foreground color
        Color c = sticker.color;
        c = color;
        c.a = sticker.color.a; //Ignore changes in alpha values.
        sticker.color = c;

        //Create new material, set mask, and reapply.
        _mat = Instantiate(maskedMaterial);
        _mat.name = "Foreground Material Instance";
        _mat.SetTexture("_MaskTex", mask);
        sticker.material = _mat;
    }

    public bool ValidScreenPoint(Vector2 screenPoint)
    {
        Vector2 localPoint;

        if(RectTransformUtility.ScreenPointToLocalPointInRectangle(sticker.rectTransform, screenPoint, null, out localPoint))
        {
            //remap local point so values reflect UV coordinates
            localPoint.x += sticker.rectTransform.rect.width / 2.0f;
            localPoint.x /= sticker.rectTransform.rect.width;
            localPoint.y += sticker.rectTransform.rect.height / 2.0f;
            localPoint.y /= sticker.rectTransform.rect.height;

            //Get texture position
            Vector2 texPos = localPoint;
            texPos.x *= shape.texture.width;
            texPos.y *= shape.texture.height;

            //Get alpha from texture position on shape
            float a = shape.texture.GetPixel((int)texPos.x, (int)texPos.y).a;

            return a > 0.1f;
        }

        return false;
    }
}
