using System.Collections;
using System.Collections.Generic;
using Enums;
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

    private float updateViewTimer;
    private Color currentColor;
    public Color CurrentColor { get => currentColor; }
    private FilteredColor filteredCurrentColor;
    public FilteredColor FilteredCurrentColor { get => filteredCurrentColor; }
    private Dictionary<(float, float, float), FilteredColor> filteredColors;

    [Header("Debug UI")]
    [SerializeField] private bool debug = false;
    [SerializeField] private TextMeshProUGUI currentColorText;
    [SerializeField] private TextMeshProUGUI filteredColorText;
    [SerializeField] private Image currentColorImage;

    // Runs at the start of the scene
    private void Start()
    {
        EnabledChecking = true;
        updateViewTimer = 0f;
        SetupFilteredColors();

        currentColorText.gameObject.SetActive(debug);
        filteredColorText.gameObject.SetActive(debug);
        currentColorImage.gameObject.SetActive(debug);
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

        // Adds up the colors of the middle pixel area with the given radius
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

        if (debug)
        {
            // Displays the color on the debug UI
            currentColorImage.color = new Color(redAmount, greenAmount, blueAmount);
            currentColorText.text = $"({averageColor.r}, {averageColor.g}, {averageColor.b})";
            filteredColorText.text = filteredCurrentColor.ToString();
        }

        // Destroys the stored screenshot to avoid lag
        Destroy(texture);
    }

    /// <summary>
    /// Filters a given color by checking if its color is within the treshold of the filtered colors list.
    /// </summary>
    /// <param name="color">The given color to filter.</param>
    /// <returns>The filtered color if applicable; None if the color didn't match any filter.</returns>
    private FilteredColor FilterColor(Color color)
    {
        foreach (KeyValuePair<(float, float, float), FilteredColor> kv in filteredColors)
        {
            if (CheckColor(color.r, kv.Key.Item1)
                && CheckColor(color.g, kv.Key.Item2)
                    && CheckColor(color.b, kv.Key.Item3))
            {
                return kv.Value;
            }
        }

        return FilteredColor.None;
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

    /// <summary>
    /// Sets up the filtered colors dictionary values.
    /// </summary>
    private void SetupFilteredColors()
    {
        filteredColors = new Dictionary<(float, float, float), FilteredColor>
        {
            { (200f, 0f, 0f), FilteredColor.Red },
            { (0f, 200f, 0f), FilteredColor.Green },
            { (0f, 0f, 200f), FilteredColor.Blue },
            { (200f, 200f, 0f), FilteredColor.Yellow },
            { (200f, 0f, 200f), FilteredColor.Pink },
            { (0f, 200f, 200f), FilteredColor.Cyan },
            { (200f, 200f, 200f), FilteredColor.White },
            { (0f, 0f, 0f), FilteredColor.Black },
            { (100f, 100f, 100f), FilteredColor.Grey },
            { (200f, 100f, 0f), FilteredColor.Orange },
            { (0f, 200f, 100f), FilteredColor.LimeGreen },
            { (100f, 0f, 200f), FilteredColor.Purple },
            { (100f, 100f, 0f), FilteredColor.Brown },
            { (100f, 0f, 0f), FilteredColor.DarkRed },
            { (0f, 100f, 0f), FilteredColor.DarkGreen },
            { (0f, 0f, 100f), FilteredColor.DarkBlue }
        };
    }
}
