using System;
using System.Linq;
using ComplexServerCommon;
using MMO.Framework;
using MMO.Photon.Application;
using MMO.Photon.Server;
using MMO.PhotonFramework.Client;
using SubServerCommon;
using SubServerCommon.ClientData;
using SubServerCommon.Data.NHibernate;

namespace ChatServer.Handlers
{
    public class ChatServerRegisterEventHandler : PhotonServerHandler
    {

        private readonly SubServerClientPeer.Factory _clientFactory;

        public ChatServerRegisterEventHandler(PhotonApplication application, SubServerClientPeer.Factory clientFactory) : base(application)
        {
            _clientFactory = clientFactory;
        }

        public override MessageType Type
        {
            get { return MessageType.Async; }
        }

        public override byte Code
        {
            get { return (byte) ServerEventCode.CharacterRegister; }
        }

        public override int? SubCode
        {
            get { return null; }
        }

        protected override bool OnHandleMessage(IMessage message, PhotonServerPeer serverPeer)
        {
            int characterId = Convert.ToInt32(message.Parameters[(byte) ClientParameterCode.CharacterId]);
            Guid peerId = new Guid((Byte[])message.Parameters[(byte)ClientParameterCode.PeerId]);
            Log.DebugFormat("character {0} peer {1}", characterId, peerId);
            try
            {
                using (var session = NHibernateHelper.OpenSession())
                {
                    using (var transaction = session.BeginTransaction())
                    {
                        var character =
                            session.QueryOver<ComplexCharacter>()
                                .Where(cc => cc.Id == characterId)
                                .List()
                                .FirstOrDefault();

                        transaction.Commit();
                        var clients = Server.ConnectionCollection<SubServerConnectionCollection>().Clients;
                        clients.Add(peerId, _clientFactory());
                        // TODO Add character data to the cl;ient list for chat
                        clients[peerId].ClientData<CharacterData>().CharacterId = character.Id;
                        clients[peerId].ClientData<CharacterData>().UserId = Convert.ToInt32(message.Parameters[(byte) ClientParameterCode.UserId]);

                        //Notify guild members that someone logged in
                        //Notify friends list that someone logged in 
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
            return true;
        }
    }
}
