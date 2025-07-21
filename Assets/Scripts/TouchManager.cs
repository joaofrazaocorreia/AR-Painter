using UnityEngine;
using UnityEngine.UI;
using EnhancedTouch = UnityEngine.InputSystem.EnhancedTouch;

public class TouchManager : MonoBehaviour
{
    [Header("Debug UI")]
    [SerializeField] private bool debug = false;
    [SerializeField] private Image touchDetectionImage;

    private GameManager gameManager;
    private bool touching;
    public bool Touching { get => touching; }

    private void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();

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

        gameManager.FingerDownAction();
    }

    private void FingerUp(EnhancedTouch.Finger finger)
    {
        if (finger.index != 0) return;
        
        touching = false;
        touchDetectionImage.color = Color.red;
    }
}