using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using ComplexServerCommon;
using LoginServer.Operations;
using MMO.Framework;
using MMO.Photon.Application;
using MMO.Photon.Server;
using Photon.SocketServer;
using SubServerCommon;
using SubServerCommon.Data.NHibernate;

namespace LoginServer.Handlers
{
    public class ComplexServerRegisterRequestHandler : PhotonServerHandler
    {
        public ComplexServerRegisterRequestHandler(PhotonApplication application) : base(application)
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
            get { return (int) MessageSubCode.Register; }
        }

        protected override bool OnHandleMessage(IMessage message, PhotonServerPeer serverPeer)
        {
            var operation = new RegisterSecurely(serverPeer.Protocol, message);
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

            if (operation.UserName == "" | operation.Email == "" | operation.Password == "")
            {
                serverPeer.SendOperationResponse(new OperationResponse(message.Code,
                   new Dictionary<byte, object>
                   {
                        {(byte) ClientParameterCode.PeerId, message.Parameters[(byte) ClientParameterCode.PeerId]}
                   })
                {
                    ReturnCode = (int)ErrorCode.OperationInvalid,
                    DebugMessage = "All Fields are Required"
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
                            Log.DebugFormat("Found account name already in use.");
                            transaction.Commit();
                            serverPeer.SendOperationResponse(
                                new OperationResponse(message.Code)
                                {
                                    ReturnCode = (int) ErrorCode.UserNameInUse,
                                    DebugMessage = "Account name already in use please use another."
                                }, new SendParameters());
                            return true;
                        }

                        var salt = Guid.NewGuid().ToString().Replace("-", "");
                        Log.DebugFormat("Created salt {0}", salt);
                        var newUser = new User
                        {
                            Email = operation.Email,
                            Username = operation.UserName,
                            //TODO may need to change back to SHA1
                            Password =
                                BitConverter.ToString(SHA1.Create().ComputeHash(
                                    Encoding.UTF8.GetBytes(salt + operation.Password))).Replace("-", ""),
                            Salt = salt,
                            Algorithm = "sha1",
                            Created = DateTime.Now,
                            Updated = DateTime.Now
                        };

                        Log.DebugFormat("built user opbject");
                        session.Save(newUser);
                        Log.DebugFormat("Saved new user");
                        transaction.Commit();
                    }
                    using (var transaction = session.BeginTransaction())
                    {
                        Log.DebugFormat("looking up newly creaded user");
                        var userList = session.QueryOver<User>().Where(u => u.Username == operation.UserName).List();
                        if (userList.Count > 0)
                        {
                            Log.DebugFormat("creating profile");
                            UserProfile profile = new UserProfile() { CharacterSlots = 1, UserId = userList[0]};
                            session.Save(profile);
                            Log.DebugFormat("saved profile");
                            transaction.Commit();
                        }
                    }

                    serverPeer.SendOperationResponse(
                        new OperationResponse(message.Code) {ReturnCode = (byte) ClientReturnCode.UserCreated},
                        new SendParameters());
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
                    ReturnCode = (int) ErrorCode.UserNameInUse,
                    //remove debug msg later
                    DebugMessage = e.ToString()
                }, new SendParameters());
            }
            return true;
        }
    }
}
