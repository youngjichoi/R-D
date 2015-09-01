using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using ComplexServerCommon;
using MMO.Photon.Application;
using MMO.Photon.Server;
using SubServerCommon;
using ComplexServer.Handlers;
using LogManager = ExitGames.Logging.LogManager;
using ExitGames.Logging;
using MMO.Photon.Client;
using SubServerCommon.ClientData;
using SubServerCommon.Handlers;

//Go between between client and servers running in background

namespace ComplexServer
{


    public class ComplexProxyServer : PhotonApplication
    {

        protected static readonly ILogger Log = LogManager.GetCurrentClassLogger();

        public override byte SubCodeParameterCode
        {
            get { return (byte) ClientParameterCode.SubOperationCode; }
        }

        public override IPEndPoint MasterEndPoint
        {
            get { throw new NotImplementedException(); }
        }

        public override int? TcpPort
        {
            get { throw new NotImplementedException(); }
        }



        public override int? UdpPort
        {
            get { throw new NotImplementedException(); }
        }

        public override IPAddress PublicIpAddress
        {
            get { throw new NotImplementedException(); }
        }

        public override int ServerType
        {
            get { throw new NotImplementedException(); }
        }

        protected override int ConnectRetryIntervalSeconds
        {
            get { throw new NotImplementedException(); }
        }

        protected override bool ConnectsToMaster
        {
            get { return false; }
        }

        public override void Register(PhotonServerPeer peer)
        {
            Log.DebugFormat("blank shit called");
        }

        protected override void ResolveParameters(IContainer container)
        {

        }

        protected override void RegisterContainerObjects(ContainerBuilder builder)
        {
            builder.RegisterType<ComplexConnectionCollection>().As<PhotonConnectionCollection>().SingleInstance();
            builder.RegisterInstance(this).As<PhotonApplication>().SingleInstance();
            builder.RegisterType<CharacterData>().As<MMO.Framework.ClientData>();
            builder.RegisterType<EventForwardHandler>().As<DefaultEventHandler>().SingleInstance();
            builder.RegisterType<ResponseForwardHandler>().As<DefaultResponseHandler>().SingleInstance();
            builder.RegisterType<RequestForwardHandler>().As<DefaultRequestHandler>().SingleInstance();
            builder.RegisterType<HandleServerRegistration>().As<PhotonServerHandler>().SingleInstance();


            //Add Handlers
            builder.RegisterType<HandleClientLoginRequests>().As<PhotonClientHandler>().SingleInstance();
            builder.RegisterType<LoginReponseHandler>().As<PhotonServerHandler>().SingleInstance();
            builder.RegisterType<SelectCharacterResponseHandler>().As<PhotonServerHandler>().SingleInstance();
        }
    }
}