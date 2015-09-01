
    using ExitGames.Client.Photon;

abstract class PhotonEventHandler : IPhotonEventHandler
    {
    protected readonly ViewController _controller;
    public abstract byte Code { get;  }

    protected PhotonEventHandler(ViewController controller)
    {
        _controller = controller;
    }

    public delegate void BeforeEventReceived();
    public BeforeEventReceived beforeEventReceived;

    public delegate void AfterEventReceived();
    public AfterEventReceived afterEventReceived;

    public void HandleEvent(EventData eventData)
    {
        if (beforeEventReceived != null)
        {
            beforeEventReceived();
        }
        OnHandleEvent(eventData);
        if (afterEventReceived != null)
        {
            afterEventReceived();
        }
    }

    public abstract void OnHandleEvent(EventData eventData);

    }

