using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class ImageZoomDrag : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler , IPointerDownHandler
{
    public Image targetImage;
    public Button disableChatImageBtn;
    public GameObject imagePnl;
    public static Action<bool, Texture> onImageClick;

    public RectTransform imageRect;
    public CanvasGroup imagePanel;
    public float zoomScale = 2.0f;
    public float zoomSpeed = 0.25f;
    public float dragThresholdPercentage = 0.5f;
    public float stringyEffectSpeed = 0.1f;

    private Vector3 originalScale;
    private Vector3 originalPosition;
    private float screenDragThreshold;
    private float currentScale = 1.0f;
    private float targetScale = 1.0f;
    private bool isZoomed = false;
    private bool isDraggingDown = false;

    private float doubleTapTime = 0.3f;
    private float lastTapTime = 0;

    private Camera mainCamera;

    void Start()
    {
        if (imageRect != null)
        {
            originalScale = imageRect.localScale;
            originalPosition = imageRect.localPosition;
        }

        mainCamera = Camera.main;
        screenDragThreshold = Screen.height * dragThresholdPercentage;

        onImageClick += EnableDisableChatImagePanel;
        disableChatImageBtn.onClick.AddListener(() => EnableDisableChatImagePanel(false));
    }

    private void OnDisable()
    {
        onImageClick -= EnableDisableChatImagePanel;
    }

    void EnableDisableChatImagePanel(bool active, Texture texture = null)
    {
        if (texture != null)
        {
            Sprite newSprite = Sprite.Create((Texture2D)texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            targetImage.sprite = newSprite;
        }
        imagePanel.gameObject.SetActive(active);
    }

    void Update()
    {
        if(!imagePanel.gameObject.activeSelf) imageRect.DOAnchorPos(new Vector2(0f, 0f), 0f);

        if (Mathf.Abs(currentScale - targetScale) > 0.01f && !isDraggingDown)
        {
            currentScale = Mathf.Lerp(currentScale, targetScale, zoomSpeed);
            imageRect.localScale = Vector3.one * currentScale;
        }
        else
        {
            currentScale = targetScale;
            imageRect.localScale = Vector3.one * currentScale;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (Time.time - lastTapTime < doubleTapTime)
        {
            ToggleZoom();
        }
        lastTapTime = Time.time;
        //if (Time.time - lastTapTime < doubleTapTime)
        //{
        //    // Get the tap position on screen
        //    Vector2 tapPosition = eventData.position;

        //    // Convert it to local position relative to the image
        //    Vector2 localTapPosition = imageRect.InverseTransformPoint(tapPosition);

        //    // Trigger zoom at the tap position
        //    ToggleZoom(localTapPosition);
        //}
        //lastTapTime = Time.time;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDraggingDown = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        //isDragging = true;

        if (currentScale > 1.0f)
        {
            imageRect.localPosition += new Vector3(eventData.delta.x, eventData.delta.y, 0);
            ClampToScreenBounds();
        }
        else
        {
            if (eventData.delta.y < 0) // Dragging downward
            {
                imageRect.localPosition += new Vector3(0, eventData.delta.y, 0);
                float alphaChange = Mathf.Clamp01(-imageRect.localPosition.y / screenDragThreshold);
                imagePanel.alpha = 1 - alphaChange;
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (currentScale > 1.0f)
        {
            isDraggingDown = false;
            return;
        }
        if (imageRect.localPosition.y < -screenDragThreshold)
        {
            isDraggingDown = false;
            DisableImagePanel();  // Ensure panel is hidden
        }
        else
        {
            // If not dragged beyond threshold, smoothly reset to original position
            imageRect.DOAnchorPos(originalPosition, 0.5f);
            imagePanel.DOFade(1f, 0.5f);
        }
    }

    public void OnPinchZoom(float delta)
    {
        if (imageRect != null)
        {
            targetScale = Mathf.Clamp(currentScale + delta * zoomSpeed, 1.0f, zoomScale);
            isZoomed = targetScale > 1.0f;

            // Smooth zooming
            currentScale = Mathf.Lerp(currentScale, targetScale, zoomSpeed);
            imageRect.localScale = Vector3.one * currentScale;

            // Ensure image doesn't overflow after zooming
            if (currentScale > 1.0f)
            {
                ClampToScreenBounds();
            }
        }
    }

    private void ToggleZoom()
    {
        if (isZoomed)
        {
            targetScale = 1.0f;
            imageRect.localPosition = originalPosition;
            isZoomed = false;
        }
        else
        {
            targetScale = zoomScale;
            isZoomed = true;
        }
    }

    //private void ToggleZoom(Vector2 tapPosition)
    //{
    //    if (isZoomed)
    //    {
    //        // Zoom out: reset to original scale and position
    //        targetScale = 1.0f;
    //        imageRect.localPosition = originalPosition;  // Return to original position
    //        isZoomed = false;
    //    }
    //    else
    //    {
    //        // Zoom in: set new zoom scale
    //        targetScale = zoomScale;
    //        isZoomed = true;

    //        // Convert tapPosition to Vector3
    //        Vector3 tapPosition3D = new Vector3(tapPosition.x, tapPosition.y, 0);

    //        // Calculate the offset of the tap relative to the image center
    //        Vector3 tapOffset = tapPosition3D - new Vector3(imageRect.rect.width / 2, imageRect.rect.height / 2, 0);

    //        // Adjust the position to zoom around the tap point
    //        Vector3 targetPosition = imageRect.localPosition - tapOffset * (zoomScale - 1);

    //        // Apply the new scale and position
    //        imageRect.localScale = Vector3.one * targetScale;
    //        imageRect.localPosition = targetPosition;
    //    }

    //    // Optional: Apply some smoothing for better UX
    //    imageRect.DOLocalMove(imageRect.localPosition, 0.3f);
    //    imageRect.DOScale(Vector3.one * targetScale, 0.3f);
    //}

    private void DisableImagePanel()
    {
        
        //imageRect.localPosition = originalPosition;
        imagePanel.gameObject.SetActive(false);
    }

    private void ClampToScreenBounds()
    {
        Vector3[] corners = new Vector3[4];
        imageRect.GetWorldCorners(corners);

        // Convert to screen space
        Vector3 bottomLeft = mainCamera.WorldToScreenPoint(corners[0]);
        Vector3 topRight = mainCamera.WorldToScreenPoint(corners[2]);

        // Image position relative to screen bounds
        Vector3 position = imageRect.localPosition;

        // Clamp position to prevent going out of bounds
        if (bottomLeft.x > 0)
            position.x -= bottomLeft.x;
        if (topRight.x < Screen.width)
            position.x += Screen.width - topRight.x;

        if (bottomLeft.y > 0)
            position.y -= bottomLeft.y;
        if (topRight.y < Screen.height)
            position.y += Screen.height - topRight.y;

        // Apply clamped position
        imageRect.localPosition = position;
    }

  
}
