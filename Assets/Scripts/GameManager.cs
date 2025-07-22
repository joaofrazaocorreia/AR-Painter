using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum PlayerActionMode { PaintableSpawning, ColorPicking, ObjectPainting, Finished }

    [SerializeField] [Min(1)] private int numOfColors = 6;
    [SerializeField] [Min(0)] private float timePerColor = 30f;
    [SerializeField] private TextMeshProUGUI timerText;

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
    public PaintableObject CurrentPaintable { get => currentPaintable;  set { currentPaintable = value; } }
    private float gameTimer;
    private int currentColorIndex;
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

        chosenColorGoals = new List<FilteredColors>();
        for (int i = 0; i < numOfColors; i++)
        {
            int randomIndex = Random.Range(1, ColorManager.filteredColors.Count);

            chosenColorGoals.Add(ColorManager.filteredColors.Keys.ElementAt(randomIndex));
            //chosenColorGoals.Add(FilteredColors.Black);
        }

        touchManager.OnFingerDown.AddListener(FingerDownAction);

        playerActionMode = PlayerActionMode.PaintableSpawning;
        UpdatePlayerAction();

        gameTimer = numOfColors * timePerColor;
        UpdateTimerText();
    }

    private void Update()
    {
        UpdatePlayerAction();
        UpdateTimer();
    }


    private void UpdatePlayerAction()
    {
        paintableSpawner.EnabledSpawning = playerActionMode == PlayerActionMode.PaintableSpawning;
        colorPicker.EnabledChecking = playerActionMode == PlayerActionMode.ColorPicking;
        objectPainter.EnabledPainting = playerActionMode == PlayerActionMode.ObjectPainting;
    }

    private void UpdateTimer()
    {
        if (gameTimer > 0)
        {
            if (playerActionMode != PlayerActionMode.PaintableSpawning &&
                playerActionMode != PlayerActionMode.Finished)
            {
                gameTimer = Mathf.Max(gameTimer - Time.deltaTime, 0f);
                UpdateTimerText();
            }
        }

        else
        {
            GameOver();
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
                    if (++currentColorIndex < numOfColors)
                    {
                        playerActionMode = PlayerActionMode.ColorPicking;
                    }

                    else
                    {
                        playerActionMode = PlayerActionMode.Finished;

                        Victory();
                    }

                    break;
                }

            default:
                {
                    break;
                }
        }
    }

    private void Victory()
    {

    }

    private void GameOver()
    {

    }
}
