using System;
using System.Collections.Generic;

using ComplexServerCommon;
using MMO.Framework;
using MMO.Photon.Application;
using MMO.Photon.Client;
using MMO.Photon.Server;
using Photon.SocketServer;
using SubServerCommon;
using SubServerCommon.ClientData;

namespace ComplexServer.Handlers
{
    public class SelectCharacterResponseHandler : PhotonServerHandler
    {
        public SelectCharacterResponseHandler(PhotonApplication application) : base(application)
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
            get { return (byte) MessageSubCode.SelectCharacter; }
        }

        protected override bool OnHandleMessage(IMessage message, PhotonServerPeer serverPeer)
        {
            if (message.Parameters.ContainsKey((byte) ClientParameterCode.PeerId))
            {
                PhotonClientPeer peer;
                Server.ConnectionCollection<ComplexConnectionCollection>()
                    .Clients.TryGetValue(new Guid((Byte[]) message.Parameters[(byte) ClientParameterCode.PeerId]),
                        out peer);
                if (peer != null)
                {
                    int characterId = Convert.ToInt32(message.Parameters[(byte) ClientParameterCode.CharacterId]);
                    peer.ClientData<CharacterData>().CharacterId = characterId;

                    var para = new Dictionary<byte, object>
                    {
                        {(byte) ClientParameterCode.CharacterId, characterId},
                        {(byte) ClientParameterCode.PeerId, new Guid((Byte[]) message.Parameters[(byte) ClientParameterCode.PeerId]).ToByteArray()},
                        {(byte)ClientParameterCode.UserId, peer.ClientData<CharacterData>().UserId}
                    };
                    var chatServer = Server.ConnectionCollection<ComplexConnectionCollection>().OnGetSeverByType((int) ServerType.Chat);
                    if (chatServer != null)
                    {
                        chatServer.SendEvent(new EventData((byte) ServerEventCode.CharacterRegister) {Parameters = para},new SendParameters());
                    }
                    //TODO add code to send same event to Region server

                    message.Parameters.Remove((byte) ClientParameterCode.PeerId);
                    message.Parameters.Remove((byte) ClientParameterCode.UserId);
                    message.Parameters.Remove((byte) ClientParameterCode.CharacterId);

                    var response = message as PhotonResponse;
                    if (response != null)
                    {
                        peer.SendOperationResponse(
                            new OperationResponse(response.Code, response.Parameters)
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
