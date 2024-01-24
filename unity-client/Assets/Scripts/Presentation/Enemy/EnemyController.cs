using Mdb.EasyTrigger.Presentation.Character;
using Mdb.EasyTrigger.Presentation.Config;
using Mdb.EasyTrigger.Presentation.Level;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Mdb.EasyTrigger.Presentation.Enemy
{
    public class EnemyController : MonoBehaviour
    {
        private struct PlayerAwareness
        {
            public bool IsAware;
            public float SqrDistance;
        }

        [field: SerializeField] public CharacterView View { get; private set; }
        [SerializeField] private IEnemyBehaviour _idleBehaviour;
        [SerializeField] private FollowBehaviour _followBehaviour;
        [SerializeField] private Vector2 _positionComparisonTolerance = new Vector2(0.1f, 0.2f);
        [SerializeField] private float _platformToleranceDistance = 5f;

        [SerializeField] private float _playerDetectionDistance = 10f;
        [SerializeField] private float _fieldOfView = 60f;

        private IPlatformConfig _platformConfig;
        private ILevel _level;

        private Dictionary<CharacterView, PlayerAwareness> _playersAwareness
            = new Dictionary<CharacterView, PlayerAwareness>();

        private IEnemyBehaviour _currentBehaviour { get; set; }

        public void Setup(IPlatformConfig platformConfig, ILevel level)
        {
            _platformConfig = platformConfig;
            _level = level;

            View.Setup(_platformConfig, _level);

            foreach (CharacterView player in _level.Players)
            {
                _playersAwareness.Add(player, new PlayerAwareness { SqrDistance = -1.0f });
            }
        }

        private void Start()
        {
            _currentBehaviour = _idleBehaviour;
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
            if (_level == null) return;

            foreach (CharacterView player in _level.Players)
            {
                PlayerAwareness playerAwareness = _playersAwareness[player];

                if (player.IsDead)
                {
                    if (playerAwareness.IsAware)
                    {
                        playerAwareness.IsAware = false;
                        playerAwareness.SqrDistance = -1.0f;
                    }
                }
                else
                {
                    Vector2 distanceToPlayer = (player.Center - View.Center);
                    playerAwareness.SqrDistance = distanceToPlayer.sqrMagnitude;

                    if (!playerAwareness.IsAware)
                    {
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
                }

                _playersAwareness[player] = playerAwareness;
            }

            if (_playersAwareness.Any(playerAwareness => playerAwareness.Value.IsAware))
            {
                Transform playerTransform = null;
                float sqrDistance = 0f;

                foreach (CharacterView player in _level.Players)
                {
                    if (!_playersAwareness[player].IsAware) continue;

                    if (playerTransform == null || _playersAwareness[player].SqrDistance < sqrDistance)
                    {
                        playerTransform = player.transform;
                        sqrDistance = _playersAwareness[player].SqrDistance;
                    }
                }

                _followBehaviour.Target = playerTransform;
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
