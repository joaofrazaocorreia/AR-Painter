using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using EnhancedTouch = UnityEngine.InputSystem.EnhancedTouch;

public class TouchManager : MonoBehaviour
{
    private PlayerInput playerInput;
    private bool touching;
    public bool Touching { get => touching; }
    public UnityEvent OnFingerDown;
    public UnityEvent OnFingerUp;

    private void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        touching = false;
    }


    private void Update()
    {
        KeyboardTouchSimulation();
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
        OnFingerDown?.Invoke();
    }

    private void FingerUp(EnhancedTouch.Finger finger)
    {
        if (finger.index != 0) return;
        
        touching = false;
        OnFingerUp?.Invoke();
    }

    private void KeyboardTouchSimulation()
    {
        if (playerInput.actions["Interact"].WasPressedThisFrame())
        {
            touching = true;
            OnFingerDown?.Invoke();
        }

        if (playerInput.actions["Interact"].WasReleasedThisFrame())
        {
            touching = false;
            OnFingerUp?.Invoke();
        }
    }
}