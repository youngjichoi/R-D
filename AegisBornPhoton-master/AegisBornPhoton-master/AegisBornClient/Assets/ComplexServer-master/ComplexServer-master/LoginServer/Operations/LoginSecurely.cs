using ComplexServerCommon;
using MMO.Framework;
using Photon.SocketServer;
using Photon.SocketServer.Rpc;

namespace LoginServer.Operations
{
    class LoginSecurely : Operation
    {

        public LoginSecurely(IRpcProtocol rpcProtocol, IMessage message)
            : base(rpcProtocol, new OperationRequest(message.Code, message.Parameters))
        {

        }


        [DataMember(Code = (byte)ClientParameterCode.UserName, IsOptional = false)]
        public string UserName { get; set; }

        [DataMember(Code = (byte)ClientParameterCode.Password, IsOptional = false)]
        public string Password { get; set; }

    }
}
