using UnityEngine;

public class ObjectPainter : MonoBehaviour
{
    [Header("Main Variables")]
    private bool enabledPainting;
    public bool EnabledPainting { get => enabledPainting; set { enabledPainting = value; } }
    private Color currentColor;
    public Color CurrentColor { get => currentColor; set { currentColor = value; } }
}
