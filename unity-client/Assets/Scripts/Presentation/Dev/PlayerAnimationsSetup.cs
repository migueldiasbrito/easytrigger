using Mdb.EasyTrigger.Presentation.Character;
using Mdb.EasyTrigger.Presentation.Config;
using Mdb.EasyTrigger.Presentation.Input;
using Mdb.EasyTrigger.Presentation.Level;
using System.Collections.Generic;
using UnityEngine;

namespace Mdb.EasyTrigger.Presentation.Dev
{
    public class PlayerAnimationsSetup : MonoBehaviour, ILevel
    {
        [SerializeField] private InputController _inputController;
        [SerializeField] private CharacterInputListener _characterController;
        [SerializeField] private PlatformConfig _platformConfig;
        [SerializeField] private List<CharacterView> _enemies;

        public CharacterView[] Enemies => _enemies.ToArray();

        private void Start()
        {
            _characterController.Setup(_inputController, _platformConfig, this);

            _enemies.ForEach(enemie => enemie.Setup(_platformConfig, this));
        }

        private void OnDestroy()
        {
            _characterController.Dispose();
        }
    }
}
