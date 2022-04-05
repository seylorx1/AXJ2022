using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIActionPane : MonoBehaviour
{

    public float colorTransitionSpeed = 2.0f;
    public Color shapeBG, patternBG, colorBG;

    private Image image;
    private Color startColor;
    private Color targetColor;
    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
        startColor = image.color;
        targetColor = startColor;
    }

    // Update is called once per frame
    void Update()
    {
        image.color = Color.Lerp(image.color, targetColor, Time.deltaTime * colorTransitionSpeed);
    }

    public void ChangeScreen(string screen)
    {
        switch (screen)
        {
            case "Shape":
                targetColor = shapeBG;
                break;
            case "Pattern":
                targetColor = patternBG;
                break;
            case "Color":
                targetColor = colorBG;
                break;
            default:
                targetColor = startColor;
                break;
        }
    }
}
