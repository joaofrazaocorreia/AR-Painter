using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class ARCursor : MonoBehaviour
{
    [SerializeField] private GameObject cursorChildObject;
    [SerializeField] private GameObject objectToPlace;
    [SerializeField] private ARRaycastManager raycastManager;
    [SerializeField] private bool useCursor = true;
    [SerializeField] private TextMeshProUGUI errorText;
    [SerializeField] private TextMeshProUGUI touchPosText;
    [SerializeField] private TextMeshProUGUI colorText;
    [SerializeField] private TextMeshProUGUI filteredColorText;
    [SerializeField] private Image colorImage;

    private void Start()
    {
        cursorChildObject.SetActive(useCursor);
    }

    private void Update()
    {
        try
        {
            if (useCursor)
            {
                UpdateCursor();
            }

            CheckForTouch();
        }

        catch (Exception e)
        {
            errorText.text = e.ToString();
        }
    }

    private void UpdateCursor()
    {
        Vector2 screenPosition = Camera.main.ViewportToScreenPoint(new Vector2(0.5f, 0.5f));
        List<ARRaycastHit> hits = new List<ARRaycastHit>();

        raycastManager.Raycast(screenPosition, hits, UnityEngine.XR.ARSubsystems.TrackableType.Planes);

        if (hits.Count > 0)
        {
            transform.position = hits[0].pose.position;
            transform.rotation = hits[0].pose.rotation;
        }
    }

    private void CheckForTouch()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            colorImage.color = Color.green;
            touchPosText.text = Input.GetTouch(0).position.ToString();
            errorText.text = $"spawned object at {transform.position}";

            if (useCursor)
            {
                Instantiate(objectToPlace, transform.position, transform.rotation);
            }

            else
            {
                List<ARRaycastHit> hits = new List<ARRaycastHit>();
                raycastManager.Raycast(Input.GetTouch(0).position, hits, UnityEngine.XR.ARSubsystems.TrackableType.Planes);

                if (hits.Count > 0)
                {
                    Instantiate(objectToPlace, hits[0].pose.position, hits[0].pose.rotation);
                }
            }
        }

        else
        {
            colorImage.color = Color.red;
        }
    }
}
