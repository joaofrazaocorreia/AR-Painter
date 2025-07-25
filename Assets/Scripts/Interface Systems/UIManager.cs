using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Game Manager HUD Elements")]
    [SerializeField] private GameObject crosshair;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private GameObject timerBackgroundUI;
    [SerializeField] private TextMeshProUGUI indexCycleTimerText;
    [SerializeField] private GameObject planeScanningTutorialText;
    [SerializeField] private GameObject paintableSpawnerTutorialText;
    [SerializeField] private GameObject colorPickerTutorialText;
    [SerializeField] private GameObject objectPainterTutorialText;
    [SerializeField] private GameObject tutorialBackgroundUI;
    [SerializeField] private bool permaToggleTutorialUI;
    private bool tutorialActive;

    [Header("Paintable Spawner HUD Elements")]
    [SerializeField] private GameObject tapToSpawnUI;
    [SerializeField] private TextMeshProUGUI countdownText;

    [Header("Color Picker HUD Elements")]
    [SerializeField] private GameObject colorCollectingBar;
    [SerializeField] private Image colorCollectingFill;
    [SerializeField] private TextMeshProUGUI currentColorText;
    [SerializeField] private TextMeshProUGUI currentColorGoalText;
    [SerializeField] private Image currentColorGoalImage;
    [SerializeField] private Image correctColorImage;
    [SerializeField] private GameObject holdToCollectUI;
    [SerializeField] private TextMeshProUGUI currentColorDebugText;
    [SerializeField] private TextMeshProUGUI filteredColorDebugText;
    [SerializeField] private Image currentColorDebugImage;

    [Header("Object Painter HUD Elements")]
    [SerializeField] private GameObject pressToPaintUI;

    [Header("Other HUD Elements")]
    [SerializeField] private TextMeshProUGUI errorText;

    [Header("Menus")]
    [SerializeField] private CanvasGroup loadingScreen;
    [SerializeField] private GameObject HUD;
    [SerializeField] private GameObject winScreen;
    [SerializeField] private GameObject loseScreen;


    private void Awake()
    {
        indexCycleTimerText.gameObject.SetActive(false);
        tutorialActive = true;
        ToggleColorPickingUI(false);
        UpdateColorCollectingFill(false);
        holdToCollectUI.SetActive(false);
        pressToPaintUI.SetActive(false);

        loadingScreen.blocksRaycasts = true;
        HUD.SetActive(true);
        winScreen.SetActive(false);
        loseScreen.SetActive(false);

        StartCoroutine(UnfadeScreen(loadingScreen, delay:1.5f));
    }


    public void LoadScene(int index)
    {
        StartCoroutine(LoadSceneCoroutine(index, loadingScreen, 3f));
    }

    public static IEnumerator LoadSceneCoroutine(int index, CanvasGroup loadingScreen, float speed = 1)
    {
        loadingScreen.alpha = 0f;
        loadingScreen.blocksRaycasts = true;
        loadingScreen.gameObject.SetActive(true);

        while (loadingScreen.alpha < 1)
        {
            loadingScreen.alpha += Time.fixedDeltaTime * speed;
            yield return null;
        }

        SceneManager.LoadScene(index);
    }

    public static IEnumerator UnfadeScreen(CanvasGroup loadingScreen, float speed = 1, float delay = 0f)
    {
        loadingScreen.alpha = 1f;
        loadingScreen.gameObject.SetActive(true);

        if(delay > 0) yield return new WaitForSeconds(delay);

        while (loadingScreen.alpha > 0)
        {
            loadingScreen.alpha -= Time.fixedDeltaTime * speed;
            yield return null;
        }

        loadingScreen.gameObject.SetActive(false);
        loadingScreen.blocksRaycasts = false;
    }

    /// <summary>
    /// Toggles the Tutorial text on the HUD based on the given index.
    /// </summary>
    /// <param name="tipIndex">0 = disable all tips; 1 = scanning tip; 
    /// 2 = spawning tip; 3 = color picking tip; 4 = painting tip.</param>
    public void UpdateTutorialText(int tipIndex)
    {
        tipIndex = Mathf.Clamp(tipIndex, 0, 4);

        tutorialBackgroundUI.SetActive(tipIndex != 0 && tutorialActive);
        planeScanningTutorialText.SetActive(tipIndex == 1 && tutorialActive);
        paintableSpawnerTutorialText.SetActive(tipIndex == 2 && tutorialActive);
        colorPickerTutorialText.SetActive(tipIndex == 3 && tutorialActive);
        objectPainterTutorialText.SetActive(tipIndex == 4 && tutorialActive);
    }

    public void ToggleTutorialText(bool toggle)
    {
        if (permaToggleTutorialUI)
        {
            tutorialActive = toggle;

            if (!toggle)
                UpdateTutorialText(0);
        }

        else
            UpdateTutorialText(0);
    }


    public void UpdateCountdownText(float timer)
    {
        countdownText.gameObject.SetActive(timer > 0);

        countdownText.text = Mathf.Ceil(timer).ToString();
    }


    public void UpdateGameTimerText(float gameTimer, bool active)
    {
        timerText.gameObject.SetActive(active);
        timerBackgroundUI.SetActive(active);

        if (active)
        {
            int minutes = (int)Mathf.Floor(gameTimer / 60);
            int seconds = (int)Mathf.Floor(gameTimer % 60);

            timerText.text = $"{minutes}:";
            if (seconds < 10) timerText.text += "0";
            timerText.text += $"{seconds}";

            if (minutes <= 0 && seconds <= 10)
            {
                timerText.color = Color.red;
            }
        }
    }

    public void UpdateColorCycleTimer(bool active, float timer = 0f)
    {
        indexCycleTimerText.gameObject.SetActive(active);
        indexCycleTimerText.text = $"Swapping in {Mathf.Ceil(timer)}...";
    }


    public void ToggleSpawningPrompt(bool toggle)
    {
        tapToSpawnUI.SetActive(toggle);
    }


    public void ToggleColorPickingUI(bool toggle)
    {
        currentColorText.gameObject.SetActive(toggle);
        currentColorGoalText.gameObject.SetActive(toggle);
        currentColorGoalImage.gameObject.SetActive(toggle);
        ToggleCrosshairs(!toggle);
    }

    public void ToggleColorPickingDebug(bool toggle)
    {
        currentColorDebugText.gameObject.SetActive(toggle);
        filteredColorDebugText.gameObject.SetActive(toggle);
        currentColorDebugImage.gameObject.SetActive(toggle);
    }

    public void ToggleCrosshairs(bool useDefault)
    {
        crosshair.SetActive(useDefault);
        correctColorImage.gameObject.SetActive(!useDefault);
    }

    public void UpdateColorCollectingFill(bool active, float fill = 0f)
    {
        colorCollectingBar.SetActive(active);
        colorCollectingFill.fillAmount = fill;
    }

    public void UpdateColorPickingDebug(Color currentColor, Color averageColor,
        List<FilteredColors> filteredCurrentColors)
    {
        // Displays the color on the debug UI
        currentColorDebugImage.color = currentColor;
        currentColorDebugText.text = $"({averageColor.r}, {averageColor.g}, {averageColor.b})";
        filteredColorDebugText.text = "";
        foreach (FilteredColors fc in filteredCurrentColors)
            filteredColorDebugText.text += fc.ToString() + "\n";
    }

    public void UpdateColorGoalUI(FilteredColors currentColorGoal, List<FilteredColors> filteredCurrentColors)
    {
        currentColorGoalText.text = $"Looking for: {currentColorGoal}";
        currentColorGoalImage.color = ColorLibrary.BinaryColor(ColorLibrary.filteredColors[currentColorGoal]);


        if (filteredCurrentColors.Contains(currentColorGoal))
        {
            currentColorText.text = currentColorGoal.ToString();
            correctColorImage.color = Color.green;
            holdToCollectUI.SetActive(true);
        }
        else
        {
            currentColorText.text = filteredCurrentColors[0].ToString();
            correctColorImage.color = Color.red;
            holdToCollectUI.SetActive(false);
        }
    }

    public void ToggleCollectingPrompt(bool toggle)
    {
        holdToCollectUI.SetActive(toggle);
    }


    public void TogglePaintingPrompt(bool toggle)
    {
        pressToPaintUI.SetActive(toggle);
    }


    public void ToggleGameOverScreen(bool win)
    {
        HUD.SetActive(false);
        winScreen.SetActive(win);
        loseScreen.SetActive(!win);
    }
}
