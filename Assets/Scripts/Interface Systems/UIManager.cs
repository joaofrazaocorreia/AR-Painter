using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Game Manager HUD Elements")]
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI indexCycleTimerText;

    [Header("Color Picker HUD Elements")]
    [SerializeField] private GameObject colorCollectingBar;
    [SerializeField] private Image colorCollectingFill;
    [SerializeField] private TextMeshProUGUI currentColorText;
    [SerializeField] private TextMeshProUGUI filteredColorText;
    [SerializeField] private Image currentColorImage;
    [SerializeField] private TextMeshProUGUI currentColorGoalText;
    [SerializeField] private Image currentColorGoalImage;
    [SerializeField] private Image correctColorImage;

    [Header("Object Painter HUD Elements")]
    [SerializeField] private GameObject pressToPaintUI;
    
    [Header("Other HUD Elements")]
    [SerializeField] private TextMeshProUGUI errorText;

    [Header("Menus")]
    [SerializeField] private GameObject winScreen;
    [SerializeField] private GameObject lossScreen;



    private void Start()
    {
        indexCycleTimerText.gameObject.SetActive(false);
        ToggleColorPickingUI(false);
        pressToPaintUI.SetActive(false);
    }


    public void UpdateGameTimerText(float gameTimer)
    {
        int minutes = (int)Mathf.Floor(gameTimer / 60);
        int seconds = (int)Mathf.Floor(gameTimer % 60);

        timerText.text = $"{minutes}:";
        if (seconds < 10) timerText.text += "0";
        timerText.text += $"{seconds}";
    }

    public void UpdateColorCycleTimer(bool active, float timer = 0f)
    {
        indexCycleTimerText.gameObject.SetActive(active);
        indexCycleTimerText.text = $"Swapping in {Mathf.Ceil(timer)}...";
    }



    public void ToggleColorPickingUI(bool toggle)
    {
        currentColorGoalText.gameObject.SetActive(toggle);
        currentColorGoalImage.gameObject.SetActive(toggle);
        correctColorImage.gameObject.SetActive(toggle);
    }

    public void ToggleColorPickingDebug(bool toggle)
    {
        currentColorText.gameObject.SetActive(toggle);
        filteredColorText.gameObject.SetActive(toggle);
        currentColorImage.gameObject.SetActive(toggle);
    }

    public void UpdateColorCollectingFill(bool active, float fill = 0f)
    {
        colorCollectingBar.SetActive(active);
        colorCollectingFill.fillAmount = fill;
    }

    public void UpdateColorPickingDebug(Color currentColor, Color averageColor,
        FilteredColors filteredCurrentColor)
    {
        // Displays the color on the debug UI
        currentColorImage.color = currentColor;
        currentColorText.text = $"({averageColor.r}, {averageColor.g}, {averageColor.b})";
        filteredColorText.text = filteredCurrentColor.ToString();
    }

    public void UpdateColorGoalUI(FilteredColors currentColorGoal, bool colorMatch)
    {
        currentColorGoalText.text = $"Looking for: {currentColorGoal}";
        currentColorGoalImage.color = ColorLibrary.BinaryColor(ColorLibrary.filteredColors[currentColorGoal]);

        if (colorMatch)
            correctColorImage.color = Color.green;
        else
            correctColorImage.color = Color.red;
    }


    public void TogglePaintingPrompt(bool toggle)
    {
        pressToPaintUI.SetActive(toggle);
    }
}
