using Mdb.EasyTrigger.Presentation.Character;
using Mdb.EasyTrigger.Presentation.Config;
using Mdb.EasyTrigger.Presentation.Level;
using UnityEngine;

namespace Mdb.EasyTrigger.Presentation.Enemy
{
    public class EnemyController : MonoBehaviour
    {
        [field: SerializeField] public CharacterView View { get; private set; }
        [SerializeField] private IEnemyBehaviour _idleBehaviour;
        [SerializeField] private FollowBehaviour _followBehaviour;
        [SerializeField] private Vector2 _positionComparisonTolerance = new Vector2(0.1f, 0.2f);
        [SerializeField] private float _platformToleranceDistance = 5f;

        private IPlatformConfig _platformConfig;
        private ILevel _level;

        private IEnemyBehaviour _currentBehaviour { get; set; }

        public void Setup(IPlatformConfig platformConfig, ILevel level)
        {
            _platformConfig = platformConfig;
            _level = level;

            View.Setup(_platformConfig, _level);
        }

        private void Start()
        {
            _currentBehaviour = _idleBehaviour;
        }

        private void FixedUpdate()
        {
            if (View.IsDead) return;

            Vector2 currentPosition = View.transform.position;
            _currentBehaviour.UpdatedPosition(currentPosition, _positionComparisonTolerance);

            Vector2? nextPoint = _currentBehaviour.GetNextPoint();
            HandleMovement(currentPosition, nextPoint);
            HandleJump(currentPosition, nextPoint);
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
