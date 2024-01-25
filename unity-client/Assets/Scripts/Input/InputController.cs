using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Mdb.EasyTrigger.Input
{
    public class InputController : MonoBehaviour, IInputController
    {
        private readonly List<IInputListener> _listeners = new List<IInputListener>();

        public void Subscribe(IInputListener listener)
        {
            if (_listeners.Contains(listener)) return;

            _listeners.Add(listener);
        }

        public void Unsubscribe(IInputListener listener)
        {
            _listeners.Remove(listener);
        }

        public void OnMove(InputAction.CallbackContext callbackContext)
        {
            float value = callbackContext.ReadValue<float>();

            _listeners.ForEach(listener => listener.OnMove(value));
        }

        public void OnJump(InputAction.CallbackContext callbackContext)
        {
            if (!callbackContext.performed && !callbackContext.canceled) return;

            _listeners.ForEach(listener => listener.OnJump(callbackContext.performed));
        }

        public void OnAttack(InputAction.CallbackContext callbackContext)
        {
            if (!callbackContext.performed) return;

            _listeners.ForEach(listener => listener.OnAttack());
        }

        public void OnTarget(InputAction.CallbackContext callbackContext)
        {
            if (!callbackContext.performed) return;

            _listeners.ForEach(listener => listener.OnTarget());
        }

        public void OnSelectFirstAttack(InputAction.CallbackContext callbackContext)
        {
            if (!callbackContext.performed) return;

            _listeners.ForEach(listener => listener.OnSelectAttack(0));
        }

        public void OnSelectSecondAttack(InputAction.CallbackContext callbackContext)
        {
            if (!callbackContext.performed) return;

            _listeners.ForEach(listener => listener.OnSelectAttack(1));
        }

        public void OnScrollAttacks(InputAction.CallbackContext callbackContext)
        {
            if (!callbackContext.performed) return;

            float value = callbackContext.ReadValue<float>();

            _listeners.ForEach(listener => listener.OnScrollAttacks(value));
        }

        public void OnJumpDown(InputAction.CallbackContext callbackContext)
        {
            if (!callbackContext.performed) return;

            _listeners.ForEach(listener => listener.OnJumpDown());
        }
    }
}
