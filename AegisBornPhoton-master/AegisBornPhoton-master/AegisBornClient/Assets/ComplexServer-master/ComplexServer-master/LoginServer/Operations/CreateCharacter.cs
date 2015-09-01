using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ComplexServerCommon;
using MMO.Framework;
using Photon.SocketServer;
using Photon.SocketServer.Rpc;

namespace LoginServer.Operations
{
    public class CreateCharacter : Operation
    {
        public CreateCharacter(IRpcProtocol protocol, IMessage message)
            : base(protocol, new OperationRequest(message.Code, message.Parameters))
        {
        }

        [DataMember(Code = (byte)ClientParameterCode.UserId, IsOptional = false)]
        public int UserId { get; set; }
        [DataMember(Code = (byte)ClientParameterCode.CharacterCreateDetails, IsOptional = false )]
        public string CharacterCreateDetails { get; set; }
    }
        
    }

   

