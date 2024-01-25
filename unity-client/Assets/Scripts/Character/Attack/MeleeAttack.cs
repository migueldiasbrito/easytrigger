using Mdb.EasyTrigger.Util;
using System;
using System.Collections;
using UnityEngine;

namespace Mdb.EasyTrigger.Character.Attack
{
    public class MeleeAttack : CharacterAttack
    {
        public override int AttackAnimationTrigger => AnimatorUtils.MeleeAttack;
        public override float AttackMovementSpeed => _attackMovementSpeed;
        public override int? TargetAnimationTrigger => null;
        public override bool CanTarget => false;
        public override float Range => _range;
        public override bool MakesSound => false;

        [field: SerializeField] private float _duration;
        [SerializeField] private Collider2D _collider;
        [SerializeField] private float _attackMovementSpeed;
        [SerializeField] private float _range;

        public override IEnumerator TryAttack(Vector2 origin, Vector2 direction, Action callback)
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
