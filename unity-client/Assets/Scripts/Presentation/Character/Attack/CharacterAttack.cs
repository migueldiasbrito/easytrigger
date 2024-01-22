using System;
using System.Collections;
using UnityEngine;

namespace Mdb.EasyTrigger.Presentation.Character.Attack
{
    public abstract class CharacterAttack : MonoBehaviour
    {
        public abstract int AttackAnimationTrigger { get; }
        public abstract float AttackMovementSpeed { get; }
        public abstract int? TargetAnimationTrigger { get; }
        public abstract bool CanTarget { get; }
        public abstract float Range { get; }

        public abstract IEnumerator TryAttack(Vector2 origin, Vector2 direction, Action callback);
    }
}
