namespace NHibernate.PropertyChanged
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using NHibernate.Proxy;

    public class PropertyChangedDynProxyTypeValidator : DynProxyTypeValidator, IProxyValidator
    {
        public new ICollection<string> ValidateType(System.Type type)
        {
            var errors = base.ValidateType(type) ?? new List<string>();
            CheckImplementsINotifyPropertyChanged(errors, type);
            return errors.Count > 0 ? errors : null;
        }

        protected virtual void CheckImplementsINotifyPropertyChanged(ICollection<string> errors, System.Type type)
        {
            if (!typeof(INotifyPropertyChanged).IsAssignableFrom(type))
            {
                errors.Add(string.Format("{0}: {1}", type, "type should implement INotifyPropertyChanged"));
            }
        }
    }
}
