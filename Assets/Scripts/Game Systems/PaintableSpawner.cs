using System.Collections;
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
    [SerializeField] private GameObject raycastCursorPrefab;

    private ARRaycastManager raycastManager;
    private ARPlaneManager planeManager;
    private GameManager gameManager;
    private List<ARRaycastHit> hits;
    private Vector2 middleScreenPosition;
    private ParticleSystem raycastHitCursor;
    private bool coutdownActive;
    private bool spawned;

    private void Start()
    {
        raycastManager = FindAnyObjectByType<ARRaycastManager>();
        planeManager = FindAnyObjectByType<ARPlaneManager>();
        gameManager = GetComponent<GameManager>();

        hits = new List<ARRaycastHit>();
        raycastHitCursor = Instantiate(raycastCursorPrefab).GetComponentInChildren<ParticleSystem>();
        Random.InitState((int)Time.time);
        spawned = false;
    }

    private void Update()
    {
        if (EnabledSpawning && !coutdownActive)
        {
            if (!raycastHitCursor.isPlaying)
            {
                raycastHitCursor.Play();
            }

            middleScreenPosition = Camera.main.ViewportToScreenPoint(new Vector2(0.5f, 0.5f));
            bool hitARPlane = raycastManager.Raycast(middleScreenPosition, hits, TrackableType.PlaneWithinPolygon);

            if (hitARPlane)
            {
                raycastHitCursor.transform.position = hits[0].pose.position;
                raycastHitCursor.transform.rotation = hits[0].pose.rotation;
            }

            gameManager.UIManager.ToggleSpawningPrompt(hitARPlane);
        }

        else
        {
            if (raycastHitCursor.isPlaying)
            {
                raycastHitCursor.Stop();
            }

            gameManager.UIManager.ToggleSpawningPrompt(false);
        }
    }

    public void SpawnObject()
    {
        if (EnabledSpawning && raycastManager.Raycast(middleScreenPosition, hits,
            TrackableType.PlaneWithinPolygon) && !spawned)
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
                    
                spawned = true;

                gameManager.AudioManager.PlayTapActionSFX();

                StartCoroutine(CountdownToStart());
            }
        }
    }

    private IEnumerator CountdownToStart()
    {
        float timer = 3f;
        float tickTimer = 0f;

        coutdownActive = true;
        gameManager.UIManager.UpdateTutorialText(0);

        while (timer > 0)
        {
            timer -= Time.deltaTime;
            gameManager.UIManager.UpdateCountdownText(timer);

            tickTimer -= Time.deltaTime;
            if (tickTimer <= 0)
            {
                tickTimer = 1f;
                gameManager.AudioManager.PlayGameTimerTickSFX();
            }

            yield return null;
        }

        gameManager.AudioManager.PlayGameTimerTickSFX(true);
        gameManager.AdvanceActionMode();
    }
}
