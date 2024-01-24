using UnityEngine;

namespace Mdb.EasyTrigger.Presentation.Enemy.Behaviours
{
    public class InspectBehaviour : IPathedBehaviour
    {
        [SerializeField] private float _distanceToInspectPoint;

        public void Setup(Vector2 inspectPoint, Vector2 currentPosition)
        {
            if (inspectPoint.x >= currentPosition.x)
            {
                _path.Add(inspectPoint + _distanceToInspectPoint * Vector2.right);
                _path.Add(inspectPoint + _distanceToInspectPoint * Vector2.left);
            }
            else
            {
                _path.Add(inspectPoint + _distanceToInspectPoint * Vector2.left);
                _path.Add(inspectPoint + _distanceToInspectPoint * Vector2.right);
            }
        }

        public void Clear()
        {
            _path.Clear();
        }
    }
}
