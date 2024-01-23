using Mdb.EasyTrigger.Presentation.Config;
using Mdb.EasyTrigger.Presentation.Input;
using Mdb.EasyTrigger.Presentation.Level;
using System;
using UnityEngine;

namespace Mdb.EasyTrigger.Presentation.Character
{
    public class CharacterInputListener : MonoBehaviour, IInputListener, IDisposable
    {
        [SerializeField] private CharacterView _view;

        private IInputController _inputController;
        private IPlatformConfig _platformConfig;
        private ILevel _level;

        public void OnMove(float axisValue)
        {
            _view.Move(axisValue);
        }

        public void OnJump(bool isKeyPressed)
        {
            if (isKeyPressed)
            {
                _view.TryJump();
            }
            else
            {
                _view.TryCancelJump();
            }
        }

        public void OnAttack()
        {
            _view.TryAttack();
        }

        public void OnTarget()
        {
            _view.TryTarget();
        }

        public void OnSelectAttack(int attackIndex)
        {
            _view.ChangeSelectedAttack(attackIndex);
        }

        public void OnScrollAttacks(float axisValue)
        {
            if (axisValue == 0) return;

            if (axisValue > 0)
            {
                _view.SelectNextAttack();
            }
            else
            {
                _view.SelectPreviousAttack();
            }
        }

        public void OnJumpDown()
        {
            _view.TryJumpDown();
        }

        public void Dispose()
        {
            _inputController.Unsubscribe(this);
        }

        public void Setup(IInputController inputController, IPlatformConfig platformConfig, ILevel level)
        {
            _inputController = inputController;
            _platformConfig = platformConfig;
            _level = level;

            _inputController.Subscribe(this);

            _view.Setup(_platformConfig, _level);
        }
    }
}
