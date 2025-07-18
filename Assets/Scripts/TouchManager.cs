using UnityEngine;
using UnityEngine.UI;
using EnhancedTouch = UnityEngine.InputSystem.EnhancedTouch;

public class TouchManager : MonoBehaviour
{
    [Header("Debug UI")]
    [SerializeField] private bool debug = false;
    [SerializeField] private Image touchDetectionImage;

    private ModelSpawner modelSpawner;
    private ColorPicker colorPicker;
    private bool touching;

    private void Start()
    {
        modelSpawner = GetComponent<ModelSpawner>();
        colorPicker = GetComponent<ColorPicker>();

        touchDetectionImage.gameObject.SetActive(debug);
        touching = false;
        touchDetectionImage.color = Color.red;
    }

    private void OnEnable()
    {
        EnhancedTouch.TouchSimulation.Enable();
        EnhancedTouch.EnhancedTouchSupport.Enable();
        EnhancedTouch.Touch.onFingerDown += FingerDown;
        EnhancedTouch.Touch.onFingerUp += FingerUp;
    }

    private void OnDisable()
    {
        EnhancedTouch.TouchSimulation.Disable();
        EnhancedTouch.EnhancedTouchSupport.Disable();
        EnhancedTouch.Touch.onFingerDown -= FingerDown;
        EnhancedTouch.Touch.onFingerUp -= FingerUp;
    }

    private void FingerDown(EnhancedTouch.Finger finger)
    {
        if (finger.index != 0) return;

        touching = true;
        touchDetectionImage.color = Color.green;

        if (modelSpawner.EnabledSpawning)
        {
            modelSpawner.SpawnObject();
        }

        else
        {
            //colorPicker.CollectColor();
        }
    }

    private void FingerUp(EnhancedTouch.Finger finger)
    {
        if (finger.index != 0) return;
        
        touching = false;
        touchDetectionImage.color = Color.red;
    }
}