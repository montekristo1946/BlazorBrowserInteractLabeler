namespace BrowserInteractLabeler.Infrastructure;

public class  Mediator<T>
{
    public event EventHandler<T> MessageEvent;
    public void NotifySomethingChanged(T message)
    {
        Task.Run(() =>
        {
            MessageEvent(this, message);
        });
    }
}