using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Image backgroundImage;
    [SerializeField] private CanvasGroup loadingScreen;
    [SerializeField] private CanvasGroup fadeOutScreen;
    private int incrementingColorMode;

    private void Start()
    {
        loadingScreen.blocksRaycasts = false;
        loadingScreen.gameObject.SetActive(false);

        incrementingColorMode = 1;

        fadeOutScreen.gameObject.SetActive(true);
        StartCoroutine(UIManager.UnfadeScreen(fadeOutScreen));
    }

    private void Update()
    {
        RotateBG(2);
        RainbowifyBG(0.2f);
    }

    public void LoadScene(int index)
    {
        StartCoroutine(UIManager.LoadSceneCoroutine(index, loadingScreen, 3f));
    }

    private void RotateBG(float speed = 1f)
    {
        Vector3 increment = Vector3.forward * Time.fixedDeltaTime * speed;
        Vector3 newRotation = backgroundImage.transform.eulerAngles + increment;

        backgroundImage.transform.rotation = Quaternion.Euler(newRotation);
    }

    private void RainbowifyBG(float speed = 1f)
    {
        float red = backgroundImage.color.r;
        float green = backgroundImage.color.g;
        float blue = backgroundImage.color.b;

        switch (incrementingColorMode)
        {
            case 1:
                (red, blue) = CycleImageColor(red, blue, 0.625f, speed);
                break;
            case 2:
                (green, red) = CycleImageColor(green, red, 0.625f, speed);
                break;
            case 3:
                (blue, green) = CycleImageColor(blue, green, 0.625f, speed);
                break;
        }

        backgroundImage.color = new Color(red, green, blue);;
    }

    private (float, float) CycleImageColor(float colorToIncrement, float colorToDecrement,
        float minDecrement, float speed = 1f)
    {
        float newIncrementedColor = colorToIncrement;
        float newDecrementedColor = colorToDecrement;

        if (colorToIncrement < 1f || colorToDecrement > minDecrement)
        {
            newIncrementedColor = Mathf.Clamp(colorToIncrement + Time.deltaTime *
                speed, minDecrement, 1f);
            newDecrementedColor = Mathf.Clamp(colorToDecrement - Time.deltaTime *
                speed, minDecrement, 1f);
        }

        else
        {
            if (++incrementingColorMode > 3)
                incrementingColorMode = 1;
        }

        return (newIncrementedColor, newDecrementedColor);
    }
}
