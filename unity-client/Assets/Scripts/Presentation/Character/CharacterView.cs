using Mdb.EasyTrigger.Presentation.Character.Attack;
using Mdb.EasyTrigger.Presentation.Config;
using Mdb.EasyTrigger.Presentation.Utils;
using System;
using System.Collections;
using UnityEngine;

namespace Mdb.EasyTrigger.Presentation.Character
{
    public class CharacterView : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D _rigidbody;
        [SerializeField] private Animator _animator;
        [SerializeField] private CharacterAttack[] _characterAttacks;
        [SerializeField] private float _walkSpeed = 100f;
        [SerializeField] private float _jumpSpeed = 7.5f;
        [SerializeField] private float _groundCheckDistance = 0.1f;
        [SerializeField] private Orientation _orientation = Orientation.Left;

        private IPlatformConfig _platformConfig;

        private float _moveDirection = 0f;
        private bool _tryJump = false;
        private bool _tryCancelJump = false;
        private bool _tryAttack = false;
        private bool _tryTarget = false;

        private int _selectedAttack = 0;
        private bool _attacking = false;
        private bool _targeting = false;

#if UNITY_EDITOR
        [SerializeField] private bool _isGrounded;
#endif

        public void Setup(IPlatformConfig platformConfig)
        {
            _platformConfig = platformConfig;
        }

        public void Move(float direction)
        {
            _moveDirection = direction;
        }

        public void TryJump()
        {
            _tryJump = true;
        }

        public void TryCancelJump()
        {
            _tryCancelJump = true;
        }

        public void TryAttack()
        {
            _tryAttack = true;
        }

        public void TryTarget()
        {
            _tryTarget = true;
        }

        public void ChangeSelectedAttack(int attackIndex)
        {
            if (attackIndex < 0 || attackIndex >= _characterAttacks.Length) return;

            _selectedAttack = attackIndex;
        }

        public void SelectNextAttack()
        {
            _selectedAttack = (_selectedAttack + 1) % _characterAttacks.Length;
        }

        public void SelectPreviousAttack()
        {
            _selectedAttack = (_selectedAttack - 1 + _characterAttacks.Length) % _characterAttacks.Length;
        }

        private void FixedUpdate()
        {
            Vector2 velocity = _rigidbody.velocity;

            bool isGrounded = IsGrounded();

            bool tryAttack = _tryAttack && !_attacking && isGrounded;
            if (tryAttack)
            {
                _attacking = true;
                StartCoroutine(_characterAttacks[_selectedAttack].TryAttack(() => _attacking = false));

                if (_attacking)
                {
                    _animator.SetTrigger(_characterAttacks[_selectedAttack].AnimationId);
                }
            }

            if (!_attacking)
            {
                velocity.x = _moveDirection * _walkSpeed * Time.fixedDeltaTime;

                if (_moveDirection != 0)
                {
                    _orientation = _moveDirection > 0 ? Orientation.Right : Orientation.Left;
                }
                else if (_targeting)
                {
                    _characterAttacks[_selectedAttack].CancelTarget();
                    _targeting = false;
                }
            }

            bool jump = _tryJump && !_attacking && isGrounded;
            if (jump)
            {
                velocity.y = _jumpSpeed;

                if (_targeting)
                {
                    _characterAttacks[_selectedAttack].CancelTarget();
                    _targeting = false;
                }
            }
            else if (_tryCancelJump)
            {
                if (velocity.y > 0)
                {
                    velocity.y = 0;
                }
            }

            bool tryTarget = _tryTarget && isGrounded && _moveDirection == 0 && !_tryJump;
            if (tryTarget)
            {
                if (_characterAttacks[_selectedAttack].TryTarget())
                {
                    _targeting = true;
                }
            }

            _rigidbody.velocity = velocity;

#if UNITY_EDITOR
            _isGrounded = isGrounded;
#endif

            _animator.SetBool(AnimatorUtils.Walking, _moveDirection != 0);
            _animator.SetBool(AnimatorUtils.LookRight, _orientation == Orientation.Right);
            _animator.SetBool(AnimatorUtils.Falling, !isGrounded && velocity.y < -0.0f);
            _animator.SetBool(AnimatorUtils.Jumping, !isGrounded && velocity.y > 0.0f);
            
            _tryJump = false;
            _tryCancelJump = false;
            _tryAttack = false;
            _tryTarget = false;
        }

        private bool IsGrounded()
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, _groundCheckDistance,
                _platformConfig.GroundLayerMask);
            return hit.collider != null;
        }
    }
}
