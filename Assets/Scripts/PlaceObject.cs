using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using EnhancedTouch = UnityEngine.InputSystem.EnhancedTouch;

public class PlaceObject : MonoBehaviour
{
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


    private void FingerDown(EnhancedTouch.Finger finger)
    {
        try
        {
            if (finger.index != 0) return;

            touchDetectionImage.color = Color.green;

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
