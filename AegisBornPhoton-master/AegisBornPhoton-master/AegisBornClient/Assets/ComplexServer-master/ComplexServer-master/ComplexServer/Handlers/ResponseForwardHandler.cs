using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ComplexServerCommon;
using MMO.Framework;
using MMO.Photon.Application;
using MMO.Photon.Client;
using MMO.Photon.Server;
using Photon.SocketServer;

namespace ComplexServer.Handlers
{
    public class ResponseForwardHandler : DefaultResponseHandler
    {

        public ResponseForwardHandler(PhotonApplication application) : base(application)
        {
            
        }

        public override MessageType Type
        {
            get { return MessageType.Response; }
        }

        public override byte Code
        {
            get { return (byte) (ClientOperationCode.Chat | ClientOperationCode.Login | ClientOperationCode.Region); }
        }

        public override int? SubCode
        {
            get { return null; }
        }

        protected override bool OnHandleMessage(IMessage message, PhotonServerPeer serverPeer)
        {
            if (message.Parameters.ContainsKey((byte) ClientParameterCode.PeerId))
            {
                Log.DebugFormat("looking for peerId {0}", new Guid((Byte[])message.Parameters[(byte)ClientParameterCode.PeerId]));
                PhotonClientPeer peer;
                Server.ConnectionCollection<PhotonConnectionCollection>().Clients.TryGetValue(
                    new Guid((Byte[]) message.Parameters[(byte) ClientParameterCode.PeerId]), out peer);

                if (peer != null)
                {
                    Log.DebugFormat("found peer");
                    message.Parameters.Remove((byte) ClientParameterCode.PeerId);
                    message.Parameters.Remove((byte)ClientParameterCode.UserId);
                    message.Parameters.Remove((byte)ClientParameterCode.CharacterId);
                    var response = message as PhotonResponse;
                    if (response != null)
                    {
                        peer.SendOperationResponse(
                            new OperationResponse(response.Code, response.Parameters)
                            {
                                DebugMessage = response.DebugMessage,
                                ReturnCode = response.ReturnCode
                            },
                            new SendParameters());

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
