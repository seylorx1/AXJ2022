using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIActionPane : MonoBehaviour
{
    private enum SelectedPage
    {
        None,
        Shape,
        Pattern,
        Color
    }

    public float colorTransitionSpeed = 2.0f;
    public Button shapeButton, patternButton, colorButton;
    public UIActionPanePage shapePage, patternPage, colorPage;
    public Animator headingAnimator;
    public Image selectionIcon;

    private SelectedPage _currentPage;
    private Image _image;
    private Color _startColor;
    private Color _targetColor;

    // Start is called before the first frame update
    void Start()
    {
        _image = GetComponent<Image>();
        _startColor = _image.color;
        _targetColor = _startColor;

        shapeButton     .onClick.AddListener(() => ChangeScreen(SelectedPage.Shape));
        patternButton   .onClick.AddListener(() => ChangeScreen(SelectedPage.Pattern));
        colorButton     .onClick.AddListener(() => ChangeScreen(SelectedPage.Color));
    }

    // Update is called once per frame
    void Update()
    {
        _image.color = Color.Lerp(_image.color, _targetColor, Time.deltaTime * colorTransitionSpeed);
    }

    private void ChangeScreen(SelectedPage page)
    {
        //Go back to main page if a button is selected again.
        if(page == _currentPage)
        {
            //Deselect page enum.
            page = SelectedPage.None;

            //Make sure that no button is selected.
            GameObject selected = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
            if(selected == shapeButton.gameObject || selected == patternButton.gameObject || selected == colorButton.gameObject)
            {
                UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
            }
        }

        //Show / Hide heading based on whether the base page is selected or not.
        headingAnimator.SetBool("Show", page == SelectedPage.None);

        //Reset all pages.
        shapePage.pageSelected = false;
        patternPage.pageSelected = false;
        colorPage.pageSelected = false;

        //Update colors and current pages.
        switch (page)
        {
            case SelectedPage.Shape:
                _targetColor = shapeButton.colors.selectedColor;
                shapePage.pageSelected = true;
                break;
            case SelectedPage.Pattern:
                _targetColor = patternButton.colors.selectedColor;
                patternPage.pageSelected = true;
                break;
            case SelectedPage.Color:
                _targetColor = colorButton.colors.selectedColor;
                colorPage.pageSelected = true;
                break;
            default:
                _targetColor = _startColor;
                break;
        }

        //Update current page reference.
        _currentPage = page;
    }
}
