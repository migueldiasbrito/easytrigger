namespace Mdb.EasyTrigger.Presentation.Input
{
    public interface IInputListener
    {
        void OnMove(float axisValue);
        void OnJump(bool isKeyPressed);
        void OnAttack();
        void OnTarget();
        void OnSelectAttack(int attackIndex);
        void OnScrollAttacks(float axisValue);
        void OnJumpDown();
    }
}
