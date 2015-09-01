using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;
using UnityEngineInternal;

public class ViewController : IViewController
{
    private readonly byte _subOperationCode;
    private readonly View _controlledView;
    public View ControlledView { get { return _controlledView;  } }

    public Dictionary<byte, IPhotonOperationHandler> _operationHandlers = new Dictionary<byte, IPhotonOperationHandler>();

    public Dictionary<byte, IPhotonEventHandler> _eventHandlers = new Dictionary<byte, IPhotonEventHandler>();


    public ViewController(View controlledView, byte subOperationCode = 0)
    {
        _controlledView = controlledView;
        _subOperationCode = subOperationCode;
        if (PhotonEngine.Instance == null)
        {
            Application.LoadLevel(0);
        }
        else
        {
            PhotonEngine.Instance.Controller = this;
        }
    }

    public Dictionary<byte, IPhotonOperationHandler> OperationHandlers
    
        
        {
            get { return _operationHandlers;}
        }

    public Dictionary<byte, IPhotonEventHandler> EventHandlers


    {
        get { return _eventHandlers; }
    }


    #region Implementation of IViewController

    public bool IsConnected
    {
        get { return PhotonEngine.Instance.State is Connected; }
    }

    public void ApplicationQuit()
    {
        PhotonEngine.Instance.Disconnect();
    }

    public void Connect()
    {
        if (!IsConnected)
        {
            PhotonEngine.Instance.Initialize();
        }
    }

    public void SendOperation(OperationRequest request, bool sendReliable, byte channelId, bool encrypt)
    {
        PhotonEngine.Instance.SendOp(request, sendReliable, channelId, encrypt);
    }

    public void DebugReturn(DebugLevel level, string message)
    {
        ControlledView.LogDebug(string.Format("{0} - {1}", level, message));
    }

    public void OnOperationResponse(OperationResponse operationResponse)
    {
        IPhotonOperationHandler handler;
        if (operationResponse.Parameters.ContainsKey(_subOperationCode) &&
            OperationHandlers.TryGetValue(
                Convert.ToByte(operationResponse.Parameters[_subOperationCode]),
                out handler))
        {
            handler.HandleResponse(operationResponse);
        }
        else
        {
            OnUnexpectedOperationResponse(operationResponse);
        }
    }

    public void OnEvent(EventData eventData)
    {
        IPhotonEventHandler handler;
        if (EventHandlers.TryGetValue(eventData.Code, out handler))
        {
            handler.HandleEvent(eventData);
        }
        else
        {
            OnUnexpectedEvent(eventData);
        }
    }

    public void OnUnexpectedEvent(EventData eventData)
    {
        ControlledView.LogError(string.Format("unexpected event {0}", eventData.Code));
    }

    public void OnUnexpectedOperationResponse(OperationResponse operationResponse)
    {
        ControlledView.LogError(string.Format("unexpected operation error {0} from operation {1}",
            operationResponse.ReturnCode, operationResponse.OperationCode));
    }

    public void OnUnexpectedStatusCode(StatusCode statusCode)
    {
        ControlledView.LogError(string.Format("unexpected Status {0}", statusCode));
    }

    public void OnDisconnected(string message)
    {
        throw new NotImplementedException();
    }

    #endregion
}
