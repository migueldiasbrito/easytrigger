using Mdb.EasyTrigger.Character.Attack;
using Mdb.EasyTrigger.Config;
using Mdb.EasyTrigger.Level;
using Mdb.EasyTrigger.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Mdb.EasyTrigger.Character
{
    public class CharacterView : MonoBehaviour
    {
        private struct TargetingStatus
        {
            public bool IsTargeting;
            public bool IsInRange;
            public bool IsTargeted;
        }

        public bool IsDead { get; private set; } = false;
        public Vector2 Center => _center.position;

        [SerializeField] private Rigidbody2D _rigidbody;
        [SerializeField] private Collider2D _collider;
        [SerializeField] private Animator _animator;
        
        [field: SerializeField] public Orientation Orientation { get; set; } = Orientation.Left;
        [SerializeField] private float _walkSpeed = 100f;
        [SerializeField] private float _jumpSpeed = 7.5f;
        [SerializeField] private float _jumpVelocityAnimationThreshold = 2.0f;
        [SerializeField] private float _jumpDownDuration = 0.3f;
        
        [field: SerializeField] public CharacterAttack[] CharacterAttacks { get; set; }
        [SerializeField] private Transform _center;

        [SerializeField] private float _groundCheckDistance = 0.1f;
        [SerializeField] private Transform _groundCheckLeft;
        [SerializeField] private Transform _groundCheckRight;

        private IPlatformConfig _platformConfig;
        private ICampaign _campaign;
        private Image _selectedAttackImage = null;

#if UNITY_EDITOR
        [SerializeField]
#endif
        private float _moveDirection = 0f;
        private bool _tryJump = false;
        private bool _tryCancelJump = false;
        private bool _tryAttack = false;
        private bool _tryTarget = false;
        private bool _tryJumpDown = false;

        private int _selectedAttackIndex = 0;
        private bool _attacking = false;
        private bool _targeting = false;
        private int? _currentTargetIndex = null;
        private bool _selectNextTarget = false;
        private bool _selectPreviousTarget = false;
        private Dictionary<CharacterView, TargetingStatus> _targetStatus =
            new Dictionary<CharacterView, TargetingStatus>();

        private CharacterAttack _selectedAttack => CharacterAttacks[_selectedAttackIndex];
        private CharacterView[] Enemies => _campaign.CurrentLevel.Enemies;
        private CharacterView _currentTarget => Enemies[_currentTargetIndex.Value];

        private Action _onDeathCallback = null;

#if UNITY_EDITOR
        [SerializeField] private Vector2 _velocity;
        [SerializeField]
#endif
        private bool _isGrounded;
        private bool _jumpingDown = false;

        public void Setup(IPlatformConfig platformConfig, ICampaign campaign)
        {
            _platformConfig = platformConfig;
            _campaign = campaign;
        }

        public void SetAttackImage(Image selectedAttackImage)
        {
            _selectedAttackImage = selectedAttackImage;

            _selectedAttackImage.sprite = _selectedAttack.Sprite;
        }

        public void SetOnDeathCallback(Action onDeathCallback)
        {
            _onDeathCallback = onDeathCallback;
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
            if (IsDead || attackIndex < 0 || attackIndex >= CharacterAttacks.Length) return;

            _selectedAttackIndex = attackIndex;

            if (_selectedAttackImage != null)
            {
                _selectedAttackImage.sprite = _selectedAttack.Sprite;
            }
        }

        public void SelectNextAttack()
        {
            if (IsDead) return;

            if (_targeting)
            {
                _selectNextTarget = true;
            }
            else
            {
                ChangeSelectedAttack((_selectedAttackIndex + 1) % CharacterAttacks.Length);
            }
        }

        public void SelectPreviousAttack()
        {
            if (IsDead) return;

            if (_targeting)
            {
                _selectPreviousTarget = true;
            }
            else
            {
                ChangeSelectedAttack((_selectedAttackIndex - 1 + CharacterAttacks.Length) % CharacterAttacks.Length);
            }
        }

        public void TryJumpDown()
        {
            _tryJumpDown = true;
        }

        public void Kill(Vector2 attackOrigin)
        {
            if (IsDead) return;

            IsDead = true;

            StartCoroutine(OnKilled());

            bool backAttack = Orientation == Orientation.Right && attackOrigin.x < transform.position.x
                || Orientation == Orientation.Left && attackOrigin.x > transform.position.x;

            _animator.SetTrigger(backAttack ? AnimatorUtils.BackHit : AnimatorUtils.FrontHit);

            if (_targeting)
            {
                for (int i = 0; i < Enemies.Length; i++)
                {
                    Enemies[i].SetPlayerTargeting(this, false);
                    Enemies[i].SetInRange(this, false);

                    if (_currentTargetIndex == i)
                    {
                        _currentTarget.SetTargeted(this, false);
                    }
                }
            }

            _animator.SetBool(AnimatorUtils.PlayerTargeting, false);
            _animator.SetBool(AnimatorUtils.InRange, false);
            _animator.SetBool(AnimatorUtils.Targeted, false);

            _onDeathCallback?.Invoke();
        }

        private IEnumerator OnKilled()
        {
            yield return new WaitUntil(IsGrounded);
            _rigidbody.velocity = Vector2.zero;
            _rigidbody.isKinematic = true;
            _collider.enabled = false;
        }
        public bool IsGrounded()
        {
            LayerMask groundedLayerMask = _platformConfig.GroundLayerMask;
            if (!_jumpingDown)
            {
                groundedLayerMask |= _platformConfig.PlatformLayerMask;
            }

            RaycastHit2D leftHit = Physics2D.Raycast(_groundCheckLeft.position, Vector2.down, _groundCheckDistance,
                groundedLayerMask);
            if (leftHit.collider != null) return true;

            RaycastHit2D rightHit = Physics2D.Raycast(_groundCheckRight.position, Vector2.down, _groundCheckDistance,
                groundedLayerMask);
            return rightHit.collider != null;
        }

        private void FixedUpdate()
        {
            if (IsDead) return;

            HandleInput();
        }

        private void HandleInput()
        {
            Vector2 velocity = _rigidbody.velocity;
            _isGrounded = IsGrounded();

            HandleAttack();
            HandleMovement(ref velocity);
            bool hasJumped = HandleJump(ref velocity);
            HandleJumpDown(hasJumped);
            HandleTarget(hasJumped);

            _rigidbody.velocity = velocity;
#if UNITY_EDITOR
            _velocity = velocity;
#endif

            _tryJump = false;
            _tryCancelJump = false;
            _tryAttack = false;
            _tryTarget = false;
            _tryJumpDown = false;
        }

        private void HandleMovement(ref Vector2 velocity)
        {
            if (!_attacking)
            {
                velocity.x = _moveDirection * _walkSpeed * Time.fixedDeltaTime;

                if (_moveDirection != 0)
                {
                    Orientation = _moveDirection > 0 ? Orientation.Right : Orientation.Left;
                }
            }
            else
            {
                velocity.x = _selectedAttack.AttackMovementSpeed * Time.fixedDeltaTime *
                    (Orientation == Orientation.Left ? -1 : 1);
            }

            _animator.SetBool(AnimatorUtils.Walking, _moveDirection != 0);
            _animator.SetBool(AnimatorUtils.LookRight, Orientation == Orientation.Right);
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

            bool isJumping = !_isGrounded && _rigidbody.velocity.y > 0.0f
                || _rigidbody.velocity.y > _jumpVelocityAnimationThreshold;
            _animator.SetBool(AnimatorUtils.Falling, !_isGrounded && _rigidbody.velocity.y < -0.0f);
            _animator.SetBool(AnimatorUtils.Jumping, isJumping);

            return hasJumped;
        }

        private void HandleAttack()
        {
            bool tryAttack = _tryAttack && !_attacking && _isGrounded;
            if (tryAttack)
            {
                _attacking = true;

                Vector2 direction = _currentTargetIndex != null
                    ? (_currentTarget.Center - Center).normalized
                    : Orientation == Orientation.Left ? Vector2.left : Vector2.right;
                StartCoroutine(_selectedAttack.TryAttack(Center, direction, () => _attacking = false));

                if (_attacking)
                {
                    _animator.SetTrigger(_selectedAttack.AttackAnimationTrigger);

                    if (_currentTargetIndex != null && _currentTarget.IsDead)
                    {
                        _currentTargetIndex = null;
                        _selectNextTarget = true;
                    }

                    if (_selectedAttack.MakesSound)
                    {
                        _campaign.Shoot(transform.position);
                    }
                }
            }
        }

        private void HandleJumpDown(bool hasJumped)
        {
            bool tryJumpDown = _tryJumpDown && _isGrounded && !_attacking && !hasJumped;

            if (tryJumpDown)
            {
                StartCoroutine(JumpingDown());
            }
        }

        private IEnumerator JumpingDown()
        {
            Collider2D platformCollider = _campaign.CurrentLevel.PlatformCollider;
            _jumpingDown = true;
            Physics2D.IgnoreCollision(_collider, platformCollider);
            yield return new WaitForSeconds(_jumpDownDuration);
            _jumpingDown = false;
            Physics2D.IgnoreCollision(_collider, platformCollider, false);
        }

        private void HandleTarget(bool hasJumped)
        {
            bool cancelTarget = _targeting && (_tryTarget || _moveDirection != 0 || hasJumped);
            bool tryTarget = _tryTarget && _isGrounded && _moveDirection == 0 && !_tryJump;

            if (cancelTarget)
            {
                if (_selectedAttack.TargetAnimationTrigger != null)
                {
                    _animator.SetBool(_selectedAttack.TargetAnimationTrigger.Value, false);
                }

                for (int i = 0; i < Enemies.Length; i++)
                {
                    Enemies[i].SetPlayerTargeting(this, false);
                    Enemies[i].SetInRange(this, false);

                    if (_currentTargetIndex == i)
                    {
                        _currentTarget.SetTargeted(this, false);
                    }
                }

                _currentTargetIndex = null;
                _targeting = false;
            }
            else if (tryTarget)
            {
                if (_selectedAttack.CanTarget)
                {
                    _targeting = true;
                    _selectNextTarget = true;

                    if (_selectedAttack.TargetAnimationTrigger != null)
                    {
                        _animator.SetBool(_selectedAttack.TargetAnimationTrigger.Value, true);
                    }

                    for (int i = 0; i < Enemies.Length; i++)
                    {
                        Enemies[i].SetPlayerTargeting(this, true);
                    }
                }
            }

            if (_targeting)
            {
                List<int> enemiesInTarget = new List<int>();
                int currentTargetIndex = -1;

                for (int i = 0; i < Enemies.Length; i++)
                {
                    if (IsEnemyInTarget(Enemies[i], _selectedAttack.Range))
                    {
                        if (_currentTargetIndex == i)
                        {
                            currentTargetIndex = enemiesInTarget.Count;
                        }

                        enemiesInTarget.Add(i);
                        Enemies[i].SetInRange(this, true);
                    }
                    else
                    {
                        if (_currentTargetIndex == i)
                        {
                            _currentTarget.SetTargeted(this, false);
                            _currentTargetIndex = null;
                        }

                        Enemies[i].SetInRange(this, false);
                    }
                }

                if (enemiesInTarget.Count > 0)
                {
                    if (_selectNextTarget)
                    {
                        currentTargetIndex = (currentTargetIndex + 1) % enemiesInTarget.Count;
                    }
                    else if (_selectPreviousTarget)
                    {
                        currentTargetIndex = (currentTargetIndex - 1 + enemiesInTarget.Count) % enemiesInTarget.Count;
                    }

                    if (_currentTargetIndex != enemiesInTarget[currentTargetIndex])
                    {
                        if (_currentTargetIndex != null)
                        {
                            _currentTarget.SetTargeted(this, false);
                        }
                        _currentTargetIndex = enemiesInTarget[currentTargetIndex];
                        _currentTarget.SetTargeted(this, true);

                        float distanceOnXCoord = _currentTarget.transform.position.x - transform.position.x;
                        if (distanceOnXCoord != 0)
                        {
                            Orientation = distanceOnXCoord > 0 ? Orientation.Right : Orientation.Left;
                        }
                    }
                }
            }

            _animator.SetBool(AnimatorUtils.PlayerTargeting, _targetStatus.Any(status => status.Value.IsTargeting));
            _animator.SetBool(AnimatorUtils.InRange, _targetStatus.Any(status => status.Value.IsInRange));
            _animator.SetBool(AnimatorUtils.Targeted, _targetStatus.Any(status => status.Value.IsTargeted));

            _selectNextTarget = false;
            _selectPreviousTarget = false;
        }

        private bool IsEnemyInTarget(CharacterView enemy, float range)
        {
            if (enemy.IsDead) return false;

            Vector2 direction = (enemy.Center - Center).normalized;
#if UNITY_EDITOR
            Vector2 end = Center + (direction * range);
            Debug.DrawLine(Center, end);
#endif
            RaycastHit2D[] hits = Physics2D.RaycastAll(Center, direction, range);

            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].transform == transform) continue;

                if (hits[i].transform == enemy.transform) return true;

                return false;
            }

            return false;
        }

        private void SetPlayerTargeting(CharacterView player, bool value)
        {
            if (!_targetStatus.TryGetValue(player, out TargetingStatus targetingStatus))
            {
                targetingStatus = new TargetingStatus();
            }

            targetingStatus.IsTargeting = value;
            _targetStatus[player] = targetingStatus;
        }

        private void SetInRange(CharacterView player, bool value)
        {
            if (!_targetStatus.TryGetValue(player, out TargetingStatus targetingStatus))
            {
                targetingStatus = new TargetingStatus();
            }

            targetingStatus.IsInRange = value;
            _targetStatus[player] = targetingStatus;
        }

        private void SetTargeted(CharacterView player, bool value)
        {
            if (!_targetStatus.TryGetValue(player, out TargetingStatus targetingStatus))
            {
                targetingStatus = new TargetingStatus();
            }

            targetingStatus.IsTargeted = value;
            _targetStatus[player] = targetingStatus;
        }
    }
}
