using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MMO.Framework;
using Photon.SocketServer;
using Photon.SocketServer.Rpc;

namespace SubServerCommon.Operations
{
    public class RegisterSubServer : Operation
    {
        public RegisterSubServer(IRpcProtocol rpcProtocol, IMessage message) : base (rpcProtocol, new OperationRequest(message.Code, message.Parameters))
        {
            
        }

        public RegisterSubServer(IRpcProtocol rpcProtocol, OperationRequest operationRequest)
            : base(rpcProtocol, operationRequest)
        {
            
        }

        public RegisterSubServer()
        {
            
        }

        [DataMember(Code = (byte)ServerParameterCode.RegisterSubServerOperation, IsOptional = false)]
        public string RegisterSubServerOperation { get; set; }
    }
}
