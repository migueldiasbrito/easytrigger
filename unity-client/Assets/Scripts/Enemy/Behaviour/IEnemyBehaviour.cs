using UnityEngine;

namespace Mdb.EasyTrigger.Enemy.Behaviour
{
    public abstract class IEnemyBehaviour : MonoBehaviour
    {
        public abstract Vector2? GetNextPoint();
        public abstract void UpdatedPosition(Vector2 position, Vector2 comparisonTolerance);
    }
}
