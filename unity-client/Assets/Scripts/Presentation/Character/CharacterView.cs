using Mdb.EasyTrigger.Presentation.Config;
using Mdb.EasyTrigger.Presentation.Utils;
using UnityEngine;

namespace Mdb.EasyTrigger.Presentation.Character
{

    public class CharacterView : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D _rigidbody;
        [SerializeField] private Animator _animator;
        [SerializeField] private float _walkSpeed = 100f;
        [SerializeField] private float _jumpSpeed = 7.5f;
        [SerializeField] private float _groundCheckDistance = 0.1f;

        private IPlatformConfig _platformConfig;

        private float _moveDirection = 0f;
        private bool _tryJump = false;
        private bool _tryCancelJump = false;

#if UNITY_EDITOR
        [SerializeField] private bool _isGrounded;
#endif

        public void Setup(IPlatformConfig platformConfig)
        {
            _platformConfig = platformConfig;
        }

        public void Move(float direction)
        {
            _moveDirection = direction;
        }

        public void TryJump()
        {
            _tryJump = true;
        }

        public void TryCancelJump()
        {
            _tryCancelJump = true;
        }

        private void FixedUpdate()
        {
            Vector2 velocity = _rigidbody.velocity;

            velocity.x = _moveDirection * _walkSpeed * Time.fixedDeltaTime;

            bool isGrounded = IsGrounded();

            if (isGrounded && _tryJump)
            {
                velocity.y = _jumpSpeed;
            }
            else if (velocity.y > 0 && _tryCancelJump)
            {
                velocity.y = 0;
            }

            _rigidbody.velocity = velocity;

#if UNITY_EDITOR
            _isGrounded = isGrounded;
#endif

            _animator.SetBool(AnimatorUtils.Walking, _moveDirection != 0);
            _animator.SetBool(AnimatorUtils.LookRight, _moveDirection >= 0);
            _animator.SetBool(AnimatorUtils.Falling, velocity.y < -_platformConfig.JumpAnimationVelocityThreshold);
            _animator.SetBool(AnimatorUtils.Jumping, velocity.y > _platformConfig.JumpAnimationVelocityThreshold);

            _tryJump = false;
            _tryCancelJump = false;
    }

        private bool IsGrounded()
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, _groundCheckDistance,
                _platformConfig.GroundLayerMask);
            return hit.collider != null;
        }
    }
}
