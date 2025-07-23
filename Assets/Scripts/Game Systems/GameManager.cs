using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum PlayerActionMode { PaintableSpawning, ColorPicking, ObjectPainting, Finished }

    [SerializeField][Min(0)] private float gameTime = 180f;
    [SerializeField][Min(1)] private int numOfColors = 6;
    [SerializeField][Min(0)] private float timePerColor = 30f;

    private PlayerActionMode playerActionMode;
    public int NumOfColors { get => numOfColors; }
    private TouchManager touchManager;
    public TouchManager TouchManager { get => touchManager; }
    private UIManager uiManager;
    public UIManager UIManager { get => uiManager; }
    private PaintableSpawner paintableSpawner;
    public PaintableSpawner PaintableSpawner { get => paintableSpawner; }
    private ColorPicker colorPicker;
    public ColorPicker ColorPicker { get => colorPicker; }
    private ObjectPainter objectPainter;
    public ObjectPainter ObjectPainter { get => objectPainter; }
    private PaintableObject currentPaintable;
    public PaintableObject CurrentPaintable { get => currentPaintable; set { currentPaintable = value; } }
    private float gameTimer;
    private int currentColorIndex;
    private List<int> incompleteGoalIndexes;
    private float indexCycleTimer;
    private List<FilteredColors> chosenColorGoals;
    public FilteredColors CurrentColorGoal { get => chosenColorGoals[currentColorIndex]; }

    private void Start()
    {
        touchManager = FindAnyObjectByType<TouchManager>();
        uiManager = FindAnyObjectByType<UIManager>();
        colorPicker = GetComponent<ColorPicker>();
        paintableSpawner = GetComponent<PaintableSpawner>();
        objectPainter = GetComponent<ObjectPainter>();
        currentPaintable = null;
        currentColorIndex = 0;
        incompleteGoalIndexes = new List<int>();
        indexCycleTimer = timePerColor;

        numOfColors = Mathf.Clamp(numOfColors, 1, ColorLibrary.filteredColors.Count() - 1);

        chosenColorGoals = new List<FilteredColors>();
        for (int i = 0; i < numOfColors; i++)
        {
            List<FilteredColors> availableColors = ColorLibrary.filteredColors.Keys.Where
                (c => !chosenColorGoals.Contains(c)).ToList();

            int randomIndex = Random.Range(1, availableColors.Count());
            FilteredColors randomColor = availableColors.ElementAt(randomIndex);

            chosenColorGoals.Add(randomColor);
            incompleteGoalIndexes.Add(i);
        }

        touchManager.OnFingerDown.AddListener(FingerDownAction);

        playerActionMode = PlayerActionMode.PaintableSpawning;
        UpdatePlayerAction();

        gameTimer = gameTime;
        uiManager.UpdateGameTimerText(gameTimer);
    }

    private void Update()
    {
        UpdatePlayerAction();
        UpdateTimers();
    }


    private void UpdatePlayerAction()
    {
        paintableSpawner.EnabledSpawning = playerActionMode == PlayerActionMode.PaintableSpawning;
        colorPicker.EnabledChecking = playerActionMode == PlayerActionMode.ColorPicking;
        objectPainter.EnabledPainting = playerActionMode == PlayerActionMode.ObjectPainting;
    }

    private void UpdateTimers()
    {
        if (gameTimer > 0)
        {
            if (playerActionMode != PlayerActionMode.PaintableSpawning &&
                playerActionMode != PlayerActionMode.Finished)
            {
                gameTimer = Mathf.Max(gameTimer - Time.deltaTime, 0f);
                uiManager.UpdateGameTimerText(gameTimer);
            }

            if (playerActionMode == PlayerActionMode.ColorPicking)
            {
                if (indexCycleTimer < 0)
                {
                    CycleColorIndex();
                }

                else
                {
                    indexCycleTimer -= Time.deltaTime;
                    uiManager.UpdateColorCycleTimer(incompleteGoalIndexes.Count > 1
                        && indexCycleTimer <= 10, indexCycleTimer);
                }
            }
        }

        else
        {
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
            case PlayerActionMode.PaintableSpawning:
                {
                    playerActionMode = PlayerActionMode.ColorPicking;
                    break;
                }

            case PlayerActionMode.ColorPicking:
                {
                    playerActionMode = PlayerActionMode.ObjectPainting;
                    break;
                }

            case PlayerActionMode.ObjectPainting:
                {
                    incompleteGoalIndexes.Remove(currentColorIndex);

                    if (incompleteGoalIndexes.Count > 0)
                    {
                        playerActionMode = PlayerActionMode.ColorPicking;
                        CycleColorIndex();
                    }

                    else
                    {
                        FinishGame(victory:true);
                    }

                    break;
                }

            default:
                {
                    break;
                }
        }
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

        if (victory)
        {

        }

        else
        {

        }
    }
}
