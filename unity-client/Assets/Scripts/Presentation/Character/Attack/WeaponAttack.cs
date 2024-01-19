using Mdb.EasyTrigger.Presentation.Utils;
using System;
using System.Collections;
using UnityEngine;

namespace Mdb.EasyTrigger.Presentation.Character.Attack
{
    public class WeaponAttack : CharacterAttack
    {
        public override int AnimationId => AnimatorUtils.Shoot;

        [field: SerializeField] private float _recoil;

        public override IEnumerator TryAttack(Action callback)
        {
            throw new System.NotImplementedException();
            yield return new WaitForSeconds(_recoil);
            callback();
        }

        public override bool TryTarget()
        {
            throw new System.NotImplementedException();
            return true;
        }

        public override void CancelTarget()
        {
            throw new System.NotImplementedException();
        }

        public override void OnSwitchTarget()
        {
            throw new System.NotImplementedException();
        }
    }
}
