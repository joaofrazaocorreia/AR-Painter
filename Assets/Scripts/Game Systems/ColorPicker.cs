using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ColorPicker : MonoBehaviour
{
    [Header("Main Variables")]
    private bool enabledChecking;
    public bool EnabledChecking
    {
        get => enabledChecking;

        set
        {
            if (enabledChecking != value)
            {
                updateViewTimer = 0f;
                enabledChecking = value;
            }
        }
    }

    [SerializeField] private float updateViewRatePerSec = 6f;
    [SerializeField] private float colorFilterTreshold = 55f;
    [SerializeField] private float colorCollectingTime = 2f;

    private GameManager gameManager;
    private float updateViewTimer;
    private FilteredColors filteredCurrentColor;
    public FilteredColors FilteredCurrentColor { get => filteredCurrentColor; }

    [Header("Debug UI")]
    [SerializeField] private bool debug = false;

    // Runs at the start of the scene
    private void Start()
    {
        gameManager = GetComponent<GameManager>();

        EnabledChecking = true;
        updateViewTimer = 0f;
    }

    // Runs after Update()s
    private void LateUpdate()
    {
        if (EnabledChecking)
        {
            // Only updates the view a few times per second to reduce lag from screenshotting repeatedly
            if (updateViewTimer <= 0f)
            {
                updateViewTimer = 1 / updateViewRatePerSec;

                StartCoroutine(CheckViewColors(16));
            }

            else
            {
                updateViewTimer -= Time.deltaTime;
            }
        }

        else
        {
            gameManager.UIManager.ToggleColorPickingUI(false);
            gameManager.UIManager.ToggleColorPickingDebug(false);
            //gameManager.UIManager.UpdateColorCollectingFill(false);
        }
    }

    /// <summary>
    /// Coroutine that runs at the end of the frame and screenshots the current screen view as a Texture,
    /// then analyses the color of the middle pixels with the given radius.
    /// </summary>
    /// <returns></returns>
    private IEnumerator CheckViewColors(int pixelCheckingRadius)
    {
        // Screenshots the current camera view as a texture at the end of frame
        yield return new WaitForEndOfFrame();
        Texture2D texture = ScreenCapture.CaptureScreenshotAsTexture();

        float redAmount = 0f;
        float greenAmount = 0f;
        float blueAmount = 0f;
        int numOfPixels = 0;

        // Adds up the colors of the middle pixels area with the given radius
        for (int i = (texture.width / 2) - pixelCheckingRadius; i <= (texture.width / 2) + pixelCheckingRadius; i++)
        {
            for (int j = (texture.height / 2) - pixelCheckingRadius; j <= (texture.height / 2) + pixelCheckingRadius; j++)
            {
                Color pixelColor = texture.GetPixel(i, j);

                redAmount += pixelColor.r;
                greenAmount += pixelColor.g;
                blueAmount += pixelColor.b;

                numOfPixels++;
            }
        }

        // Calculates the average of the colors among all the pixels checked
        redAmount /= numOfPixels;
        greenAmount /= numOfPixels;
        blueAmount /= numOfPixels;
        Color averageColor = new Color(Mathf.Floor(redAmount * 255),
            Mathf.Floor(greenAmount * 255), Mathf.Floor(blueAmount * 255));

        // Updates the current filtered color
        filteredCurrentColor = FilterColor(averageColor);

        // Updates the color picking UI
        gameManager.UIManager.ToggleColorPickingUI(true);
        gameManager.UIManager.UpdateColorGoalUI(gameManager.CurrentColorGoal,
            filteredCurrentColor == gameManager.CurrentColorGoal);

        // Updates the debug UI
        gameManager.UIManager.ToggleColorPickingDebug(debug);
        gameManager.UIManager.UpdateColorPickingDebug(new Color(redAmount, greenAmount, blueAmount),
            averageColor, filteredCurrentColor);

        // Destroys the stored screenshot to avoid lag
        Destroy(texture);
    }

    /// <summary>
    /// Filters a given color by checking if its color is within the treshold of the filtered colors list.
    /// </summary>
    /// <param name="color">The given color to filter.</param>
    /// <returns>The filtered color if applicable; None if the color didn't match any filter.</returns>
    private FilteredColors FilterColor(Color color)
    {
        foreach (KeyValuePair<FilteredColors, (float, float, float)> kv in ColorLibrary.filteredColors)
        {
            if (CheckColor(color.r, ColorLibrary.RGBColor(kv.Value).r)
                && CheckColor(color.g, ColorLibrary.RGBColor(kv.Value).g)
                    && CheckColor(color.b, ColorLibrary.RGBColor(kv.Value).b))
            {
                return kv.Key;
            }
        }

        return FilteredColors.None;
    }

    /// <summary>
    /// Checks if a given color value is within the treshold of a given second value.
    /// </summary>
    /// <param name="colorValue">The color value to check.</param>
    /// <param name="checkValue">The value of the filtered color.</param>s
    /// <returns>True if the value is within the treshold, false if not.</returns>
    private bool CheckColor(float colorValue, float checkValue)
    {
        return Mathf.Abs(checkValue - colorValue) <= colorFilterTreshold;
    }

    public void StartCollectingColor()
    {
        StartCoroutine(HoldToCollectColor());
    }

    private IEnumerator HoldToCollectColor()
    {
        float collectingTimer = colorCollectingTime;

        while (EnabledChecking && gameManager.TouchManager.Touching &&
            FilteredCurrentColor == gameManager.CurrentColorGoal)
        {
            if (collectingTimer > 0)
            {
                collectingTimer -= Time.deltaTime;
                gameManager.UIManager.UpdateColorCollectingFill(true,
                    1 - (collectingTimer / colorCollectingTime));

                yield return null;
            }

            else
            {
                gameManager.ObjectPainter.CurrentColor = ColorLibrary.RGBColor(
                    ColorLibrary.filteredColors[filteredCurrentColor]);
                gameManager.AdvanceActionMode();
                break;
            }
        }

        gameManager.UIManager.UpdateColorCollectingFill(false);
    }
}
