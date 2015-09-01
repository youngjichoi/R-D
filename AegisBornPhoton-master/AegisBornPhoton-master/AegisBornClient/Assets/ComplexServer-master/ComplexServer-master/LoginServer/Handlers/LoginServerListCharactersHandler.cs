using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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
    public class LoginServerListCharactersHandler : PhotonServerHandler
    {
        public LoginServerListCharactersHandler(PhotonApplication application) : base(application)
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
            get { return (byte) MessageSubCode.ListCharacters; }
        }

        protected override bool OnHandleMessage(IMessage message, PhotonServerPeer serverPeer)
        {
            var operation = new ListCharacters(serverPeer.Protocol, message);
            if (!operation.IsValid)
            {
                Log.DebugFormat("operation is invalid");
                Log.DebugFormat("Invalid Operation - {0}", operation.GetErrorMessage());
                serverPeer.SendOperationResponse(
                    new OperationResponse(message.Code)
                    {
                        ReturnCode = (int) ErrorCode.OperationInvalid,
                        DebugMessage = operation.GetErrorMessage()
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
                        if (user != null)
                        {
                            var profile =
                                session.QueryOver<UserProfile>().Where(up => up.UserId == user).List().FirstOrDefault();
                            if (profile != null)
                            {
                                var para = new Dictionary<byte, object>
                                {
                                    {(byte) ClientParameterCode.CharacterSlots, profile.CharacterSlots},
                                    {
                                        (byte) ClientParameterCode.PeerId,
                                        message.Parameters[(byte) ClientParameterCode.PeerId]
                                    },
                                    {
                                        (byte) ClientParameterCode.SubOperationCode,
                                        message.Parameters[(byte) ClientParameterCode.SubOperationCode]
                                    }
                                };
                                Log.DebugFormat("user id from operation is {0}", operation.UserId);
                                var characters =
                                    session.QueryOver<ComplexCharacter>().Where(cc => cc.UserId == user).List();
                                XmlSerializer characterSerializer = new XmlSerializer(typeof(CharacterListItem));

                                Hashtable characterList = new Hashtable();
                                Log.DebugFormat("characterlist count = {0}", characterList.Count);
                                foreach (var complexCharacter in characters)
                                {
                                    StringWriter outStream = new StringWriter();
                                    characterSerializer.Serialize(outStream, complexCharacter.BuilderCharacterListItem());
                                    characterList.Add(complexCharacter.Id, outStream.ToString());
                                }

                                para.Add((byte)ClientParameterCode.CharacterList, characterList);

                                transaction.Commit();

                                Log.DebugFormat("sending op response");
                                serverPeer.SendOperationResponse(
                                    new OperationResponse(message.Code, para) {ReturnCode = (byte) ErrorCode.Ok},
                                    new SendParameters());

//                                serverPeer.SendOperationResponse(
                                //                                  new OperationResponse((byte) ClientOperationCode.Login) {Parameters = para},
                                //                                new SendParameters());
                            }
                            else
                            {
                                serverPeer.SendOperationResponse(
                                    new OperationResponse(message.Code)
                                    {
                                        ReturnCode = (int) ErrorCode.OperationInvalid,
                                        DebugMessage = "Profile not found"
                                    }, new SendParameters());
                            }
                        }
                        else
                        {
                            serverPeer.SendOperationResponse(
                                new OperationResponse(message.Code)
                                {
                                    ReturnCode = (int) ErrorCode.OperationInvalid,
                                    DebugMessage = "User not found"
                                }, new SendParameters());
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                serverPeer.SendOperationResponse(
                    new OperationResponse(message.Code)
                    {
                        ReturnCode = (int) ErrorCode.OperationInvalid,
                        DebugMessage = e.ToString()
                    }, new SendParameters());
            }
            return true;
        }
    }
}
