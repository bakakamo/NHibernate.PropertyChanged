namespace NHibernate.PropertyChanged.Tests.WithoutPropertyChanged
{
    using System.ComponentModel;
    using NHibernate.PropertyChanged.Tests.WithoutPropertyChanged.Domain;
    using NUnit.Framework;

    [TestFixture]
    public class TestWithoutPropertyChanged : AbstractTestFixtureFixture
    {
        [Test]
        public void Created_entity_doesnt_handle_PropertyChanged()
        {
            var person = new Person
            {
                FirstName = "First name",
                LastName = "Last name",
                Company = new Company { Name = "Company name" }
            };

            Assert.That(person, Is.Not.InstanceOf<INotifyPropertyChanged>());
        }

        [Test]
        public void Created_proxy_entity_should_handle_PropertyChanged()
        {
            var company = AddPropertyChangedInterceptorProxyFactory.Create<Company>();
            company.Name = "Company name";

            var person = AddPropertyChangedInterceptorProxyFactory.Create<Person>();
            person.FirstName = "First name";
            person.LastName = "Last name";
            person.Company = company;

            Check_entity_handles_PropertyChanged(person);
        }

        [Test]
        public void Created_proxy_entity_should_be_persistable()
        {
            var company = AddPropertyChangedInterceptorProxyFactory.Create<Company>();
            company.Name = "Company name";

            var person = AddPropertyChangedInterceptorProxyFactory.Create<Person>();
            person.FirstName = "First name";
            person.LastName = "Last name";
            person.Company = company;

            using (var transaction = Session.BeginTransaction())
            {
                Session.SaveOrUpdate(typeof(Company).FullName, person.Company);
                Session.SaveOrUpdate(typeof(Person).FullName, person);

                transaction.Commit();
            }
        }

        [Test]
        public void Fetched_entity_should_handle_PropertyChanged()
        {
            var person = new Person
            {
                FirstName = "First name",
                LastName = "Last name",
                Company = new Company { Name = "Company name" }
            };

            using (var transaction = Session.BeginTransaction())
            {
                Session.SaveOrUpdate(person.Company);
                Session.SaveOrUpdate(person);

                transaction.Commit();
            }

            Session.Clear();

            var personFetched = Session.Get<Person>(person.Id);
            Assert.That(personFetched, Is.Not.SameAs(person));

            Check_entity_handles_PropertyChanged(personFetched);
        }

        [Test]
        public void Loaded_entity_should_handle_PropertyChanged()
        {
            var person = new Person
            {
                FirstName = "First name",
                LastName = "Last name",
                Company = new Company { Name = "Company name" }
            };

            using (var transaction = Session.BeginTransaction())
            {
                Session.SaveOrUpdate(person.Company);
                Session.SaveOrUpdate(person);

                transaction.Commit();
            }

            Session.Clear();

            var personFetched = Session.Load<Person>(person.Id);
            Assert.That(personFetched, Is.Not.SameAs(person));

            Assert.That(!NHibernateUtil.IsInitialized(personFetched));
            Check_entity_handles_PropertyChanged(personFetched);
        }

        [Test]
        public void Fetched_entitys_relation_should_handle_PropertyChanged()
        {
            var person = new Person
            {
                FirstName = "First name",
                LastName = "Last name",
                Company = new Company { Name = "Company name" }
            };

            using (var transaction = Session.BeginTransaction())
            {
                Session.SaveOrUpdate(person.Company);
                Session.SaveOrUpdate(person);

                transaction.Commit();
            }

            Session.Clear();

            var personFetched = Session.Get<Person>(person.Id);
            Assert.That(personFetched, Is.Not.SameAs(person));

            Assert.That(!NHibernateUtil.IsInitialized(personFetched.Company));
            Check_entity_handles_PropertyChanged(personFetched.Company);

            using (var transaction = Session.BeginTransaction())
            {
                Session.SaveOrUpdate(personFetched.Company);

                transaction.Commit();
            }
        }

        protected override FluentNHibernate.Cfg.FluentConfiguration CreateFluentConfiguration()
        {
            return base.CreateFluentConfiguration().ProxyFactoryFactory<AddPropertyChangedProxyFactoryFactory>().ExposeConfiguration(c => c.SetInterceptor(new AddPropertyChangedInterceptor()));
        }

        private void Check_entity_handles_PropertyChanged(Person person)
        {
            Assert.That(person, Is.InstanceOf<INotifyPropertyChanged>());

            var eventWasCalled = false;
            var propertyName = string.Empty;
            object sender = null;

            ((INotifyPropertyChanged)person).PropertyChanged += (s, e) => { eventWasCalled = true; sender = s; propertyName = e.PropertyName; };

            person.FirstName = "New first name";

            Assert.That(eventWasCalled);
            Assert.That(propertyName, Is.EqualTo("FirstName"));
            Assert.That(sender, Is.SameAs(person));
        }

        private void Check_entity_handles_PropertyChanged(Company company)
        {
            Assert.That(company, Is.InstanceOf<INotifyPropertyChanged>());

            var eventWasCalled = false;
            var propertyName = string.Empty;
            object sender = null;

            ((INotifyPropertyChanged)company).PropertyChanged += (s, e) => { eventWasCalled = true; sender = s; propertyName = e.PropertyName; };

            company.Name = "New name";

            Assert.That(eventWasCalled);
            Assert.That(propertyName, Is.EqualTo("Name"));
            Assert.That(sender, Is.SameAs(company));
        }
    }
}