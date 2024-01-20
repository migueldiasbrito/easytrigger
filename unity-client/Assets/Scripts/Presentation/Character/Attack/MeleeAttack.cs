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
        [SerializeField] private Collider2D _collider;

        public override IEnumerator TryAttack(Action callback)
        {
            _collider.enabled = true;
            yield return new WaitForSeconds(_duration);
            _collider.enabled = false;
            callback();
        }

        public override bool TryTarget()
        {
            return false;
        }

        public override void CancelTarget() {}

        public override void OnSwitchTarget() { }

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
