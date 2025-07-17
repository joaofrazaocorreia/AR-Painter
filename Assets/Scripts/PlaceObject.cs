using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using EnhancedTouch = UnityEngine.InputSystem.EnhancedTouch;

public class PlaceObject : MonoBehaviour
{
    private enum TouchInputAction {SpawnObject, DetectColor}

    [SerializeField] [EnumButtons] private TouchInputAction touchInputAction;
    [SerializeField] private GameObject raycastHitCursor;
    [SerializeField] private GameObject testSpawnPrefab;
    [SerializeField] private TextMeshProUGUI currentColorText;
    [SerializeField] private TextMeshProUGUI filteredColorText;
    [SerializeField] private TextMeshProUGUI errorText;
    [SerializeField] private Image currentColorImage;
    [SerializeField] private Image touchDetectionImage;

    private ARRaycastManager raycastManager;
    private ARPlaneManager planeManager;
    private List<ARRaycastHit> hits;
    private Vector2 middleScreenPosition;

    private void Start()
    {
        raycastManager = GetComponent<ARRaycastManager>();
        planeManager = GetComponent<ARPlaneManager>();
        hits = new List<ARRaycastHit>();

        touchDetectionImage.color = Color.red;
    }

    private IEnumerator CheckViewColors()
    {
        // Screenshots the current camera view and determines the average color of the middle 16x16 pixel area
        yield return new WaitForEndOfFrame();
        Texture2D texture = ScreenCapture.CaptureScreenshotAsTexture();

        int pixelsFromCenter = 16;

        float redAmount = 0f;
        float greenAmount = 0f;
        float blueAmount = 0f;
        int numOfPixels = 0;
        for (int i = (texture.width / 2) - pixelsFromCenter; i <= (texture.width / 2) + pixelsFromCenter; i++)
        {
            for (int j = (texture.height / 2) - pixelsFromCenter; j <= (texture.height / 2) + pixelsFromCenter; j++)
            {
                Color pixelColor = texture.GetPixel(i, j);

                redAmount += pixelColor.r;
                greenAmount += pixelColor.g;
                blueAmount += pixelColor.b;

                numOfPixels++;
            }
        }

        // Calculates the average of the colors among all the pixels
        redAmount /= numOfPixels;
        greenAmount /= numOfPixels;
        blueAmount /= numOfPixels;
        Color averageColor = new Color(redAmount, greenAmount, blueAmount);

        // Applies the color to the UI
        currentColorImage.color = averageColor;
        currentColorText.text = $"({averageColor.r * 255}, {averageColor.g * 255}, {averageColor.b * 255})";

        // Destroys the stored screenshot to avoid lag
        Destroy(texture);
    }

    private void OnEnable()
    {
        EnhancedTouch.TouchSimulation.Enable();
        EnhancedTouch.EnhancedTouchSupport.Enable();
        EnhancedTouch.Touch.onFingerDown += FingerDown;
        EnhancedTouch.Touch.onFingerUp += FingerUp;
        raycastHitCursor.SetActive(true);
    }

    private void OnDisable()
    {
        EnhancedTouch.TouchSimulation.Disable();
        EnhancedTouch.EnhancedTouchSupport.Disable();
        EnhancedTouch.Touch.onFingerDown -= FingerDown;
        EnhancedTouch.Touch.onFingerUp -= FingerUp;
        if(raycastHitCursor)
            raycastHitCursor.SetActive(false);
    }

    private void Update()
    {
        try
        {
            middleScreenPosition = Camera.main.ViewportToScreenPoint(new Vector2(0.5f, 0.5f));

            if (raycastManager.Raycast(middleScreenPosition, hits, TrackableType.PlaneWithinPolygon))
            {
                raycastHitCursor.transform.position = hits[0].pose.position;
                raycastHitCursor.transform.rotation = hits[0].pose.rotation;
            }
        }

        catch (Exception e)
        {
            errorText.text = e.ToString();
        }
    }

    public void LateUpdate()
    {
        StartCoroutine(CheckViewColors());
    }


    private void FingerDown(EnhancedTouch.Finger finger)
    {
        try
        {
            if (finger.index != 0) return;

            touchDetectionImage.color = Color.green;

            if (touchInputAction == TouchInputAction.SpawnObject)
            {
                //if (raycastManager.Raycast(finger.currentTouch.screenPosition, hits, TrackableType.PlaneWithinPolygon))
                if (raycastManager.Raycast(middleScreenPosition, hits, TrackableType.PlaneWithinPolygon))
                {
                    if (planeManager.GetPlane(hits[0].trackableId).alignment == PlaneAlignment.HorizontalUp)
                    {
                        Vector3 direction = Camera.main.transform.position - hits[0].pose.position;
                        direction = Quaternion.LookRotation(direction).eulerAngles;
                        direction = Vector3.Scale(direction, hits[0].pose.up.normalized); // (0, 1, 0)

                        Quaternion faceCameraRotation = Quaternion.Euler(direction);

                        Instantiate(testSpawnPrefab, hits[0].pose.position, faceCameraRotation);
                    }
                }
            }

            else
            {

            }
        }

        catch (Exception e)
        {
            errorText.text = e.ToString();
        }
    }

    private void FingerUp(EnhancedTouch.Finger finger)
    {
        try
        {
            if (finger.index != 0) return;

            touchDetectionImage.color = Color.red;
        }

        catch (Exception e)
        {
            errorText.text = e.ToString();
        }
    }
}
