﻿using Mdb.EasyTrigger.Presentation.Config;
using Mdb.EasyTrigger.Presentation.Input;
using Mdb.EasyTrigger.Presentation.Level;
using System;
using UnityEngine;

namespace Mdb.EasyTrigger.Presentation.Character
{
    public class CharacterInputListener : MonoBehaviour, IInputListener, IDisposable
    {
        [field: SerializeField] public CharacterView View { get; private set; }

        private IInputController _inputController;
        private IPlatformConfig _platformConfig;
        private ILevel _level;

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

        public void Setup(IInputController inputController, IPlatformConfig platformConfig, ILevel level)
        {
            _inputController = inputController;
            _platformConfig = platformConfig;
            _level = level;

            _inputController.Subscribe(this);

            View.Setup(_platformConfig, _level);
        }
    }
}
