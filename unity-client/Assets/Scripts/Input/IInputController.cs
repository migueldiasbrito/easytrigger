namespace Mdb.EasyTrigger.Input
{
    public interface IInputController
    {
        void Subscribe(IInputListener listener);
        void Unsubscribe(IInputListener listener);
    }
}
