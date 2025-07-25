using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum PlayerActionMode { PlaneScanning, PaintableSpawning, ColorPicking, ObjectPainting, Finished }

    [SerializeField][Min(0)] private float gameTime = 180f;
    [SerializeField][Min(1)] private int numOfColors = 6;
    [SerializeField][Min(0)] private float timePerColor = 30f;

    private PlayerActionMode playerActionMode;
    public int NumOfColors { get => numOfColors; }
    private TouchManager touchManager;
    public TouchManager TouchManager { get => touchManager; }
    private UIManager uiManager;
    public UIManager UIManager { get => uiManager; }
    private AudioManager audioManager;
    public AudioManager AudioManager { get => audioManager; }
    private PlaneScanner planeScanner;
    private PaintableSpawner paintableSpawner;
    private ColorPicker colorPicker;
    private ObjectPainter objectPainter;
    public ObjectPainter ObjectPainter { get => objectPainter; }
    private PaintableObject currentPaintable;
    public PaintableObject CurrentPaintable { get => currentPaintable; set { currentPaintable = value; } }
    public bool IsGameActive { get => playerActionMode == PlayerActionMode.ColorPicking ||
        playerActionMode == PlayerActionMode.ObjectPainting; }
    private float gameTimer;
    private float gameTimerTickTimer;
    private int currentColorIndex;
    private List<int> incompleteGoalIndexes;
    private float indexCycleTimer;
    private float colorTimerTickTimer;
    private List<FilteredColors> chosenColorGoals;
    public FilteredColors CurrentColorGoal { get => chosenColorGoals[currentColorIndex]; }

    private void Start()
    {
        touchManager = FindAnyObjectByType<TouchManager>();
        uiManager = FindAnyObjectByType<UIManager>();
        audioManager = FindAnyObjectByType<AudioManager>();

        planeScanner = GetComponent<PlaneScanner>();
        paintableSpawner = GetComponent<PaintableSpawner>();
        colorPicker = GetComponent<ColorPicker>();
        objectPainter = GetComponent<ObjectPainter>();

        currentPaintable = null;
        currentColorIndex = 0;
        incompleteGoalIndexes = new List<int>();
        indexCycleTimer = timePerColor;
        Random.InitState((int)Time.time);

        numOfColors = Mathf.Clamp(numOfColors, 1, ColorLibrary.filteredColors.Count() - 1);

        chosenColorGoals = new List<FilteredColors>();
        for (int i = 0; i < numOfColors; i++)
        {
            List<FilteredColors> availableColors = ColorLibrary.filteredColors.Keys.Where
                (c => !chosenColorGoals.Contains(c)).ToList();

            int randomIndex = Random.Range(1, availableColors.Count());
            FilteredColors randomColor = availableColors.ElementAt(randomIndex);
            //FilteredColors randomColor = FilteredColors.DarkGrey;

            chosenColorGoals.Add(randomColor);
            incompleteGoalIndexes.Add(i);
        }

        touchManager.OnFingerDown.AddListener(FingerDownAction);

        playerActionMode = PlayerActionMode.PlaneScanning;
        uiManager.UpdateTutorialText(1);
        UpdatePlayerAction();

        gameTimer = gameTime;
        uiManager.UpdateGameTimerText(gameTimer, IsGameActive);
    }

    private void Update()
    {
        UpdatePlayerAction();
        UpdateTimers();
    }


    private void UpdatePlayerAction()
    {
        planeScanner.EnabledScanning = playerActionMode == PlayerActionMode.PlaneScanning;
        paintableSpawner.EnabledSpawning = playerActionMode == PlayerActionMode.PaintableSpawning;
        colorPicker.EnabledChecking = playerActionMode == PlayerActionMode.ColorPicking;
        objectPainter.EnabledPainting = playerActionMode == PlayerActionMode.ObjectPainting;
    }

    private void UpdateTimers()
    {
        if (gameTimer > 0)
        {
            if (IsGameActive)
            {
                gameTimer = Mathf.Max(gameTimer - Time.deltaTime, 0f);
                uiManager.UpdateGameTimerText(gameTimer, IsGameActive);

                if (gameTimer <= 10)
                {
                    gameTimerTickTimer -= Time.deltaTime;
                    if (gameTimerTickTimer <= 0)
                    {
                        AudioManager.PlayGameTimerTickSFX(gameTimer < 1);
                        gameTimerTickTimer = 1f;
                    }
                }
            }

            if (playerActionMode == PlayerActionMode.ColorPicking)
            {
                if (incompleteGoalIndexes.Count > 1)
                {
                    if (indexCycleTimer < 0)
                    {
                        CycleColorIndex();
                        AudioManager.PlayColorTimerTickSFX(true);
                    }

                    else
                    {
                        indexCycleTimer -= Time.deltaTime;
                        uiManager.UpdateColorCycleTimer(indexCycleTimer <= 10, indexCycleTimer);

                        if (indexCycleTimer <= 10)
                        {
                            colorTimerTickTimer -= Time.deltaTime;
                            if (colorTimerTickTimer <= 0)
                            {
                                AudioManager.PlayColorTimerTickSFX();
                                colorTimerTickTimer = 1f;
                            }
                        }
                    }
                }
            }
        }

        else if(playerActionMode != PlayerActionMode.Finished)
        {
            uiManager.UpdateGameTimerText(0, false);
            uiManager.UpdateColorCycleTimer(false);
            FinishGame(victory: false);
        }
    }

    public void FingerDownAction()
    {
        if (paintableSpawner.EnabledSpawning)
        {
            paintableSpawner.SpawnObject();
        }

        else if (colorPicker.EnabledChecking)
        {
            colorPicker.StartCollectingColor();
        }

        else if (objectPainter.EnabledPainting)
        {
            objectPainter.PaintObjectParts(currentColorIndex);
        }
    }

    public void AdvanceActionMode()
    {
        switch (playerActionMode)
        {
            case PlayerActionMode.PlaneScanning:
                {
                    playerActionMode = PlayerActionMode.PaintableSpawning;
                    uiManager.UpdateTutorialText(2);
                    break;
                }
            case PlayerActionMode.PaintableSpawning:
                {
                    playerActionMode = PlayerActionMode.ColorPicking;
                    uiManager.UpdateTutorialText(3);
                    AudioManager.PlaySucessSFX();
                    AudioManager.SetMusicVolume(0.85f);
                    break;
                }

            case PlayerActionMode.ColorPicking:
                {
                    playerActionMode = PlayerActionMode.ObjectPainting;
                    uiManager.UpdateColorCycleTimer(false);
                    uiManager.UpdateTutorialText(4);
                    AudioManager.PlaySucessSFX();
                    break;
                }

            case PlayerActionMode.ObjectPainting:
                {
                    incompleteGoalIndexes.Remove(currentColorIndex);

                    if (incompleteGoalIndexes.Count > 0)
                    {
                        playerActionMode = PlayerActionMode.ColorPicking;
                        uiManager.UpdateTutorialText(3);
                        AudioManager.PlaySucessSFX();
                        CycleColorIndex();
                    }

                    else
                    {
                        uiManager.UpdateTutorialText(0);
                        currentPaintable.VictoryParticles();
                        FinishGame(victory: true);
                    }

                    break;
                }

            default:
                {
                    break;
                }
        }

        uiManager.ToggleCrosshairs(playerActionMode != PlayerActionMode.ColorPicking);
    }

    private void CycleColorIndex()
    {
        if (incompleteGoalIndexes.Contains(currentColorIndex + 1))
            currentColorIndex++;

        else
        {
            int loop = 0;
            while (loop < numOfColors * 2)
            {
                currentColorIndex++;

                if (currentColorIndex >= numOfColors)
                {
                    currentColorIndex = 0;
                }

                if (incompleteGoalIndexes.Contains(currentColorIndex))
                    break;

                else loop++;
            }
        }

        indexCycleTimer = timePerColor;
    }

    private void FinishGame(bool victory)
    {
        playerActionMode = PlayerActionMode.Finished;
        uiManager.ToggleGameOverScreen(victory);

        if (victory) AudioManager.PlayWinSFX();
        else AudioManager.PlayLoseSFX();
    }
}
