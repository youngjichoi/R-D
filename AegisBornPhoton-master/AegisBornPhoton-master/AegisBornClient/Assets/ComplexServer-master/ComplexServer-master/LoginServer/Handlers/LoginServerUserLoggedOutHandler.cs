using System;
using ComplexServerCommon;
using MMO.Framework;
using MMO.Photon.Application;
using LogManager = ExitGames.Logging.LogManager;
using ExitGames.Logging;
using MMO.Photon.Server;
using SubServerCommon;

namespace LoginServer.Handlers
{
    class LoginServerUserLoggedOutHandler : PhotonServerHandler
    {
        protected static readonly ILogger Log = LogManager.GetCurrentClassLogger();

        public LoginServerUserLoggedOutHandler(PhotonApplication application) : base(application)
        {
        }

        public override MessageType Type
        {
            get { return MessageType.Async; }
        }

        public override byte Code
        {
            get { return (byte) ServerEventCode.UserLoggedOut; }
        }

        public override int? SubCode
        {
            get { return null; }
        }
        //TODO fix me!! this wont catch and we cant see when someone logs out
        protected override bool OnHandleMessage(IMessage message, PhotonServerPeer serverPeer)
        {
            Log.DebugFormat("Handling User Logged Out Message");
            LoginServer server = Server as LoginServer;
            if (server != null)
            {
                Guid peerId = new Guid((Byte[])message.Parameters[(byte)ClientParameterCode.PeerId]);
                server.ConnectionCollection<SubServerConnectionCollection>().Clients.Remove(peerId);
            }
            return true;
        }
    }
}
