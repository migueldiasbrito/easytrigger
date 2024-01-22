using Mdb.EasyTrigger.Presentation.Character;
using UnityEngine;

namespace Mdb.EasyTrigger.Presentation.Level
{
    public interface ILevel
    {
        CharacterView[] Enemies { get; }
        void PlaySound(AudioClip audioClip);
    }
}
