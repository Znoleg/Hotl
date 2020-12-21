using UnityEngine;

public class MinimapUI : GroupedUI
{
    private RectTransform _rect;
    [SerializeField] private RectTransform _imageUI;
    [SerializeField] private float _aspectRatio = 1f;
    [SerializeField] [Range(0f, 100f)] private int _offsetPercentage = 10;

    private MinMaxPair _boundsX;
    private MinMaxPair _boundsY;
    private Vector2 _sizeInScreen;

    private bool _isMinimapOpened;

    public override void Activate()
    {
        base.Activate();
        _isMinimapOpened = true;
        _imageUI.gameObject.SetActive(_isMinimapOpened);
    }

    public override void Deactivate()
    {
        base.Deactivate();
        _isMinimapOpened = false;
        _imageUI.gameObject.SetActive(_isMinimapOpened);
    }

    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
    }

    private void Start()
    {
        var rectRealSize = ScreenManager.Instance.RealScreenSize;
        var width = rectRealSize.x;
        var height = rectRealSize.y;
        var ratio = width / height;
        var minimapSizeRatio = 1f - _offsetPercentage * 2 / 100f;
        float minimapWidth;
        float minimapHeight;
        if (_aspectRatio > ratio)
        {
            minimapWidth = width * minimapSizeRatio;
            minimapHeight = minimapWidth / _aspectRatio;
        }
        else
        {
            minimapHeight = height * minimapSizeRatio;
            minimapWidth = minimapHeight * _aspectRatio;
        }
        var widthOffset = (width - minimapWidth) / 2;
        var heightOffset = (height - minimapHeight) / 2;

        _rect.SetLeft(widthOffset);
        _rect.SetRight(widthOffset);
        _rect.SetTop(heightOffset);
        _rect.SetBottom(heightOffset);

        _imageUI.SetLeft(widthOffset);
        _imageUI.SetRight(widthOffset);
        _imageUI.SetTop(heightOffset);
        _imageUI.SetBottom(heightOffset);

        rectRealSize = RectTransformUtility.CalculateRelativeRectTransformBounds(_rect).size;
        var xScale = UIManager.Instance.CanvasToRealResolutionScale.x;
        var yScale = UIManager.Instance.CanvasToRealResolutionScale.y;
        _sizeInScreen = new Vector2(rectRealSize.x * xScale, rectRealSize.y * yScale);
        _boundsX = new MinMaxPair(_rect.offsetMin.x * xScale, _rect.offsetMin.x * xScale + _sizeInScreen.x);
        _boundsY = new MinMaxPair(_rect.offsetMin.y * yScale, _rect.offsetMin.y * yScale + _sizeInScreen.y);
    }

    private void Update()
    {
        if (_isMinimapOpened && Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector3 touchPos = touch.position;
            if (!(touchPos.x >= _boundsX.Min && touchPos.x <= _boundsX.Max && touchPos.y >= _boundsY.Min && touchPos.y <= _boundsY.Max))
            {
                Deactivate();
            }
        }
    }
}