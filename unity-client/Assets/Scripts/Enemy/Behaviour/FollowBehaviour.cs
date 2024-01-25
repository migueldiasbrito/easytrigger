using UnityEngine;

namespace Mdb.EasyTrigger.Enemy.Behaviour
{
    public class FollowBehaviour : IEnemyBehaviour
    {
        public Transform Target { get; set; }

        public override Vector2? GetNextPoint()
        {
            if (Target != null)
            {
                return Target.position;
            }
            else
            {
                return null;
            }
        }

        public override void UpdatedPosition(Vector2 position, Vector2 comparisonTolerance) { }
    }
}
