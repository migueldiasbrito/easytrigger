using Mdb.EasyTrigger.Presentation.Character;
using UnityEngine;

namespace Mdb.EasyTrigger.Presentation.Enemy
{
    public class EnemyController : MonoBehaviour
    {
        [SerializeField] private CharacterView _view;
        [SerializeField] private IEnemyBehaviour _idleBehaviour;
        [SerializeField] private FollowBehaviour _followBehaviour;

        private IEnemyBehaviour _currentBehaviour { get; set; }

        private void Start()
        {
            _currentBehaviour = _idleBehaviour;
        }

        private void FixedUpdate()
        {
            if (_view.IsDead) return;

            Vector2 currentPosition = _view.transform.position;
            _currentBehaviour.UpdatedPosition(currentPosition);

            Vector2? nextPoint = _currentBehaviour.GetNextPoint();
            if (nextPoint.HasValue)
            {
                float direction = nextPoint.Value.x - currentPosition.x;
                if (direction != 0)
                {
                    direction /= Mathf.Abs(direction);
                }

                _view.Move(direction);
            }
            else
            {
                _view.Move(0f);
            }
        }
    }
}
