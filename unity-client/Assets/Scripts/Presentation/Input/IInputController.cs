namespace Mdb.EasyTrigger.Presentation.Input
{
    public interface IInputController
    {
        void Subscribe(IInputListener listener);
        void Unsubscribe(IInputListener listener);
    }
}
