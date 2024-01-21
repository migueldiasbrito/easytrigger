using Mdb.EasyTrigger.Presentation.Character.Attack;
using Mdb.EasyTrigger.Presentation.Config;
using Mdb.EasyTrigger.Presentation.Level;
using Mdb.EasyTrigger.Presentation.Utils;
using System;
using System.Collections;
using UnityEngine;

namespace Mdb.EasyTrigger.Presentation.Character
{
    public class CharacterView : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D _rigidbody;
        [SerializeField] private Collider2D _collider2D;
        [SerializeField] private Animator _animator;
        [SerializeField] private CharacterAttack[] _characterAttacks;
        [SerializeField] private float _walkSpeed = 100f;
        [SerializeField] private float _jumpSpeed = 7.5f;
        [SerializeField] private float _groundCheckDistance = 0.1f;
        [SerializeField] private Orientation _orientation = Orientation.Left;

        private IPlatformConfig _platformConfig;
        private ILevel _level;

        private float _moveDirection = 0f;
        private bool _tryJump = false;
        private bool _tryCancelJump = false;
        private bool _tryAttack = false;
        private bool _tryTarget = false;

        private int _selectedAttack = 0;
        private bool _attacking = false;
        private bool _targeting = false;
        private bool _isDead = false;

#if UNITY_EDITOR
        [SerializeField]
#endif
        private bool _isGrounded;

        public void Setup(IPlatformConfig platformConfig, ILevel level)
        {
            _platformConfig = platformConfig;
            _level = level;
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
            if (_isDead || attackIndex < 0 || attackIndex >= _characterAttacks.Length) return;

            _selectedAttack = attackIndex;
        }

        public void SelectNextAttack()
        {
            if (_isDead) return;

            if (_targeting)
            {
                //_characterAttacks[_selectedAttack].SelectNextTarget();
            }
            else
            {
                _selectedAttack = (_selectedAttack + 1) % _characterAttacks.Length;
            }
        }

        public void SelectPreviousAttack()
        {
            if (_isDead) return;

            if (_targeting)
            {
                //_characterAttacks[_selectedAttack].SelectPreviousTarget();
            }
            else
            {
                _selectedAttack = (_selectedAttack - 1 + _characterAttacks.Length) % _characterAttacks.Length;
            }
        }

        public void Kill(Vector2 attackOrigin)
        {
            if (_isDead) return;

            _isDead = true;

            StartCoroutine(OnKilled());

            bool backAttack = _orientation == Orientation.Right && attackOrigin.x < transform.position.x
                || _orientation == Orientation.Left && attackOrigin.x > transform.position.x;

            _animator.SetTrigger(backAttack ? AnimatorUtils.BackHit : AnimatorUtils.FrontHit);
        }

        private IEnumerator OnKilled()
        {
            yield return new WaitUntil(IsGrounded);
            _rigidbody.velocity = Vector2.zero;
            _rigidbody.isKinematic = true;
            _collider2D.enabled = false;
        }

        private void FixedUpdate()
        {
            if (_isDead) return;

            HandleInput();
        }

        private void HandleInput()
        {
            Vector2 velocity = _rigidbody.velocity;
            _isGrounded = IsGrounded();

            HandleAttack();
            HandleMovement(ref velocity);
            bool hasJumped = HandleJump(ref velocity);
            HandleTarget(hasJumped);

            _rigidbody.velocity = velocity;

            _tryJump = false;
            _tryCancelJump = false;
            _tryAttack = false;
            _tryTarget = false;
        }

        private void HandleMovement(ref Vector2 velocity)
        {
            if (!_attacking)
            {
                velocity.x = _moveDirection * _walkSpeed * Time.fixedDeltaTime;

                if (_moveDirection != 0)
                {
                    _orientation = _moveDirection > 0 ? Orientation.Right : Orientation.Left;
                }
            }
            else
            {
                velocity.x = _characterAttacks[_selectedAttack].AttackMovementSpeed * Time.fixedDeltaTime
                    * (_orientation == Orientation.Left ? -1 : 1);
            }

            _animator.SetBool(AnimatorUtils.Walking, _moveDirection != 0);
            _animator.SetBool(AnimatorUtils.LookRight, _orientation == Orientation.Right);
        }

        private bool HandleJump(ref Vector2 velocity)
        {
            bool hasJumped = false;
            bool jump = _tryJump && !_attacking && _isGrounded;
            if (jump)
            {
                velocity.y = _jumpSpeed;
                hasJumped = true;
            }
            else if (_tryCancelJump)
            {
                if (velocity.y > 0)
                {
                    velocity.y = 0;
                }
            }

            _animator.SetBool(AnimatorUtils.Falling, !_isGrounded && _rigidbody.velocity.y < -0.0f);
            _animator.SetBool(AnimatorUtils.Jumping, !_isGrounded && _rigidbody.velocity.y > 0.0f);

            return hasJumped;
        }

        private void HandleAttack()
        {
            bool tryAttack = _tryAttack && !_attacking && _isGrounded;
            if (tryAttack)
            {
                _attacking = true;
                StartCoroutine(_characterAttacks[_selectedAttack].TryAttack(() => _attacking = false));

                if (_attacking)
                {
                    _animator.SetTrigger(_characterAttacks[_selectedAttack].AttackAnimationTrigger);
                }
            }
        }

        private void HandleTarget(bool hasJumped)
        {
            bool cancelTarget = _targeting && (_tryTarget || _moveDirection != 0 || hasJumped);
            if (cancelTarget)
            {
                if (_characterAttacks[_selectedAttack].TargetAnimationTrigger != null)
                {
                    _animator.SetBool(_characterAttacks[_selectedAttack].TargetAnimationTrigger.Value, false);
                }
                _targeting = false;
                return;
            }

            bool tryTarget = _tryTarget && _isGrounded && _moveDirection == 0 && !_tryJump;
            if (tryTarget)
            {
                if (_characterAttacks[_selectedAttack].CanTarget)
                {
                    _targeting = true;

                    if (_characterAttacks[_selectedAttack].TargetAnimationTrigger != null)
                    {
                        _animator.SetBool(_characterAttacks[_selectedAttack].TargetAnimationTrigger.Value, true);
                    }
                }
            }
        }

        private bool IsGrounded()
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, _groundCheckDistance,
                _platformConfig.GroundLayerMask);
            return hit.collider != null;
        }
    }
}
