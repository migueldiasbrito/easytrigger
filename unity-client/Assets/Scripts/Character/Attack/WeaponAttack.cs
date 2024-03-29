﻿using Mdb.EasyTrigger.Util;
using System;
using System.Collections;
using UnityEngine;

namespace Mdb.EasyTrigger.Character.Attack
{
    public class WeaponAttack : CharacterAttack
    {
        public override int AttackAnimationTrigger => AnimatorUtils.Shoot;
        public override float AttackMovementSpeed => 0;
        public override int? TargetAnimationTrigger => AnimatorUtils.Targeting;
        public override bool CanTarget => true;
        public override float Range => _range;
        public override bool MakesSound => true;

        [SerializeField] private float _recoil;
        [SerializeField] private float _range;
        [SerializeField] private AudioClip _audioClip;

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
