using System.IO;
using System.Xml.Serialization;
using ComplexServerCommon;
using MMO.Framework;
using MMO.Photon.Application;
using MMO.Photon.Server;
using Photon.SocketServer;
using SubServerCommon.Data;
using SubServerCommon.Operations;

namespace SubServerCommon.Handlers
{
    public class HandleServerRegistration : PhotonServerHandler
    {
        public HandleServerRegistration(PhotonApplication application) : base(application)
        {
        }

        public override MessageType Type
        {
            get { return MessageType.Request; }
        }

        public override byte Code
        {
            get { return (byte) ServerOperationCode.RegisterSubServer; }
        }

        public override int? SubCode
        {
            get { return null; }
        }

        protected override bool OnHandleMessage(IMessage message, PhotonServerPeer serverPeer)
        {
            Log.DebugFormat("On handle message called in handleserverregistration");
            OperationResponse operationResponse;
            if (serverPeer.ServerId.HasValue)
            {
                Log.DebugFormat("REGISTRATION EXISTS");
                operationResponse = new OperationResponse(message.Code)
                {
                    ReturnCode = -1,
                    DebugMessage = "Already registered."
                };
            }
            else
            {
                var registerRequest = new RegisterSubServer(serverPeer.Protocol, message);
                if (!registerRequest.IsValid)
                {
                    Log.DebugFormat("REGISTRATION NOT VALID");
                    string msg = registerRequest.GetErrorMessage();
                    if (Log.IsDebugEnabled)
                    {
                        Log.DebugFormat("invalid register request {0}", msg);
                    }

                    operationResponse = new OperationResponse(message.Code)
                    {
                        DebugMessage = msg,
                        ReturnCode = (short) ErrorCode.OperationInvalid
                    };
                }
                else
                {
                    Log.DebugFormat("REGISTRATION VALID");

                    XmlSerializer mySerializer = new XmlSerializer(typeof(RegisterSubServerData));
                    StringReader inStream = new StringReader(registerRequest.RegisterSubServerOperation);
                    var registerData = (RegisterSubServerData)mySerializer.Deserialize(inStream);

                    if (Log.IsDebugEnabled)
                    {
                       
                        Log.DebugFormat("Received register request: Address={0}, udptport={2}, tcport={1}, Type={3}",
                            registerData.GameServerAddress, registerData.TcpPort, registerData.UdpPort, registerData.ServerType);
                    }
                    if (registerData.UdpPort.HasValue)
                    {
                        serverPeer.UdpAddress = registerData.GameServerAddress + ":" + registerData.UdpPort;
                    }
                    if (registerData.TcpPort.HasValue)
                    {
                        serverPeer.TcpAddress = registerData.GameServerAddress + ":" + registerData.UdpPort;
                    }

                    serverPeer.ServerId = registerData.ServerId;
                    serverPeer.ServerType = registerData.ServerType;
                    serverPeer.ApplicationName = registerData.ApplicationName;
                    Server.ConnectionCollection<PhotonConnectionCollection>().OnConnect(serverPeer);
                    operationResponse = new OperationResponse(message.Code);
                }
            }
            serverPeer.SendOperationResponse(operationResponse, new SendParameters());
            return true;
        }
    }
}
