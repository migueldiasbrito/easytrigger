using Mdb.EasyTrigger.Presentation.Utils;
using UnityEngine;

namespace Mdb.EasyTrigger.Presentation.Character
{

    public class CharacterView : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D _rigidbody;
        [SerializeField] private Animator _animator;
        [SerializeField] private float _walkSpeed;

        private float _moveDirection = 0f;

        public void Move(float direction)
        {
            _moveDirection = direction;
        }

        private void FixedUpdate()
        {
            Vector2 velocity = _rigidbody.velocity;

            velocity.x = _moveDirection * _walkSpeed * Time.fixedDeltaTime;

            _rigidbody.velocity = velocity;

            _animator.SetBool(AnimatorUtils.Walking, _moveDirection != 0);
            _animator.SetBool(AnimatorUtils.LookRight, _moveDirection >= 0);
        }
    }
}
