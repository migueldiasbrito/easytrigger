using Mdb.EasyTrigger.Config;
using Mdb.EasyTrigger.Input;
using Mdb.EasyTrigger.Level;
using System;
using UnityEngine;

namespace Mdb.EasyTrigger.Character
{
    public class CharacterInputListener : MonoBehaviour, IInputListener, IDisposable
    {
        [field: SerializeField] public CharacterView View { get; private set; }

        private IInputController _inputController;
        private IPlatformConfig _platformConfig;
        private IGameManager _gameManager;

        public void OnMove(float axisValue)
        {
            View.Move(axisValue);
        }

        public void OnJump(bool isKeyPressed)
        {
            if (isKeyPressed)
            {
                View.TryJump();
            }
            else
            {
                View.TryCancelJump();
            }
        }

        public void OnAttack()
        {
            View.TryAttack();
        }

        public void OnTarget()
        {
            View.TryTarget();
        }

        public void OnSelectAttack(int attackIndex)
        {
            View.ChangeSelectedAttack(attackIndex);
        }

        public void OnScrollAttacks(float axisValue)
        {
            if (axisValue == 0) return;

            if (axisValue > 0)
            {
                View.SelectNextAttack();
            }
            else
            {
                View.SelectPreviousAttack();
            }
        }

        public void OnJumpDown()
        {
            View.TryJumpDown();
        }

        public void Dispose()
        {
            _inputController.Unsubscribe(this);
        }

        public void Setup(IInputController inputController, IPlatformConfig platformConfig, IGameManager gameManager)
        {
            _inputController = inputController;
            _platformConfig = platformConfig;
            _gameManager = gameManager;

            _inputController.Subscribe(this);

            View.Setup(_platformConfig, _gameManager);
        }
    }
}
