using UnityEngine;

namespace Mdb.EasyTrigger.Presentation.Enemy
{
    public abstract class IEnemyBehaviour : MonoBehaviour
    {
        public abstract Vector2? GetNextPoint();
        public abstract void UpdatedPosition(Vector2 position);
    }
}
