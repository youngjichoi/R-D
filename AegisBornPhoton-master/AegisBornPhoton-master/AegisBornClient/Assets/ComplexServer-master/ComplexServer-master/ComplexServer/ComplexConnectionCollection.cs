using System;
using System.Collections.Generic;
using System.Linq;
using ComplexServerCommon;
using ExitGames.Logging;
using MMO.Photon.Application;
using MMO.Photon.Client;
using MMO.Photon.Server;
using Photon.SocketServer;
using SubServerCommon;
using SubServerCommon.ClientData;

namespace ComplexServer
{
    public class ComplexConnectionCollection : PhotonConnectionCollection
    {
        public PhotonServerPeer LoginServer { get; protected set; }
        public PhotonServerPeer ChatServer { get; protected set; }
        protected static readonly ILogger Log = LogManager.GetCurrentClassLogger();

        public ComplexConnectionCollection()
        {
            LoginServer = null;
            ChatServer = null;
        }

        public override void Disconnect(PhotonServerPeer serverPeer)
        {
            if (serverPeer.ServerId.HasValue)
            {
                if (ChatServer != null && serverPeer.ServerId.Value == ChatServer.ServerId)
                {
                    ChatServer = null;
                }
                if (LoginServer != null && serverPeer.ServerId.Value == LoginServer.ServerId)
                {
                    LoginServer = null;
                }
            }
        }

        public override void Connect(PhotonServerPeer serverPeer)
        {
            if ((serverPeer.ServerType & (int) ServerType.Region) != 0)
            {
                var parameters = new Dictionary<byte, object>();
                var serverList = Servers.Where(
                    incomingSubServerPeer =>
                        incomingSubServerPeer.Value.ServerId.HasValue &&
                        !incomingSubServerPeer.Value.ServerId.Equals(serverPeer.ServerId) &&
                        (incomingSubServerPeer.Value.ServerType & (int) ServerType.Region) != 0)
                    .ToDictionary(incomingSubServerPeer => incomingSubServerPeer.Value.ApplicationName,
                        incomingSubServerPeer => incomingSubServerPeer.Value.TcpAddress);
                if (serverList.Count > 0)
                {
                    if (Log.IsDebugEnabled)
                    {
                        Log.DebugFormat("Sending list of {0} connected sub servers ", serverList.Count);
                    }
                    parameters.Add((byte) ServerParameterCode.SubServerDictionary, serverList);
                    serverPeer.SendEvent(new EventData((byte) ServerEventCode.SubServerList, parameters),
                        new SendParameters());
                }
            }
        }

        public override void ClientConnect(PhotonClientPeer clientPeer)
        {
            if (clientPeer.ClientData<CharacterData>().CharacterId.HasValue)
            {
                var para = new Dictionary<byte, object>
                {
                    {(byte) ClientParameterCode.CharacterId, clientPeer.ClientData<CharacterData>().CharacterId.Value},
                    {(byte) ClientParameterCode.PeerId, clientPeer.PeerId}
                };
                if (ChatServer != null)
                {
                    ChatServer.SendEvent(new EventData((byte)ServerEventCode.CharacterRegister, para),
                  new SendParameters());
                }
                if (clientPeer.CurrentServer != null)
                {
                    clientPeer.CurrentServer.SendEvent(
                  new EventData((byte)ServerEventCode.CharacterRegister, para), new SendParameters());
                }
              
            }
        }

