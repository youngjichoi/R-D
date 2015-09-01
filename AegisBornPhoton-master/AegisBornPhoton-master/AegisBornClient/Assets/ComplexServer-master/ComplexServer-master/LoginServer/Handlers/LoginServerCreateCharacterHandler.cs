using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using ComplexServerCommon;
using ComplexServerCommon.MessageObjects;
using LoginServer.Operations;
using MMO.Framework;
using MMO.Photon.Application;
using MMO.Photon.Server;
using Photon.SocketServer;
using SubServerCommon;
using SubServerCommon.Data.NHibernate;

namespace LoginServer.Handlers
{
    public class LoginServerCreateCharacterHandler : PhotonServerHandler
    {
        public LoginServerCreateCharacterHandler(PhotonApplication application) : base(application)
        {
        }

        public override MessageType Type
        {
            get { return MessageType.Request; }
        }

        public override byte Code
        {
            get { return (byte) ClientOperationCode.Login; }
        }

        public override int? SubCode
        {
            get { return (int) MessageSubCode.CreateCharacter; }
        }

        protected override bool OnHandleMessage(IMessage message, PhotonServerPeer serverPeer)
        {
            Log.DebugFormat("on handle in login server create character handler hit");
            var para = new Dictionary<byte, object>
            {
                {(byte) ClientParameterCode.PeerId, message.Parameters[(byte) ClientParameterCode.PeerId]},
                {
                    (byte) ClientParameterCode.SubOperationCode,
                    message.Parameters[(byte) ClientParameterCode.SubOperationCode]
                }
            };
            var operation = new CreateCharacter(serverPeer.Protocol, message);
            if (!operation.IsValid)
            {
                Log.DebugFormat("operation invalid");
                serverPeer.SendOperationResponse(
                    new OperationResponse(message.Code)
                    {
                        ReturnCode = (int) ErrorCode.OperationInvalid,
                        DebugMessage = operation.GetErrorMessage(),
                        Parameters = para
                    }, new SendParameters());
                return true;
            }

            try
            {
                using (var session = NHibernateHelper.OpenSession())
                {
                    using (var transaction = session.BeginTransaction())
                    {
                        var user =
                            session.QueryOver<User>().Where(u => u.Id == operation.UserId).List().FirstOrDefault();
                        var profile =
                            session.QueryOver<UserProfile>().Where(up => up.UserId == user).List().FirstOrDefault();
                        var characters = session.QueryOver<ComplexCharacter>().Where(cc => cc.UserId == user).List();
                        if (profile != null && profile.CharacterSlots <= characters.Count)
                        {
                            Log.DebugFormat("profile invalid or no slots");
                            serverPeer.SendOperationResponse(
                                new OperationResponse(message.Code)
                                {
                                    ReturnCode = (int) ErrorCode.InvalidCharacter,
                                    DebugMessage = "No free character slots",
                                    Parameters = para
                                }, new SendParameters());
                        }
                        else
                        {
                            var mySerializer = new XmlSerializer(typeof (CharacterCreateDetails));
                            var reader = new StringReader(operation.CharacterCreateDetails);
                            var createCharacter = (CharacterCreateDetails) mySerializer.Deserialize(reader);
                            var character =
                                session.QueryOver<ComplexCharacter>()
                                    .Where(cc => cc.Name == createCharacter.CharacterName).List().FirstOrDefault();
                            if (character != null)
                            {
                                Log.DebugFormat("null character");
                                transaction.Commit();
                                serverPeer.SendOperationResponse(
                                    new OperationResponse(message.Code)
                                    {
                                        ReturnCode = (int) ErrorCode.InvalidCharacter,
                                        DebugMessage = "Character name taken",
                                        Parameters = para
                                    }, new SendParameters());
                            }
                            else
                            {
                                Log.DebugFormat("creating character");
                                var newChar = new ComplexCharacter
                                {
                                    UserId = user,
                                    Name = createCharacter.CharacterName,
                                    Class = createCharacter.CharacterClass,
                                    Sex = createCharacter.Sex,
                                    Level = 1
                                };

                                session.Save(newChar);
                                transaction.Commit();
                                serverPeer.SendOperationResponse(
                                    new OperationResponse(message.Code)
                                    {
                                        ReturnCode = (int) ErrorCode.Ok,
                                        Parameters = para
                                    }, new SendParameters());
                            }
                        }
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                Log.DebugFormat("invalid character");
                Log.Error(e);
                serverPeer.SendOperationResponse(
                    new OperationResponse(message.Code)
                    {
                        ReturnCode = (int) ErrorCode.InvalidCharacter,
                        DebugMessage = e.ToString(),
                        Parameters = para
                    }, new SendParameters());
            }
            return true;
        }
    }
}
