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
        public override int? TargetAnimationTrigger => AnimatorUtils.Targeting;
        public override bool CanTarget => true;
        public override float Range => _range;

        [SerializeField] private float _recoil;
        [SerializeField] private float _range;

        public override IEnumerator TryAttack(Vector2 origin, Vector2 direction, Action callback)
        {
            RaycastHit2D[] hits = Physics2D.RaycastAll(origin, direction, _range);

            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].transform == transform.parent) continue;

                if (hits[i].collider.TryGetComponent(out CharacterView character))
                {
                    character.Kill(origin);
                    break;
                }
            }
            
            yield return new WaitForSeconds(_recoil);
            callback();
        }
    }
}
