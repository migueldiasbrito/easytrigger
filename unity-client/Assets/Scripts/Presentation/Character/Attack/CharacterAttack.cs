using System;
using System.Collections;
using UnityEngine;

namespace Mdb.EasyTrigger.Presentation.Character.Attack
{
    public abstract class CharacterAttack : MonoBehaviour
    {
        public abstract int AnimationId { get; }

        public abstract IEnumerator TryAttack(Action callback);
        public abstract bool TryTarget();
        public abstract void CancelTarget();
        public abstract void OnSwitchTarget();
    }
}
