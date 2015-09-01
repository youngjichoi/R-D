using System;
using ComplexServerCommon;
using MMO.Framework;
using MMO.Photon.Application;
using MMO.Photon.Client;
using MMO.Photon.Server;
using Photon.SocketServer;
using SubServerCommon.ClientData;

namespace ComplexServer.Handlers
{
    public class LoginReponseHandler : PhotonServerHandler
    {
        public LoginReponseHandler(PhotonApplication application) : base(application)
        {
        }

        public override MessageType Type
        {
            get { return MessageType.Response; }
        }

        public override byte Code
        {
            get { return (byte) ClientOperationCode.Login; }
        }

        public override int? SubCode
        {
            get { return (int) MessageSubCode.Login; }
        }

        protected override bool OnHandleMessage(IMessage message, PhotonServerPeer serverPeer)
        {
            Log.DebugFormat("LoginResponseHandler.OnHandleMessage with code {0}", message.Code);
            if (message.Parameters.ContainsKey((byte) ClientParameterCode.PeerId))
            {
                PhotonClientPeer peer;
                Server.ConnectionCollection<PhotonConnectionCollection>().Clients.TryGetValue(
                    new Guid((Byte[]) message.Parameters[(byte) ClientParameterCode.PeerId]), out peer);

                if (peer != null)
                {
                    //puts userid into characterData so we dont worry about what their name and shit is
                    if (message.Parameters.ContainsKey((byte) ClientParameterCode.UserId))
                    {
                        Log.DebugFormat("Found user {0}",Convert.ToInt32(message.Parameters[(byte) ClientParameterCode.UserId]));
                        peer.ClientData<CharacterData>().UserId = Convert.ToInt32(message.Parameters[(byte) ClientParameterCode.UserId]);
                    }

                    message.Parameters.Remove((byte) ClientParameterCode.PeerId);
                    message.Parameters.Remove((byte) ClientParameterCode.CharacterId);
                    message.Parameters.Remove((byte) ClientParameterCode.UserId);

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
            }
            return true;
        }
    }
}
