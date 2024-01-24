using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mdb.EasyTrigger.Presentation.Enemy.Behaviours
{
    public abstract class IPathedBehaviour : IEnemyBehaviour
    {
        [SerializeField] private bool _idleThroughPoints;
        [SerializeField] private float _idleTime;

        protected List<Vector2> _path = new List<Vector2>();
        private int _currentPointIndex = 0;
        private bool _idling = false;

        private Vector2 _currentPoint => _path[_currentPointIndex];

        public override Vector2? GetNextPoint()
        {
            if (_idling || _path.Count <= 0) return null;

            return _currentPoint;
        }

        public override void UpdatedPosition(Vector2 position, Vector2 comparisonTolerance)
        {
            if (Mathf.Abs(_currentPoint.x - position.x) <= comparisonTolerance.x
                && Mathf.Abs(_currentPoint.y - position.y) <= comparisonTolerance.y)
            {
                _currentPointIndex = (_currentPointIndex + 1) % _path.Count;

                if (_idleThroughPoints)
                {
                    StartCoroutine(Idle());
                }
            }
        }

        private IEnumerator Idle()
        {
            _idling = true;
            yield return new WaitForSeconds(_idleTime);
            _idling = false;
        }
    }
}