        public override void ClientDisconnect(PhotonClientPeer clientPeer)
        {
            var para = new Dictionary<byte, object>
            {
                {(byte) ClientParameterCode.PeerId, clientPeer.PeerId.ToByteArray()}
            };

            if (clientPeer.ClientData<CharacterData>().CharacterId.HasValue)
            {
                Log.DebugFormat("Sending disconnect for client {0}:{1}", clientPeer.PeerId,
                    clientPeer.ClientData<CharacterData>().CharacterId.Value);
                if (ChatServer != null)
                {
                    ChatServer.SendEvent(new EventData((byte)ServerEventCode.CharacterDeRegister, para),
                   new SendParameters());
                }
                if (clientPeer.CurrentServer != null)
                {
                    clientPeer.CurrentServer.SendEvent(
                    new EventData((byte)ServerEventCode.CharacterDeRegister, para), new SendParameters());
                }
                
            }

            Log.DebugFormat("xxx sending user loggedout sendevent");
            LoginServer.SendEvent(new EventData((byte) ServerEventCode.UserLoggedOut, para), new SendParameters());
        }

        public override void ResetServers()
        {
            Log.DebugFormat("Reseting servers");
            if (ChatServer != null && ChatServer.ServerType != (int) ServerType.Chat)
            {
                var peer =
                    Servers.Values.Where(subServerPeer => subServerPeer.ServerType == (int) ServerType.Chat)
                        .FirstOrDefault();
                if (peer != null)
                {
                    ChatServer = peer;
                }
            }

            if (LoginServer != null && LoginServer.ServerType != (int) ServerType.Login)
            {
                Log.DebugFormat("Reseting login");

                var peer =
                    Servers.Values.Where(subServerPeer => subServerPeer.ServerType == (int) ServerType.Login)
                        .FirstOrDefault();
                if (peer != null)
                {
                    LoginServer = peer;
                }
            }

            if (ChatServer == null || ChatServer.ServerId == null)
            {
                ChatServer =
                    Servers.Values.Where(subServerPeer => subServerPeer.ServerType == (int) ServerType.Chat)
                        .FirstOrDefault() ??
                    Servers.Values.Where(subServerPeer => (subServerPeer.ServerType & (int) ServerType.Chat) != 0)
                        .FirstOrDefault();
            }

            if (LoginServer == null || LoginServer.ServerId == null)
            {
                Log.DebugFormat("Reseting login");

                LoginServer =
                    Servers.Values.Where(subServerPeer => subServerPeer.ServerType == (int) ServerType.Login)
                        .FirstOrDefault() ??
                    Servers.Values.Where(subServerPeer => (subServerPeer.ServerType & (int) ServerType.Login) != 0)
                        .FirstOrDefault();
            }

            if (LoginServer != null)
            {
                Log.DebugFormat("LoginSErver : {0}", LoginServer.ConnectionId);
            }
            if (ChatServer != null)
            {
                Log.DebugFormat("ChatServer  : {0}", ChatServer.ConnectionId);
            }
        }

        public override bool IsServerPeer(InitRequest initRequest)
        {
            Log.DebugFormat("Received init request to {0}:{1} - {2}", initRequest.LocalIP, initRequest.LocalPort,
                initRequest);
            if (initRequest.LocalPort == 4520)
            {
                return true;
            }
            return false;
        }

        public override PhotonServerPeer OnGetSeverByType(int serverType)
        {
            PhotonServerPeer server = null;
            switch ((ServerType) Enum.ToObject(typeof (ServerType), serverType))
            {
                case ServerType.Login:
                    if (LoginServer != null)
                    {
                        Log.DebugFormat("Found login server");
                        server = LoginServer;
                    }
                    else
                    {
                        Log.DebugFormat("Login server does not exist in collection!");
                    }
                    break;
                case ServerType.Chat:
                    if (ChatServer != null)
                    {
                        Log.DebugFormat("Found Chat Server");
                        server = ChatServer;
                    }
                    break;
                default:
                    Log.DebugFormat("oh shit this server type is fucked up");
                    break;
            }
            return server;
        }

        public override void DisconnectAll()
        {
            foreach (var photonServerPeer in Servers)
            {
                photonServerPeer.Value.Disconnect();
            }
            foreach (var photonClientPeer in Clients)
            {
                photonClientPeer.Value.Disconnect();
            }
        }
    }
}
