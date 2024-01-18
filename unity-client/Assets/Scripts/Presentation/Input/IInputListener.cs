namespace Mdb.EasyTrigger.Presentation.Input
{
    public interface IInputListener
    {
        void OnMove(float axisValue);
        void OnJump(bool isKeyPressed);
    }
}
