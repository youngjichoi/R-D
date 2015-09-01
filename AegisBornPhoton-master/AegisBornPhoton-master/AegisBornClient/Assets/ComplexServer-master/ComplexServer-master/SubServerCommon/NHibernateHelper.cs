
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;

namespace SubServerCommon
{
    public class NHibernateHelper
    {
        public NHibernateHelper()
        {
            InitializeSessionFactory();
        }

        private static ISessionFactory _sessionFactory;

        private static ISessionFactory SessionFactory
        {
            get
            {
                if (_sessionFactory == null)

                    InitializeSessionFactory();
                    return _sessionFactory;

            }
        }

        private static void InitializeSessionFactory()
        {
            _sessionFactory = Fluently.Configure().Database(
                MySQLConfiguration.Standard
                .ConnectionString(cs => cs.Server("localhost")
                .Database("cjrgaming_cs")
                .Username("cjrgaming")
                .Password("user_cs1!")))
                .Mappings(m => m.FluentMappings.AddFromAssemblyOf<NHibernateHelper>())
                .BuildSessionFactory();
                
        }

        public static ISession OpenSession()
        {
            return SessionFactory.OpenSession();
        }
    }
}
