namespace Mdb.EasyTrigger.Presentation.Character.Attack
{
    public class MeleeAttack : CharacterAttack
    {
        public override bool TryAttack()
        {
            throw new System.NotImplementedException();
        }

        public override bool TryTarget()
        {
            return false;
        }

        public override void CancelTarget() {}

        public override void OnSwitchTarget() { }
    }
}
