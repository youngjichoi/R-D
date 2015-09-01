using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ComplexServerCommon;
using MMO.Framework;
using MMO.Photon.Application;
using MMO.Photon.Server;
using SubServerCommon;

namespace ChatServer.Handlers
{
    public class ChatServerDeregisterEventHandler : PhotonServerHandler
    {
        public ChatServerDeregisterEventHandler(PhotonApplication application) : base(application)
        {
        }

        public override MessageType Type
        {
            get { return MessageType.Async; }
        }

        public override byte Code
        {
            get { return (byte) ServerEventCode.CharacterDeRegister; }
        }

        public override int? SubCode
        {
            get { return null; }
        }

        protected override bool OnHandleMessage(IMessage message, PhotonServerPeer serverPeer)
        {
            Guid peerId = new Guid((Byte[])message.Parameters[(byte)ClientParameterCode.PeerId]);
            //TODO remove from groups? guilds etc.
            Server.ConnectionCollection<SubServerConnectionCollection>().Clients.Remove(peerId);
            Log.DebugFormat("Removed peer {0}, now we have {1} clients",peerId, Server.ConnectionCollection<SubServerConnectionCollection>().Clients.Count);
            return true;
        }
    }
}
