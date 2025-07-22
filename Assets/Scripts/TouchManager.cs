using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using EnhancedTouch = UnityEngine.InputSystem.EnhancedTouch;

public class TouchManager : MonoBehaviour
{
    [Header("Debug UI")]
    [SerializeField] private bool debug = false;
    [SerializeField] private Image touchDetectionImage;

    private PlayerInput playerInput;
    private bool touching;
    public bool Touching { get => touching; }
    public UnityEvent OnFingerDown;
    public UnityEvent OnFingerUp;

    private void Start()
    {
        playerInput = GetComponent<PlayerInput>();

        touchDetectionImage.gameObject.SetActive(debug);
        touching = false;
        touchDetectionImage.color = Color.red;
    }


    private void Update()
    {
        TouchSimulation();
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

        OnFingerDown?.Invoke();
    }

    private void FingerUp(EnhancedTouch.Finger finger)
    {
        if (finger.index != 0) return;
        
        touching = false;
        touchDetectionImage.color = Color.red;

        OnFingerUp?.Invoke();
    }

    private void TouchSimulation()
    {
        if (playerInput.actions["Interact"].WasPressedThisFrame())
        {
            touching = true;
            touchDetectionImage.color = Color.green;

            OnFingerDown?.Invoke();
        }

        if (playerInput.actions["Interact"].WasReleasedThisFrame())
        {
            touching = false;
            touchDetectionImage.color = Color.red;

            OnFingerUp?.Invoke();
        }
    }
}