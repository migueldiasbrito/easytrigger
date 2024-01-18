using Mdb.EasyTrigger.Presentation.Character;
using Mdb.EasyTrigger.Presentation.Config;
using Mdb.EasyTrigger.Presentation.Input;
using System.Collections.Generic;
using UnityEngine;

namespace Mdb.EasyTrigger.Presentation.Dev
{
    public class PlayerAnimationsSetup : MonoBehaviour
    {
        [SerializeField] private InputController _inputController;
        [SerializeField] private CharacterInputListener _characterController;
        [SerializeField] private PlatformConfig _platformConfig;
        [SerializeField] private List<CharacterView> _enemies;

        private void Start()
        {
            _characterController.Setup(_inputController, _platformConfig);

            _enemies.ForEach(enemie => enemie.Setup(_platformConfig));
        }

        private void OnDestroy()
        {
            _characterController.Dispose();
        }
    }
}
