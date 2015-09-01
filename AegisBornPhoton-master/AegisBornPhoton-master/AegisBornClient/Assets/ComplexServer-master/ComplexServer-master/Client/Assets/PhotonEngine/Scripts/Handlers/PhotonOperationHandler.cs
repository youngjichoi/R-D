

using ExitGames.Client.Photon;

public abstract class PhotonOperationHandler : IPhotonOperationHandler
{
    protected readonly ViewController _controller;
    public abstract byte Code { get; }

    protected PhotonOperationHandler(ViewController controller)
    {
        _controller = controller;
    }

    public delegate void BeforeResponseReceived();

    public BeforeResponseReceived beforeResponseReceived;

    public delegate void AfterResponseReceived();

    public AfterResponseReceived afterResponseReceived;

    public void HandleResponse(OperationResponse response)
    {
        if (beforeResponseReceived != null)
        {
            beforeResponseReceived();
        }
        OnHandleResponse(response);
        if (afterResponseReceived != null)
        {
            afterResponseReceived();
        }
    }

    public abstract void OnHandleResponse(OperationResponse response);
}

