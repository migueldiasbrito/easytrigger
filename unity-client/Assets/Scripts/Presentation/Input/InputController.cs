using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Mdb.EasyTrigger.Presentation.Input
{
    public class InputController : MonoBehaviour, IInputController
    {
        private readonly List<IInputListener> _listeners = new List<IInputListener>();

        public void OnMove(InputAction.CallbackContext callbackContext)
        {
            float value = callbackContext.ReadValue<float>();

            _listeners.ForEach(listener => listener.OnMove(value));
        }

        public void Subscribe(IInputListener listener)
        {
            if (_listeners.Contains(listener)) return;

            _listeners.Add(listener);
        }

        public void Unsubscribe(IInputListener listener)
        {
            _listeners.Remove(listener);
        }
    }
}
