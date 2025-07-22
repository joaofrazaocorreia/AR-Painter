using UnityEngine;

public class ObjectPainter : MonoBehaviour
{
    private bool enabledPainting;
    public bool EnabledPainting { get => enabledPainting; set { enabledPainting = value; } }
    private Color currentColor;
    public Color CurrentColor { get => currentColor; set { currentColor = value; } }
    private GameManager gameManager;
    public bool IsFacingObject
    {
        get
        {
            Physics.Raycast(Camera.main.transform.position,
                Camera.main.transform.forward.normalized, out RaycastHit hit, 50f);

            return hit.transform != null &&
                    hit.transform.gameObject == gameManager.CurrentPaintable.gameObject;
        }
    }

    private void Start()
    {
        gameManager = GetComponent<GameManager>();
        CurrentColor = ColorLibrary.RGBColor(ColorLibrary.filteredColors[FilteredColors.None]);
    }

    private void Update()
    {
        gameManager.UIManager.TogglePaintingPrompt(IsFacingObject && EnabledPainting &&
            CurrentColor != ColorLibrary.RGBColor(ColorLibrary.filteredColors[FilteredColors.None]));
    }

    public void PaintObjectParts(int index)
    {
        if (IsFacingObject)
        {
            gameManager.CurrentPaintable.PaintParts(index, ColorLibrary.BinaryColor(
                CurrentColor.r, CurrentColor.g, CurrentColor.b));
                
            CurrentColor = ColorLibrary.RGBColor(ColorLibrary.filteredColors[FilteredColors.None]);
            gameManager.UIManager.TogglePaintingPrompt(false);

            gameManager.AdvanceActionMode();
        }
    }
}
