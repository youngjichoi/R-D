using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using ComplexServerCommon;
using LoginServer.Operations;
using MMO.Framework;
using MMO.Photon.Application;
using MMO.Photon.Server;
using MMO.Photon.Application;
using MMO.PhotonFramework.Client;
using Photon.SocketServer;
using SubServerCommon;
using SubServerCommon.ClientData;
using SubServerCommon.Data.NHibernate;

namespace LoginServer.Handlers
{
    internal class ComplexServerLoginRequestHandler : PhotonServerHandler
    {
        private readonly SubServerClientPeer.Factory _clientFactory;

        public ComplexServerLoginRequestHandler(PhotonApplication application, SubServerClientPeer.Factory clientFactory) : base(application)
        {
            _clientFactory = clientFactory;
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
            get { return (int) MessageSubCode.Login; }
        }

        protected override bool OnHandleMessage(IMessage message, PhotonServerPeer serverPeer)
        {
            Log.DebugFormat("Login message being handled message code {0}", message.Code);
            var operation = new LoginSecurely(serverPeer.Protocol, message);
            if (!operation.IsValid)
            {
                serverPeer.SendOperationResponse(new OperationResponse(message.Code,
                    new Dictionary<byte, object>
                    {
                        {(byte) ClientParameterCode.PeerId, message.Parameters[(byte) ClientParameterCode.PeerId]}
                    })
                {
                    ReturnCode = (int) ErrorCode.OperationInvalid,
                    DebugMessage = operation.GetErrorMessage()
                }, new SendParameters());
                return true;
            }

            if (operation.UserName == "" | operation.Password == "")
            {
                serverPeer.SendOperationResponse(new OperationResponse(message.Code,
                    new Dictionary<byte, object>
                    {
                        {(byte) ClientParameterCode.PeerId, message.Parameters[(byte) ClientParameterCode.PeerId]}
                    })
                {
                    ReturnCode = (int) ErrorCode.IncorrectUserNameOrPassword,
                    DebugMessage = "User name or password is incorrect"
                }, new SendParameters());
                return true;
            }
            try
            {
                using (var session = NHibernateHelper.OpenSession())
                {
                    using (var transaction = session.BeginTransaction())
                    {
                        Log.DebugFormat("about to look for user account {0}", operation.UserName);
                        var userList = session.QueryOver<User>().Where(u => u.Username == operation.UserName).List();
                        if (userList.Count > 0)
                        {
                            Log.DebugFormat("found user {0} in database", operation.UserName);
                            var user = userList[0];
                            var hash = BitConverter.ToString(SHA1.Create().ComputeHash(
                                Encoding.UTF8.GetBytes(user.Salt + operation.Password)))
                                .Replace("-", "");
                            Log.DebugFormat("original pass {0}", hash.Trim());
                            Log.DebugFormat("login pass {0}", user.Password.Trim());

                            if (String.Equals(hash.Trim(), user.Password.Trim(), StringComparison.OrdinalIgnoreCase))
                            {
                                LoginServer server = Server as LoginServer;
                                if (server != null)
                                {
                                    bool founduser = false;
                                    foreach (var subServerClientPeer in server.ConnectionCollection<SubServerConnectionCollection>().Clients)
                                    {
                                        if (subServerClientPeer.Value.ClientData<CharacterData>().UserId == user.Id)
                                        {
                                            founduser = true;
                                        }
                                    }
                                    if (founduser)
                                    {
                                        Log.DebugFormat("user is already logged in");
                                        var para = new Dictionary<byte, object>
                                        {
                                            {(byte) ClientParameterCode.PeerId, message.Parameters[(byte) ClientParameterCode.PeerId]},
                                            {(byte) ClientParameterCode.SubOperationCode, message.Parameters[(byte) ClientParameterCode.SubOperationCode]}
                                        };
                                        serverPeer.SendOperationResponse(
                                            new OperationResponse((byte) ClientOperationCode.Login)
                                            {
                                                Parameters = para,
                                                ReturnCode = (short) ErrorCode.UserCurrentlyLoggedIn,
                                                DebugMessage = "User is currently logged in."
                                            }, new SendParameters());
                                    }
                                    else
                                    {
                                        Log.Debug("Login handler successfully found character to log in.");

                                        server.ConnectionCollection<SubServerConnectionCollection>().Clients.Add(new Guid((Byte[]) message.Parameters[(byte) ClientParameterCode.PeerId]), _clientFactory());
                                        server.ConnectionCollection<SubServerConnectionCollection>().Clients[new Guid((Byte[])message.Parameters[(byte)ClientParameterCode.PeerId])].ClientData<CharacterData>().UserId = user.Id;
                                        var para = new Dictionary<byte, object>
                                        {
                                            {
                                                (byte) ClientParameterCode.PeerId,
                                                message.Parameters[(byte) ClientParameterCode.PeerId]
                                            },
                                            {
                                                (byte) ClientParameterCode.SubOperationCode,
                                                message.Parameters[(byte) ClientParameterCode.SubOperationCode]
                                            },
                                            {(byte) ClientParameterCode.UserId, user.Id}
                                        };
                                        serverPeer.SendOperationResponse(
                                            new OperationResponse((byte) ClientOperationCode.Login) {Parameters = para},
                                            new SendParameters());
                                    }
                                }
                                return true;
                            }
                            Log.Debug("password does not match.");

                            serverPeer.SendOperationResponse(new OperationResponse(message.Code,
                                new Dictionary<byte, object>
                                {
                                    {
                                        (byte) ClientParameterCode.PeerId,
                                        message.Parameters[(byte) ClientParameterCode.PeerId]
                                    }
                                })
                            {
                                ReturnCode = (int) ErrorCode.IncorrectUserNameOrPassword,
                                DebugMessage = "User name or password is incorrect"
                            }, new SendParameters());
                            return true;
                        }
                        Log.DebugFormat("Account name does not exist {0}", operation.UserName);
                        transaction.Commit();
                        serverPeer.SendOperationResponse(
                            new OperationResponse(message.Code)
                            {
                                ReturnCode = (int) ErrorCode.IncorrectUserNameOrPassword,
                                DebugMessage = "User name or password is incorrect."
                            }, new SendParameters());
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error("Error Occured", e);
                serverPeer.SendOperationResponse(new OperationResponse(message.Code,
                    new Dictionary<byte, object>
                    {
                        {(byte) ClientParameterCode.PeerId, message.Parameters[(byte) ClientParameterCode.PeerId]}
                    })
                {
                    ReturnCode = (int) ErrorCode.IncorrectUserNameOrPassword,
                    //remove debug msg later
                    DebugMessage = e.ToString()
                }, new SendParameters());
            }
            return true;
        }
    }
}
