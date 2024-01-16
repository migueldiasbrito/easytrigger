using Mdb.EasyTrigger.Presentation.Character;
using Mdb.EasyTrigger.Presentation.Input;
using UnityEngine;

namespace Mdb.EasyTrigger.Presentation.Dev
{
    public class PlayerAnimationsSetup : MonoBehaviour
    {
        [SerializeField] private InputController _inputController;
        [SerializeField] private CharacterInputListener _characterController;

        private void Start()
        {
            _inputController.Subscribe(_characterController);
        }
    }
}
