namespace NHibernate.PropertyChanged.Tests.WithPropertyChanged
{
    using NHibernate.PropertyChanged.Tests.WithPropertyChanged.Domain;
    using NUnit.Framework;

    [TestFixture]
    public class TestWithPropertyChanged : AbstractTestFixtureFixture
    {
        [Test]
        public void Created_entity_should_handle_PropertyChanged()
        {
            var person = new Person
            {
                FirstName = "First name",
                LastName = "Last name",
                Company = new Company { Name = "Company name" }
            };

            Check_entity_handles_PropertyChanged(person);
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
        }

        protected override FluentNHibernate.Cfg.FluentConfiguration CreateFluentConfiguration()
        {
            return base.CreateFluentConfiguration().ProxyFactoryFactory<PropertyChangedProxyFactoryFactory>();
        }

        private void Check_entity_handles_PropertyChanged(Person person)
        {
            var eventWasCalled = false;
            var propertyName = string.Empty;
            object sender = null;

            person.PropertyChanged += (s, e) => { eventWasCalled = true; sender = s; propertyName = e.PropertyName; };

            person.FirstName = "New first name";

            Assert.That(eventWasCalled);
            Assert.That(propertyName, Is.EqualTo("FirstName"));
            Assert.That(sender, Is.SameAs(person));
        }

        private void Check_entity_handles_PropertyChanged(Company company)
        {
            var eventWasCalled = false;
            var propertyName = string.Empty;
            object sender = null;

            company.PropertyChanged += (s, e) => { eventWasCalled = true; sender = s; propertyName = e.PropertyName; };

            company.Name = "New name";

            Assert.That(eventWasCalled);
            Assert.That(propertyName, Is.EqualTo("Name"));
            Assert.That(sender, Is.SameAs(company));
        }
    }
}