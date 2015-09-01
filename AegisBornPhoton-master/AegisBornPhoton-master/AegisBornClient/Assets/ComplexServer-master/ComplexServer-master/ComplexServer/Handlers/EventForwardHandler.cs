using System;
using ComplexServerCommon;
using MMO.Framework;
using MMO.Photon.Application;
using MMO.Photon.Client;
using MMO.Photon.Server;
using Photon.SocketServer;

namespace ComplexServer.Handlers
{
    public class EventForwardHandler : DefaultEventHandler
    {
        public EventForwardHandler(PhotonApplication application) : base(application)
        {
        }

        public override MessageType Type
        {
            get { return MessageType.Async; }
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
            if (message.Parameters.ContainsKey((byte) ClientParameterCode.PeerId))
            {
                PhotonClientPeer peer;
                Server.ConnectionCollection<PhotonConnectionCollection>().Clients.TryGetValue(
                    new Guid((Byte[]) message.Parameters[(byte) ClientParameterCode.PeerId]), out peer);
                if (peer != null)
                {
                    message.Parameters.Remove((byte) ClientParameterCode.PeerId);
                    peer.SendEvent(new EventData(message.Code, message.Parameters), new SendParameters());
                }

            }
            return true;
        }
    }
}