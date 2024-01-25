using Mdb.EasyTrigger.Character;
using UnityEngine;

namespace Mdb.EasyTrigger.Level
{
    public interface ILevel
    {
        CharacterView[] Players { get; }
        CharacterView[] Enemies { get; }
        Collider2D PlaftformCollider { get; }
        void Shoot(Vector2 point);
    }
}
