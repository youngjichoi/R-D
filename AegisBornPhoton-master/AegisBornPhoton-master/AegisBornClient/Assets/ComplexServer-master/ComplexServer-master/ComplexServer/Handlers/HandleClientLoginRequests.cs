
using System;
using System.ComponentModel;
using ComplexServerCommon;
using MMO.Framework;
using MMO.Photon.Application;
using MMO.Photon.Client;
using Photon.SocketServer;
using SubServerCommon;
using SubServerCommon.ClientData;

namespace ComplexServer.Handlers
{
    public class HandleClientLoginRequests :  PhotonClientHandler
    {
        public HandleClientLoginRequests(PhotonApplication application) : base(application)
        {
        }

        public override MessageType Type
        {
            get { return MessageType.Async | MessageType.Request | MessageType.Response; }
        }

        public override byte Code
        {
            get { return  (byte) ClientOperationCode.Login; }
        }

        public override int? SubCode
        {
            get { return null; }
        }

        protected override bool OnHandleMessage(IMessage message, PhotonClientPeer peer)
        {
            Log.DebugFormat("Here we are handling a message with code {0}", message.Code);
            message.Parameters.Remove((byte) ClientParameterCode.PeerId);
            message.Parameters.Add((byte)ClientParameterCode.PeerId, peer.PeerId.ToByteArray());
            message.Parameters.Remove((byte) ClientParameterCode.UserId);
            if (peer.ClientData<CharacterData>().UserId.HasValue)
            {
                message.Parameters.Add((byte)ClientParameterCode.UserId, peer.ClientData<CharacterData>().UserId);
            }

            if (peer.ClientData<CharacterData>().CharacterId.HasValue)
            {
                message.Parameters.Remove((byte)ClientParameterCode.CharacterId);
                message.Parameters.Add((byte)ClientParameterCode.CharacterId, peer.ClientData<CharacterData>().CharacterId);

            }
            var operationRequest = new OperationRequest(message.Code, message.Parameters);
            switch (message.Code)
            {
                case (byte)ClientOperationCode.Login:
                    if (Server.ConnectionCollection<PhotonConnectionCollection>() != null)
                    {
                        Server.ConnectionCollection<PhotonConnectionCollection>().GetServerByType((int) ServerType.Login)
                            .SendOperationRequest(operationRequest, new SendParameters());
                    }
                    break;
                default:
                    Log.DebugFormat("invalid operation code expecting login");
                    break;
            }

            return true; 
        }
    }
}
