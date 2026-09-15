using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

namespace BaskgayBall.Input
{
    [DisallowMultipleComponent]
    public class TouchInputManager : MonoBehaviour
    {
        public static TouchInputManager Instance { get; private set; }

        [Header("Debug / Testing")]
        [Tooltip("Permite simular touches con el click del mouse mientras se prueba en el Editor.")]
        [SerializeField] private bool simulateWithMouseInEditor = true;

        public event Action<EPlayerSide> OnPlayerPressStart;

        public event Action<EPlayerSide> OnPlayerPressEnd;

        private readonly Dictionary<EPlayerSide, int> _activeTouchIdBySide = new Dictionary<EPlayerSide, int>(2);

        private const int MouseSimulatedId = int.MinValue;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void OnEnable()
        {
            EnhancedTouchSupport.Enable();
            Touch.onFingerDown += HandleFingerDown;
            Touch.onFingerUp += HandleFingerUp;
        }

        private void OnDisable()
        {
            Touch.onFingerDown -= HandleFingerDown;
            Touch.onFingerUp -= HandleFingerUp;
            EnhancedTouchSupport.Disable();
        }

        private void Update()
        {
#if UNITY_EDITOR
            if (simulateWithMouseInEditor)
            {
                HandleMouseSimulation();
            }
#endif
        }

        private void HandleFingerDown(Finger finger)
        {
            TryRegisterTouch(finger.screenPosition, finger.index);
        }

        private void HandleFingerUp(Finger finger)
        {
            ReleaseTouch(finger.index);
        }

#if UNITY_EDITOR
        private bool _mouseWasPressedLastFrame;

        private void HandleMouseSimulation()
        {
            if (Mouse.current == null) return;

            bool isPressed = Mouse.current.leftButton.isPressed;

            if (isPressed && !_mouseWasPressedLastFrame)
            {
                TryRegisterTouch(Mouse.current.position.ReadValue(), MouseSimulatedId);
            }
            else if (!isPressed && _mouseWasPressedLastFrame)
            {
                ReleaseTouch(MouseSimulatedId);
            }

            _mouseWasPressedLastFrame = isPressed;
        }
#endif
        private void TryRegisterTouch(Vector2 screenPosition, int touchId)
        {
            print($"Tried registering <{touchId}> touch");
            EPlayerSide side = GetSideFromScreenPosition(screenPosition);

            if (_activeTouchIdBySide.ContainsKey(side))
            {
                // Ese lado ya está siendo usado por otro dedo: se ignora el nuevo touch.
                return;
            }

            _activeTouchIdBySide[side] = touchId;
            OnPlayerPressStart?.Invoke(side);
        }

        private void ReleaseTouch(int touchId)
        {
            print($"Tried releasing <{touchId}> touch");

            foreach (var kvp in _activeTouchIdBySide)
            {
                if (kvp.Value != touchId) continue;

                EPlayerSide side = kvp.Key;
                _activeTouchIdBySide.Remove(side);
                OnPlayerPressEnd?.Invoke(side);
                return;
            }
        }

        private static EPlayerSide GetSideFromScreenPosition(Vector2 screenPosition)
        {
            return screenPosition.x >= Screen.width * 0.5f ? EPlayerSide.Player1 : EPlayerSide.Player2;
        }

        public bool IsSideActive(EPlayerSide side) => _activeTouchIdBySide.ContainsKey(side);
    }
}
