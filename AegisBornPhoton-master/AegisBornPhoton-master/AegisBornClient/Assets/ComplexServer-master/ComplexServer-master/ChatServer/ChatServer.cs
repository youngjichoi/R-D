using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;
using Autofac;
using ComplexServerCommon;
using MMO.Framework;
using MMO.Photon.Application;
using MMO.Photon.Server;
using MMO.Photon.Application;
using MMO.PhotonFramework.Client;
using Photon.SocketServer;
using SubServerCommon;
using SubServerCommon.ClientData;
using SubServerCommon.Data;
using SubServerCommon.Handlers;
using SubServerCommon.Operations;

namespace ChatServer
{
    public class ChatServer : PhotonApplication
    {
        private readonly IPAddress _publicIPAddress = IPAddress.Parse("127.0.0.1");
        private readonly IPEndPoint _masterEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 4520);

        public override byte SubCodeParameterCode
        {
            get { return (byte)ClientParameterCode.SubOperationCode; }
        }

        public override IPEndPoint MasterEndPoint
        {
            get { return _masterEndPoint; }
        }

        public override int? TcpPort
        {
            get { return 4532; }
        }

        public override int? UdpPort
        {
            get { return 5057; }
        }

        public override IPAddress PublicIpAddress
        {
            get { return _publicIPAddress; }
        }

        public override int ServerType
        {
            get { return (int)SubServerCommon.ServerType.Chat; }
        }

        protected override int ConnectRetryIntervalSeconds
        {
            get { return 14; }
        }

        protected override bool ConnectsToMaster
        {
            get { return true; }
        }



        public override void Register(PhotonServerPeer peer)
        {
            var registerSubServerOperation = new RegisterSubServerData()
            {
                GameServerAddress = PublicIpAddress.ToString(),
                TcpPort = TcpPort,
                UdpPort = UdpPort,
                ServerId = ServerId,
                ServerType = ServerType,
                ApplicationName = ApplicationName

            };
            XmlSerializer mySerializer = new XmlSerializer(typeof(RegisterSubServerData));
            StringWriter outString = new StringWriter();
            mySerializer.Serialize(outString, registerSubServerOperation);

            peer.SendOperationRequest(
                new OperationRequest((byte)ServerOperationCode.RegisterSubServer,
                    new RegisterSubServer() { RegisterSubServerOperation = outString.ToString() }), new SendParameters());

        }

        protected override void ResolveParameters(IContainer container)
        {
        }

        protected override void RegisterContainerObjects(ContainerBuilder builder)
        {



            builder.RegisterType<ErrorEventForwardHandler>().As<DefaultEventHandler>().SingleInstance();
            builder.RegisterType<ErrorRequestForwardHandler>().As<DefaultRequestHandler>().SingleInstance();
            builder.RegisterType<ErrorResponseForwardHandler>().As<DefaultResponseHandler>().SingleInstance();
            //builder.RegisterType<HandleServerRegistration>().As<PhotonServerHandler>().SingleInstance();

            builder.RegisterType<SubServerConnectionCollection>().As<PhotonConnectionCollection>().SingleInstance();
            builder.RegisterInstance(this).As<PhotonApplication>().SingleInstance();
            builder.RegisterType<SubServerClientPeer>();
            builder.RegisterType<CharacterData>().As<ClientData>();
            builder.RegisterType<ChatPlayer>().As<ClientData>();

                builder.RegisterAssemblyTypes(Assembly.GetAssembly(GetType())).Where(t => t.Name.EndsWith("Handler")).As<PhotonServerHandler>().SingleInstance();
            // builder.RegisterType<ComplexServerLoginRequestHandler>().As<PhotonServerHandler>().SingleInstance();
            //  builder.RegisterType<ComplexServerRegisterRequestHandler>().As<PhotonServerHandler>().SingleInstance();
            // builder.RegisterType<LoginServerUserLoggedOutHandler>().As<PhotonServerHandler>().SingleInstance();

            //builder.RegisterType<ComplexServerRegisterRequestHandler>().As<PhotonServerHandler>().SingleInstance();


        }
    }
}