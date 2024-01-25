using Mdb.EasyTrigger.Character;
using Mdb.EasyTrigger.Character.Attack;
using Mdb.EasyTrigger.Config;
using Mdb.EasyTrigger.Enemy.Behaviour;
using Mdb.EasyTrigger.Level;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Mdb.EasyTrigger.Enemy
{
    public class EnemyController : MonoBehaviour
    {
        private struct PlayerAwareness
        {
            public bool IsAware;
            public float SqrDistance;
            public bool CanMeleeAttack;
            public bool CanShoot;
        }

        [field: SerializeField] public CharacterView View { get; private set; }
        [SerializeField] private IEnemyBehaviour _idleBehaviour;
        [SerializeField] private FollowBehaviour _followBehaviour;
        [SerializeField] private InspectBehaviour _inspectBehaviour;
        [SerializeField] private Vector2 _positionComparisonTolerance = new Vector2(0.1f, 0.2f);
        [SerializeField] private float _platformToleranceDistance = 5f;
        [SerializeField] private float _inspectTime = 10f;

        [SerializeField] private float _playerDetectionDistance = 10f;
        [SerializeField] private float _fieldOfView = 60f;

        private IPlatformConfig _platformConfig;
        private ICampaign _campaign;

        private Dictionary<CharacterView, PlayerAwareness> _playersAwareness
            = new Dictionary<CharacterView, PlayerAwareness>();

        private int _meleeAttackIndex = -1;
        private int _weaponAttackIndex = -1;

        private IEnemyBehaviour _currentBehaviour { get; set; }
        private Coroutine _inspectCoroutine = null;

        public void Setup(IPlatformConfig platformConfig, ICampaign _campaign)
        {
            _platformConfig = platformConfig;
            this._campaign = _campaign;

            View.Setup(_platformConfig, this._campaign);

            foreach (CharacterView player in this._campaign.Players)
            {
                _playersAwareness.Add(player, new PlayerAwareness { SqrDistance = -1.0f });
            }
        }

        public void AddPointOfInterest(Vector2 inspectPoint)
        {
            if (_currentBehaviour == _followBehaviour) return;

            _inspectBehaviour.Setup(inspectPoint, View.transform.position);
            _inspectCoroutine = StartCoroutine(Inspect());
        }

        private IEnumerator Inspect()
        {
            _currentBehaviour = _inspectBehaviour;
            yield return new WaitForSeconds(_inspectTime);

            _currentBehaviour = _idleBehaviour;
            _inspectBehaviour.Clear();
        }

        private void Start()
        {
            _currentBehaviour = _idleBehaviour;

            for (int i = 0; i < View.CharacterAttacks.Length; i++)
            {
                if (View.CharacterAttacks[i] is MeleeAttack)
                {
                    _meleeAttackIndex = i;
                }
                else if (View.CharacterAttacks[i] is WeaponAttack)
                {
                    _weaponAttackIndex = i;
                }
            }
        }

        private void FixedUpdate()
        {
            if (View.IsDead) return;

            DetectPlayer();

            Vector2 currentPosition = View.transform.position;
            _currentBehaviour.UpdatedPosition(currentPosition, _positionComparisonTolerance);

            Vector2? nextPoint = _currentBehaviour.GetNextPoint();
            HandleMovement(currentPosition, nextPoint);
            HandleJump(currentPosition, nextPoint);
        }

        private void DetectPlayer()
        {
#if UNITY_EDITOR
            float direction = View.Orientation == Orientation.Left ? -1 : 1;
            float radians = Mathf.Deg2Rad * (_fieldOfView / 2);
            Vector2 topAngle = new Vector2(direction * Mathf.Cos(radians), Mathf.Sin(radians));
            Vector2 topFov = View.Center + _playerDetectionDistance * topAngle;
            Vector2 bottomAngle = new Vector2(direction * Mathf.Cos(radians), -Mathf.Sin(radians));
            Vector2 bottomFov = View.Center + _playerDetectionDistance * bottomAngle;
            Debug.DrawLine(View.Center, topFov);
            Debug.DrawLine(View.Center, bottomFov);
#endif
            if (_campaign == null) return;

            foreach (CharacterView player in _campaign.Players)
            {
                PlayerAwareness playerAwareness = _playersAwareness[player];

                if (player.IsDead)
                {
                    if (playerAwareness.IsAware)
                    {
                        playerAwareness.IsAware = false;
                        playerAwareness.CanMeleeAttack = false;
                        playerAwareness.CanShoot = false;
                        playerAwareness.SqrDistance = -1.0f;
                    }
                }
                else
                {
                    playerAwareness.CanMeleeAttack = false;
                    playerAwareness.CanShoot = false;

                    Vector2 distanceToPlayer = (player.Center - View.Center);
                    playerAwareness.SqrDistance = distanceToPlayer.sqrMagnitude;

                    if (playerAwareness.SqrDistance <= _playerDetectionDistance * _playerDetectionDistance)
                    {
                        Vector2 aimDirection = View.Orientation == Orientation.Left ? Vector2.left : Vector2.right;
                        Vector2 directionToPlayer = distanceToPlayer.normalized;
                        if (Vector2.Angle(aimDirection, directionToPlayer) < _fieldOfView / 2f)
                        {
                            RaycastHit2D[] hits = Physics2D.RaycastAll(View.Center, directionToPlayer,
                                _playerDetectionDistance);

                            foreach (RaycastHit2D hit in hits)
                            {
                                // Is the player
                                if (hit.transform == player.transform)
                                {
                                    playerAwareness.IsAware = true;
                                    playerAwareness.CanMeleeAttack = playerAwareness.SqrDistance
                                        <= Mathf.Pow(View.CharacterAttacks[_meleeAttackIndex].Range, 2);
                                    playerAwareness.CanShoot = playerAwareness.SqrDistance
                                        <= Mathf.Pow(View.CharacterAttacks[_weaponAttackIndex].Range, 2);
                                    break;
                                }

                                // Is self
                                if (hit.transform == View.transform) continue;

                                // Other characters won't hide the player, but platforms do
                                if (!hit.collider.TryGetComponent(out CharacterView _)) break;
                            }
                        }
                    }
                }

                _playersAwareness[player] = playerAwareness;
            }

            if (_playersAwareness.Any(playerAwareness => playerAwareness.Value.IsAware))
            {
                CharacterView targettedPlayer = null;
                PlayerAwareness targettedPlayerAwareness = new PlayerAwareness();

                foreach (CharacterView player in _campaign.Players)
                {
                    PlayerAwareness playerAwareness = _playersAwareness[player];
                    if (!playerAwareness.IsAware) continue;

                    if (targettedPlayer == null || playerAwareness.SqrDistance < targettedPlayerAwareness.SqrDistance)
                    {
                        targettedPlayer = player;
                        targettedPlayerAwareness = playerAwareness;
                    }
                }

                if (targettedPlayerAwareness.CanMeleeAttack)
                {
                    View.ChangeSelectedAttack(_meleeAttackIndex);
                    View.TryAttack();
                }
                else if (targettedPlayerAwareness.CanShoot)
                {
                    View.ChangeSelectedAttack(_weaponAttackIndex);
                    View.TryAttack();
                }

                if (_currentBehaviour == _inspectBehaviour)
                {
                    StopCoroutine(_inspectCoroutine);
                    _inspectBehaviour.Clear();
                }

                _followBehaviour.Target = targettedPlayer.transform;
                _currentBehaviour = _followBehaviour;
            }
            else if (_currentBehaviour == _followBehaviour)
            {
                _currentBehaviour = _idleBehaviour;
            }
        }

        private void HandleMovement(Vector2 currentPosition, Vector2? nextPoint)
        {
            if (nextPoint.HasValue && Mathf.Abs(currentPosition.x - nextPoint.Value.x) > _positionComparisonTolerance.x)
            {
                float direction = nextPoint.Value.x - currentPosition.x;
                if (direction != 0)
                {
                    direction /= Mathf.Abs(direction);
                }

                View.Move(direction);
            }
            else
            {
                View.Move(0f);
            }
        }

        private void HandleJump(Vector2 currentPosition, Vector2? nextPoint)
        {
            if (!nextPoint.HasValue || !View.IsGrounded()) return;

            if (Mathf.Abs(currentPosition.y - nextPoint.Value.y) > _positionComparisonTolerance.y)
            {
                if (nextPoint.Value.y > currentPosition.y)
                {
                    RaycastHit2D hit = Physics2D.Raycast(View.transform.position, Vector2.up, _platformToleranceDistance,
                        _platformConfig.PlatformLayerMask);

                    if (hit.collider != null)
                    {
                        View.TryJump();
                    }
                }
                else
                {
                    RaycastHit2D hit = Physics2D.Raycast(View.transform.position, Vector2.up, _platformToleranceDistance,
                        _platformConfig.GroundLayerMask | _platformConfig.PlatformLayerMask);

                    if (hit.collider != null)
                    {
                        View.TryJumpDown();
                    }
                }
            }
        }
    }
}
