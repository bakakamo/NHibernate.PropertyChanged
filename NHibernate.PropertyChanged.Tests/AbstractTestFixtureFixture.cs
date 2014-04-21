using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHibernate.PropertyChanged.Tests
{
    using FluentNHibernate.Automapping;
    using FluentNHibernate.Cfg;
    using FluentNHibernate.Cfg.Db;
    using NHibernate.Cfg;
    using NHibernate.PropertyChanged.Tests.WithPropertyChanged.Domain;
    using NHibernate.Tool.hbm2ddl;
    using NUnit.Framework;

    public abstract class AbstractTestFixtureFixture
    {
        private ISessionFactory _sessionFactory;
        private Configuration _configuration;

        [SetUp]
        public void SetUp()
        {
            _sessionFactory = CreateSessionFactory();
            Session = OpenSession();
        }

        [TearDown]
        public void TearDown()
        {
            if (Session != null)
                Session.Dispose();
            if (_sessionFactory != null)
                _sessionFactory.Dispose();
        }

        protected ISession Session { get; private set; }

        protected virtual FluentConfiguration CreateFluentConfiguration()
        {
            return
                Fluently.Configure()
                    .Database(SQLiteConfiguration.Standard.InMemory().ShowSql())
                    .Mappings(
                        m =>
                            m.AutoMappings.Add(
                                AutoMap.AssemblyOf<Person>()
                                    .Where(t => t.Namespace.Contains(GetType().Namespace + ".Domain"))));
        }

        private ISessionFactory CreateSessionFactory()
        {
            _configuration = CreateFluentConfiguration().BuildConfiguration();
            return _configuration.BuildSessionFactory();
        }

        public ISession OpenSession()
        {
            ISession session = _sessionFactory.OpenSession();

            var export = new SchemaExport(_configuration);
            export.Execute(true, true, false, session.Connection, null);

            return session;
        }
    }
}
