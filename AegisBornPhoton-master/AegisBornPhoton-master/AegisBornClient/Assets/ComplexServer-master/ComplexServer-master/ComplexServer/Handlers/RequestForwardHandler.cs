using System;
using ComplexServerCommon;
using MMO.Framework;
using MMO.Photon.Application;
using MMO.Photon.Client;
using MMO.Photon.Server;
using Photon.SocketServer;

namespace ComplexServer.Handlers
{
    public class RequestForwardHandler : DefaultRequestHandler
    {
        public RequestForwardHandler(PhotonApplication application) : base(application)
        {
        }

        public override MessageType Type
        {
            get { return MessageType.Request; }
        }

        public override byte Code
        {
            get { return (byte)(ClientOperationCode.Chat | ClientOperationCode.Login | ClientOperationCode.Region); }

        }

        public override int? SubCode
        {
            get { return null; }
        }

        protected override bool OnHandleMessage(IMessage message, PhotonServerPeer serverPeer)
        {
            Log.ErrorFormat("Looking for Peer Id {0}", new Guid((Byte[])message.Parameters[(byte)ClientParameterCode.PeerId]));
            PhotonClientPeer peer;
            Server.ConnectionCollection<PhotonConnectionCollection>().Clients.TryGetValue(
                new Guid((Byte[]) message.Parameters[(byte) ClientParameterCode.PeerId]), out peer);
            if (peer != null)
            {
                Log.DebugFormat("Found Peer");
                message.Parameters.Remove((byte) ClientParameterCode.PeerId);
                var response = message as PhotonResponse;
                if (response != null)
                {
                    peer.SendOperationResponse(new OperationResponse(response.Code, response.Parameters)
                    {
                        DebugMessage = response.DebugMessage,
                        ReturnCode = response.ReturnCode
                    }, new SendParameters());
                }
                else
                {
                    peer.SendOperationResponse(new OperationResponse(message.Code, message.Parameters),
                        new SendParameters());
                }
            }
            return true;
        }
    }
}