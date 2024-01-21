using Mdb.EasyTrigger.Presentation.Utils;
using System;
using System.Collections;
using UnityEngine;

namespace Mdb.EasyTrigger.Presentation.Character.Attack
{
    public class WeaponAttack : CharacterAttack
    {
        public override int AttackAnimationTrigger => AnimatorUtils.Shoot;
        public override float AttackMovementSpeed => 0;
        public override int? TargetAnimationTrigger => AnimatorUtils.Target;
        public override bool CanTarget => true;

        [field: SerializeField] private float _recoil;

        public override IEnumerator TryAttack(Action callback)
        {
            throw new System.NotImplementedException();
            yield return new WaitForSeconds(_recoil);
            callback();
        }
    }
}
