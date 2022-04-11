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

    #region Visible Variables
    public UISticker stickerPreview;
    public Image selectionCircle;
    public float colorTransitionSpeed = 2.0f;
    public Animator headingAnimator;

    [Header("Shape Settings")]
    public Button shapeButton;
    public UIActionPanePage shapePage;

    [Header("Pattern Settings")]
    public Button patternButton;
    public UIActionPanePage patternPage;

    [Header("Color Settings")]
    public Button colorButton;
    public UIActionPanePage colorPage;
    #endregion

    #region Hidden Variables
    [HideInInspector]
    public bool pageTransitionLock = false;

    private SelectedPage _currentPage;
    private Image _image;
    private Color _startColor;
    private Color _targetColor;

    //These variables cover what's needed for the sticker:
    private Sprite _shape;
    private Texture2D _pattern;
    private Color _color;
    #endregion

    // Start is called before the first frame update
    private void Start()
    {
        _image = GetComponent<Image>();
        _startColor = _image.color;
        _targetColor = _startColor;

        shapeButton     .onClick.AddListener(() => ChangeScreen(SelectedPage.Shape));
        shapePage       .actionPane = this;

        patternButton   .onClick.AddListener(() => ChangeScreen(SelectedPage.Pattern));
        patternPage     .actionPane = this;

        colorButton     .onClick.AddListener(() => ChangeScreen(SelectedPage.Color));
        colorPage       .actionPane = this;
    }

    // Update is called once per frame
    private void Update()
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
        shapePage.PageActive = false;
        patternPage.PageActive = false;
        colorPage.PageActive = false;

        //Update colors and current pages.
        switch (page)
        {
            case SelectedPage.Shape:
                _targetColor = shapeButton.colors.selectedColor;
                shapePage.PageActive = true;
                break;
            case SelectedPage.Pattern:
                _targetColor = patternButton.colors.selectedColor;
                patternPage.PageActive = true;
                break;
            case SelectedPage.Color:
                _targetColor = colorButton.colors.selectedColor;
                colorPage.PageActive = true;
                break;
            default:
                _targetColor = _startColor;

                //Disable transition lock incase it's stuck
                pageTransitionLock = false;
                break;
        }

        //Update current page reference.
        _currentPage = page;

        //Update the selection circle to reflect current page.
        UpdateSelectionCircle();
    }

    public void UpdateSticker()
    {
        _shape = shapePage.activeSelection.shape;
        _pattern = patternPage.activeSelection.mask;
        _color = colorPage.activeSelection.color;

        //Update sticker preview
        stickerPreview.shape = _shape;
        stickerPreview.mask = _pattern;
        stickerPreview.color = _color;
        stickerPreview.UpdateStickerInstance();

        //Reflect sticker changes in children.
        //This is what makes the pages 'adaptive' as it were...
        shapePage.      ReflectStickerInChildren(null, _pattern, _color, true);
        patternPage.    ReflectStickerInChildren(_shape, null, _color, true);
        colorPage.      ReflectStickerInChildren(_shape, _pattern, _color, false);
    }

    public void UpdateSelectionCircle()
    {

    }
}
