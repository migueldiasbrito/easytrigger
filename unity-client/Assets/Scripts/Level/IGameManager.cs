using Mdb.EasyTrigger.Character;
using UnityEngine;

namespace Mdb.EasyTrigger.Level
{
    public interface IGameManager
    {
        ILevel CurrentLevel { get; }
        CharacterView[] Players { get; }
        void Shoot(Vector2 point);
    }
}
