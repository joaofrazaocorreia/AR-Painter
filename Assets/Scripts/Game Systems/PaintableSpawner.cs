using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class PaintableSpawner : MonoBehaviour
{
    [Header("Main Variables")]
    private bool enabledSpawning;
    public bool EnabledSpawning { get => enabledSpawning; set { enabledSpawning = value; } }

    [SerializeField] private List<GameObject> prefabsToSpawn;
    [SerializeField] private GameObject raycastHitCursor;

    private ARRaycastManager raycastManager;
    private ARPlaneManager planeManager;
    private GameManager gameManager;
    private List<ARRaycastHit> hits;
    private Vector2 middleScreenPosition;

    private void Start()
    {
        raycastManager = FindAnyObjectByType<ARRaycastManager>();
        planeManager = FindAnyObjectByType<ARPlaneManager>();
        gameManager = GetComponent<GameManager>();

        hits = new List<ARRaycastHit>();
    }

    private void Update()
    {
        raycastHitCursor.SetActive(EnabledSpawning);
        middleScreenPosition = Camera.main.ViewportToScreenPoint(new Vector2(0.5f, 0.5f));

        if (raycastManager.Raycast(middleScreenPosition, hits, TrackableType.PlaneWithinPolygon))
        {
            raycastHitCursor.transform.position = hits[0].pose.position;
            raycastHitCursor.transform.rotation = hits[0].pose.rotation;
        }
    }

    public void SpawnObject()
    {
        if (EnabledSpawning && raycastManager.Raycast(middleScreenPosition, hits,
            TrackableType.PlaneWithinPolygon))
        {
            if (planeManager.GetPlane(hits[0].trackableId).alignment == PlaneAlignment.HorizontalUp)
            {
                Vector3 direction = Camera.main.transform.position - hits[0].pose.position;
                direction = Quaternion.LookRotation(direction).eulerAngles;
                direction = Vector3.Scale(direction, hits[0].pose.up.normalized); // (0, 1, 0)

                Quaternion faceCameraRotation = Quaternion.Euler(direction);

                int randomIndex = Random.Range(0, prefabsToSpawn.Count);
                GameObject chosenPaintable = prefabsToSpawn[randomIndex];

                gameManager.CurrentPaintable = Instantiate(chosenPaintable, hits[0].pose.position,
                    faceCameraRotation).GetComponent<PaintableObject>();

                gameManager.AdvanceActionMode();
            }
        }
    }
}
