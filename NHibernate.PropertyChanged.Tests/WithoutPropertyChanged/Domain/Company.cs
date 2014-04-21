namespace NHibernate.PropertyChanged.Tests.WithoutPropertyChanged.Domain
{
    using System.ComponentModel;

    public class Company
    {
        public virtual int Id { get; set; }

        public virtual string Name { get; set; }
    }
}
