namespace NHibernate.PropertyChanged.Tests.WithPropertyChanged.Domain
{
    using System;
    using System.ComponentModel;

    public class Person : INotifyPropertyChanged
    {
        private string _lastName;
        private string _firstName;
        private Company _company;

        public virtual int Id { get; set; }

        public virtual string FirstName
        {
            get { return _firstName; }
            set
            {
                if (Equals(_firstName, value))
                    return;

                _firstName = value;
                OnPropertyChanged("FirstName");
            }
        }

        public virtual string LastName
        {
            get { return _lastName; }
            set
            {
                if (Equals(_lastName, value))
                    return;

                _lastName = value;
                OnPropertyChanged("LastName");
            }
        }

        public virtual Company Company
        {
            get { return _company; }
            set
            {
                if (Equals(_company, value))
                    return;

                _company = value;
                OnPropertyChanged("Company");
            }
        }

        public virtual event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
