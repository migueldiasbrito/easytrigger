using Mdb.EasyTrigger.Presentation.Input;
using UnityEngine;

namespace Mdb.EasyTrigger.Presentation.Character
{
    public class CharacterInputListener : MonoBehaviour, IInputListener
    {
        [SerializeField] private CharacterView _view;

        public void OnMove(float axisValue)
        {
            _view.Move(axisValue);
        }
    }
}
