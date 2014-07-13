namespace NHibernate.PropertyChanged
{
    using System;
    using System.ComponentModel;
    using System.Reflection;
    using NHibernate.Engine;
    using NHibernate.Proxy;
    using NHibernate.Proxy.DynamicProxy;
    using NHibernate.Type;

    [Serializable]
    public class PropertyChangedLazyInitializer : DefaultLazyInitializer, IInterceptor
    {
        private readonly bool _entityHandlesPropertyChanged;
        private PropertyChangedEventHandler _changed = delegate { };
        private object _proxy;

        public PropertyChangedLazyInitializer(
            string entityName,
            Type persistentClass,
            object id,
            MethodInfo getIdentifierMethod,
            MethodInfo setIdentifierMethod,
            IAbstractComponentType componentIdType,
            ISessionImplementor session,
            bool entityHandlesPropertyChanged)
            : base(entityName, persistentClass, id, getIdentifierMethod, setIdentifierMethod, componentIdType, session)
        {
            _entityHandlesPropertyChanged = entityHandlesPropertyChanged;
        }

        #region Implementation of IInterceptor

        public override void Initialize()
        {
            var wasUninitialized = Target == null;
            base.Initialize();
            if (_entityHandlesPropertyChanged && wasUninitialized && Target != null)
            {
                ((INotifyPropertyChanged)Target).PropertyChanged += OnEntityPropertyChanged;
            }
        }

        public new virtual object Intercept(InvocationInfo info)
        {
            if (_proxy == null)
            {
                _proxy = info.Target;
            }

            object returnValue = null;

            if (info.TargetMethod.Name == "add_PropertyChanged")
            {
                var propertyChangedEventHandler = info.Arguments[0] as PropertyChangedEventHandler;
                _changed += propertyChangedEventHandler;
            }
            else if (info.TargetMethod.Name == "remove_PropertyChanged")
            {
                var propertyChangedEventHandler = info.Arguments[0] as PropertyChangedEventHandler;
                _changed -= propertyChangedEventHandler;
            }
            else
            {
                returnValue = base.Intercept(info);
            }

            if (!_entityHandlesPropertyChanged && info.TargetMethod.Name.StartsWith("set_"))
            {
                var propertyName = info.TargetMethod.Name.Substring("set_".Length);
                _changed(info.Target, new PropertyChangedEventArgs(propertyName));
            }

            return returnValue;
        }

        private void OnEntityPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            _changed(_proxy, e);
        }

        #endregion
    }
}