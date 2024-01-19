using UnityEngine;

namespace Mdb.EasyTrigger.Presentation.Character.Attack
{
    public abstract class CharacterAttack : MonoBehaviour
    {
        [field: SerializeField] public float Recoil { get; }

        public abstract bool TryAttack();
        public abstract bool TryTarget();
        public abstract void CancelTarget();
        public abstract void OnSwitchTarget();
    }
}
