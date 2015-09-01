
using System.IO;
using System.Net;
using System.Reflection;
using System.Xml.Serialization;
using Autofac;
using MMO.Photon.Application;
using MMO.Photon.Server;
using Photon.SocketServer;
using SubServerCommon;
using SubServerCommon.Data;
using SubServerCommon.Handlers;
using SubServerCommon.Operations;
using ComplexServerCommon;
using LoginServer.Handlers;
using MMO.Framework;
using MMO.Photon.Application;
using MMO.PhotonFramework.Client;
using NHibernate.Criterion;
using SubServerCommon.ClientData;

namespace LoginServer
{
    public class LoginServer : PhotonApplication
    {
        private readonly IPAddress _publicIPAddress = IPAddress.Parse("127.0.0.1");
        private readonly IPEndPoint _masterEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 4520);

        public override byte SubCodeParameterCode
        {
            get { return (byte) ClientParameterCode.SubOperationCode; }
        }

        public override IPEndPoint MasterEndPoint
        {
            get { return _masterEndPoint; }
        }

        public override int? TcpPort
        {
            get { return 4531; }
        }

        public override int? UdpPort
        {
            get { return 5056; }
        }

        public override IPAddress PublicIpAddress
        {
            get { return _publicIPAddress; }
        }

        public override int ServerType
        {
            get { return (int)SubServerCommon.ServerType.Login; }
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
            Log.DebugFormat("RRRegistering login server.");
            var registerSubServerOperation = new RegisterSubServerData()
            {
                GameServerAddress = PublicIpAddress.ToString(),
                TcpPort =  TcpPort,
                UdpPort =  UdpPort,
                ServerId = ServerId,
                ServerType = ServerType,
                ApplicationName = ApplicationName

            };
            XmlSerializer mySerializer = new XmlSerializer(typeof(RegisterSubServerData));
            StringWriter outString = new StringWriter();
            mySerializer.Serialize(outString, registerSubServerOperation);
            
            peer.SendOperationRequest(
                new OperationRequest((byte) ServerOperationCode.RegisterSubServer,
                    new RegisterSubServer() { RegisterSubServerOperation = outString.ToString()}), new SendParameters());
            
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
            // Add Handlers
            
            builder.RegisterAssemblyTypes(Assembly.GetAssembly(GetType())).Where(t => t.Name.EndsWith("Handler")).As<PhotonServerHandler>().SingleInstance();
            // builder.RegisterType<ComplexServerLoginRequestHandler>().As<PhotonServerHandler>().SingleInstance();
            //  builder.RegisterType<ComplexServerRegisterRequestHandler>().As<PhotonServerHandler>().SingleInstance();
            // builder.RegisterType<LoginServerUserLoggedOutHandler>().As<PhotonServerHandler>().SingleInstance();

            //builder.RegisterType<ComplexServerRegisterRequestHandler>().As<PhotonServerHandler>().SingleInstance();
           Log.DebugFormat("xxx registered types into autofac container");

        }
    }
}
