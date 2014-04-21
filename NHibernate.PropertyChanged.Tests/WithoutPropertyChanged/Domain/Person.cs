namespace NHibernate.PropertyChanged.Tests.WithoutPropertyChanged.Domain
{
    using System.ComponentModel;

    public class Person
    {
        public virtual int Id { get; set; }

        public virtual string FirstName { get; set; }

        public virtual string LastName { get; set; }

        public virtual Company Company { get; set; }
    }
}
