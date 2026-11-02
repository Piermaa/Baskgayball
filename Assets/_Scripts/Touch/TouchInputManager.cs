using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.SceneManagement;
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
            SceneManager.sceneLoaded += OnSceneLoaded;
            GlobalEvents.OnFinishMatch += ClearAllTouches;
            GlobalEvents.OnStartMatch += ClearAllTouches;
        }

        private void OnEnable()
        {
            EnhancedTouchSupport.Enable();
            Touch.onFingerDown += HandleFingerDown;
            Touch.onFingerUp += HandleFingerUp;
        }

        private void OnDisable()
        {
            // Si es un objeto duplicado destruido por ser singleton, no deshabilitar EnhancedTouch global!
            if (Instance != this) return;

            Touch.onFingerDown -= HandleFingerDown;
            Touch.onFingerUp -= HandleFingerUp;
            EnhancedTouchSupport.Disable();
            SceneManager.sceneLoaded -= OnSceneLoaded;
            GlobalEvents.OnFinishMatch -= ClearAllTouches;
            GlobalEvents.OnStartMatch -= ClearAllTouches;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            ClearAllTouches();
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
            if (IsPointerOverUI(finger.index, finger.screenPosition)) return;

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
                Vector2 mousePos = Mouse.current.position.ReadValue();
                if (!IsPointerOverUI(MouseSimulatedId, mousePos))
                {
                    TryRegisterTouch(mousePos, MouseSimulatedId);
                }
            }
            else if (!isPressed && _mouseWasPressedLastFrame)
            {
                ReleaseTouch(MouseSimulatedId);
            }

            _mouseWasPressedLastFrame = isPressed;
        }
#endif

        private bool IsPointerOverUI(int touchId, Vector2 screenPosition)
        {
            if (EventSystem.current == null) return false;

            if (touchId == MouseSimulatedId)
            {
                return EventSystem.current.IsPointerOverGameObject();
            }

            return EventSystem.current.IsPointerOverGameObject(touchId);
        }

        private void TryRegisterTouch(Vector2 screenPosition, int touchId)
        {
            EPlayerSide side = GetSideFromScreenPosition(screenPosition);

            if (_activeTouchIdBySide.ContainsKey(side))
            {
                return;
            }

            _activeTouchIdBySide[side] = touchId;
            OnPlayerPressStart?.Invoke(side);
        }

        private void ReleaseTouch(int touchId)
        {
            var sidesToRemove = new List<EPlayerSide>();
            foreach (var kvp in _activeTouchIdBySide)
            {
                if (kvp.Value == touchId)
                {
                    sidesToRemove.Add(kvp.Key);
                }
            }

            foreach (var side in sidesToRemove)
            {
                _activeTouchIdBySide.Remove(side);
                OnPlayerPressEnd?.Invoke(side);
            }
        }

        public void ClearAllTouches()
        {
            var activeSides = new List<EPlayerSide>(_activeTouchIdBySide.Keys);
            _activeTouchIdBySide.Clear();

            foreach (var side in activeSides)
            {
                OnPlayerPressEnd?.Invoke(side);
            }

#if UNITY_EDITOR
            _mouseWasPressedLastFrame = false;
#endif
        }

        private static EPlayerSide GetSideFromScreenPosition(Vector2 screenPosition)
        {
            return screenPosition.x >= Screen.width * 0.5f ? EPlayerSide.Player1 : EPlayerSide.Player2;
        }

        public bool IsSideActive(EPlayerSide side) => _activeTouchIdBySide.ContainsKey(side);
    }
}
