﻿using ComplexServerCommon;
using MMO.Framework;
using MMO.Photon.Application;
using MMO.Photon.Server;

namespace SubServerCommon.Handlers
{
    public class ErrorEventForwardHandler : DefaultEventHandler
    {
        public ErrorEventForwardHandler(PhotonApplication application) : base(application)
        {
        }

        public override MessageType Type
        {
            get { return MessageType.Async; }
        }

        public override byte Code
        {
            get { return (byte) (ClientOperationCode.Chat | ClientOperationCode.Region | ClientOperationCode.Login); }
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
