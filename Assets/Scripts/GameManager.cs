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
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI indexCycleTimerText;

    private PlayerActionMode playerActionMode;
    public int NumOfColors { get => numOfColors; }
    private TouchManager touchManager;
    public TouchManager TouchManager { get => touchManager; }
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
        colorPicker = GetComponent<ColorPicker>();
        paintableSpawner = GetComponent<PaintableSpawner>();
        objectPainter = GetComponent<ObjectPainter>();
        currentPaintable = null;
        currentColorIndex = 0;
        incompleteGoalIndexes = new List<int>();
        indexCycleTimer = timePerColor;
        indexCycleTimerText.gameObject.SetActive(false);

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
        UpdateTimerText();
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
        indexCycleTimerText.gameObject.SetActive(false);
        
        if (gameTimer > 0)
        {
            if (playerActionMode != PlayerActionMode.PaintableSpawning &&
                playerActionMode != PlayerActionMode.Finished)
            {
                gameTimer = Mathf.Max(gameTimer - Time.deltaTime, 0f);
                UpdateTimerText();
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
                    indexCycleTimerText.text = $"Swapping in {Mathf.Ceil(indexCycleTimer)}...";
                    indexCycleTimerText.gameObject.SetActive(incompleteGoalIndexes.Count > 1 && indexCycleTimer <= 10);
                }
            }
        }

        else
        {
            FinishGame(victory: false);
        }
    }

    private void UpdateTimerText()
    {
        int minutes = (int)Mathf.Floor(gameTimer / 60);
        int seconds = (int)Mathf.Floor(gameTimer % 60);

        timerText.text = $"{minutes}:";
        if (seconds < 10) timerText.text += "0";
        timerText.text += $"{seconds}";
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
            while (loop < 100)
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
