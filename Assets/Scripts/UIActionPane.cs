using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIActionPane : MonoBehaviour
{

    public float colorTransitionSpeed = 2.0f;
    public Color shapeBG, patternBG, colorBG;

    public Animator headingAnimator;

    private Image _image;
    private Color _startColor;
    private Color _targetColor;

    private string _lastScreen = "";
    // Start is called before the first frame update
    void Start()
    {
        _image = GetComponent<Image>();
        _startColor = _image.color;
        _targetColor = _startColor;
    }

    // Update is called once per frame
    void Update()
    {
        _image.color = Color.Lerp(_image.color, _targetColor, Time.deltaTime * colorTransitionSpeed);
    }

    public void ChangeScreen(string screen)
    {
        if(screen == _lastScreen)
        {
            //Same screen has been pressed.. close.
            screen = "";
        }

        headingAnimator.SetBool("Show", false);
        switch (screen)
        {
            case "Shape":
                _targetColor = shapeBG;
                break;
            case "Pattern":
                _targetColor = patternBG;
                break;
            case "Color":
                _targetColor = colorBG;
                break;
            default:
                _targetColor = _startColor;
                headingAnimator.SetBool("Show", true);
                break;
        }

        _lastScreen = screen;
    }
}
