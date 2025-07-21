using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum PlayerActionMode { ModelSpawning, ColorPicking, Painting, Finished }

    [SerializeField] private float gameTime = 180f;
    [SerializeField] private TextMeshProUGUI timerText;

    private PlayerActionMode playerActionMode;
    public PlayerActionMode GameMode { get => playerActionMode; set { playerActionMode = value; } }
    private PaintableSpawner paintableSpawner;
    private ColorPicker colorPicker;
    private ObjectPainter objectPainter;
    private PaintableObject currentPaintable;
    public PaintableObject CurrentPaintable { get => currentPaintable;  set { currentPaintable = value; } }
    private float gameTimer;

    private void Start()
    {
        colorPicker = GetComponent<ColorPicker>();
        paintableSpawner = GetComponent<PaintableSpawner>();
        objectPainter = GetComponent<ObjectPainter>();

        playerActionMode = PlayerActionMode.ModelSpawning;
        UpdatePlayerAction();

        gameTimer = gameTime;
        UpdateTimerText();
    }

    private void Update()
    {
        UpdatePlayerAction();
        UpdateTimer();
    }


    private void UpdatePlayerAction()
    {
        paintableSpawner.EnabledSpawning = playerActionMode == PlayerActionMode.ModelSpawning;
        colorPicker.EnabledChecking = playerActionMode == PlayerActionMode.ColorPicking;
        objectPainter.EnabledPainting = playerActionMode == PlayerActionMode.Painting;
    }

    private void UpdateTimer()
    {
        if (gameTimer > 0)
        {
            if (playerActionMode != PlayerActionMode.ModelSpawning &&
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
            //colorPicker.StartCoroutine(CollectColor);
        }

        else if (objectPainter.EnabledPainting)
        {
            //PaintObjectParts();
        }
    }

    private void Victory()
    {

    }

    private void GameOver()
    {

    }
}
