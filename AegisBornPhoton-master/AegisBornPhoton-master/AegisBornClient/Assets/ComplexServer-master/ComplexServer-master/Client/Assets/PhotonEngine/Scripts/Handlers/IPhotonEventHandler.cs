using ExitGames.Client.Photon;

public  interface IPhotonEventHandler
{
    byte Code { get; }
    void HandleEvent(EventData eventData);
    void OnHandleEvent(EventData eventData);

}