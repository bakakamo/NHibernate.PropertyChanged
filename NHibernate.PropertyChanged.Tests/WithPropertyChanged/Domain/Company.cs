namespace NHibernate.PropertyChanged.Tests.WithPropertyChanged.Domain
{
    using System;
    using System.ComponentModel;

    public class Company : INotifyPropertyChanged
    {
        private string _name;
        public virtual int Id { get; set; }

        public virtual string Name
        {
            get { return _name; }
            set
            {
                if (Equals(_name, value))
                    return;

                _name = value;
                OnPropertyChanged("Name");
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
