using Mdb.EasyTrigger.Presentation.Utils;
using System;
using System.Collections;
using UnityEngine;

namespace Mdb.EasyTrigger.Presentation.Character.Attack
{
    public class MeleeAttack : CharacterAttack
    {
        public override int AnimationId => AnimatorUtils.MeleeAttack;

        [field: SerializeField] private float _duration;

        public override IEnumerator TryAttack(Action callback)
        {
            throw new System.NotImplementedException();
            yield return new WaitForSeconds(_duration);
            callback();
        }

        public override bool TryTarget()
        {
            return false;
        }

        public override void CancelTarget() {}

        public override void OnSwitchTarget() { }
    }
}
