using UnityEngine;

namespace Mdb.EasyTrigger.Presentation.Utils
{
    public static class AnimatorUtils
    {
        public static int Walking = Animator.StringToHash("Walking");
        public static int LookRight = Animator.StringToHash("LookRight");
        public static int Falling = Animator.StringToHash("Falling");
        public static int Jumping = Animator.StringToHash("Jumping");
        public static int MeleeAttack = Animator.StringToHash("MeleeAttack");
        public static int Shoot = Animator.StringToHash("Shoot");
        public static int FrontHit = Animator.StringToHash("FrontHit");
        public static int BackHit = Animator.StringToHash("BackHit");
        public static int Targeting = Animator.StringToHash("Targeting");
        public static int Targeted = Animator.StringToHash("Targeted");
        public static int InRange = Animator.StringToHash("InRange");
        public static int PlayerTargeting = Animator.StringToHash("PlayerTargeting");
    }
}
