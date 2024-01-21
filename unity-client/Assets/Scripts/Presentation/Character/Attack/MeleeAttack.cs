﻿using Mdb.EasyTrigger.Presentation.Utils;
using System;
using System.Collections;
using UnityEngine;

namespace Mdb.EasyTrigger.Presentation.Character.Attack
{
    public class MeleeAttack : CharacterAttack
    {
        public override int AttackAnimationTrigger => AnimatorUtils.MeleeAttack;
        public override float AttackMovementSpeed => _attackMovementSpeed;
        public override int? TargetAnimationTrigger => null;
        public override bool CanTarget => false;

        [field: SerializeField] private float _duration;
        [SerializeField] private Collider2D _collider;
        [SerializeField] private float _attackMovementSpeed;

        public override IEnumerator TryAttack(Action callback)
        {
            _collider.enabled = true;
            yield return new WaitForSeconds(_duration);
            _collider.enabled = false;
            callback();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.transform == transform.parent) return;

            if (collision.TryGetComponent(out CharacterView characterHitted))
            {
                characterHitted.Kill(transform.position);
            }
        }
    }
}
