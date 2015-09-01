using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ComplexServerCommon;
using MMO.Framework;
using MMO.Photon.Application;
using MMO.Photon.Server;

namespace SubServerCommon.Handlers
{
    public class ErrorResponseForwardHandler : DefaultResponseHandler
    {

        public ErrorResponseForwardHandler(PhotonApplication application) : base(application)
        {
        }

        public override MessageType Type
        {
            get { return MessageType.Response; }
        }

        public override byte Code
        {
            get { return (byte)(ClientOperationCode.Chat | ClientOperationCode.Region | ClientOperationCode.Login); }
        }

        public override int? SubCode
        {
            get { return null; }
        }

        protected override bool OnHandleMessage(IMessage message, PhotonServerPeer serverPeer)
        {
            Log.ErrorFormat("No existing event Handler. code: {0} type {1} subcode {2}", message.Code, message.GetType(), message.SubCode);

            return true;
        }
    }
}
