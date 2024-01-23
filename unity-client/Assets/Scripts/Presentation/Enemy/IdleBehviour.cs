using UnityEngine;

namespace Mdb.EasyTrigger.Presentation.Enemy
{
    public class IdleBehviour : IEnemyBehaviour
    {
        public override Vector2? GetNextPoint()
        {
            return null;
        }

        public override void UpdatedPosition(Vector2 position, Vector2 comparisonTolerance) {}
    }
}
