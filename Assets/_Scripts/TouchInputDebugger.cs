using UnityEngine;

namespace BaskgayBall.Input
{
    public class TouchInputDebugger : MonoBehaviour
    {
        [SerializeField] private TouchInputManager touchInputManager;

        private void OnEnable()
        {
            if (touchInputManager == null)
            {
                touchInputManager = TouchInputManager.Instance;
            }

            if (touchInputManager == null)
            {
                Debug.LogWarning("[TouchInputDebugger] No se encontró un TouchInputManager en la escena.");
                return;
            }

            touchInputManager.OnPlayerPressStart += HandlePressStart;
            touchInputManager.OnPlayerPressEnd += HandlePressEnd;
        }

        private void OnDisable()
        {
            if (touchInputManager == null) return;

            touchInputManager.OnPlayerPressStart -= HandlePressStart;
            touchInputManager.OnPlayerPressEnd -= HandlePressEnd;
        }

        private void HandlePressStart(PlayerSide side)
        {
            Debug.Log($"[Input] {side} -> PRESS");
        }

        private void HandlePressEnd(PlayerSide side)
        {
            Debug.Log($"[Input] {side} -> RELEASE");
        }
    }
}
