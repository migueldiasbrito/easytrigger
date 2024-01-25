using Mdb.EasyTrigger.Character;
using UnityEngine;

namespace Mdb.EasyTrigger.Level
{
    public interface ILevel
    {
        CharacterView[] Enemies { get; }
        Collider2D PlatformCollider { get; }
    }
}
