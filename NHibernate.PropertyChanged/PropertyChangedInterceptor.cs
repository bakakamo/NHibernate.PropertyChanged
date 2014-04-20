namespace NHibernate.PropertyChanged
{
    using System;
    using System.ComponentModel;
    using NHibernate;
    using NHibernate.Proxy.DynamicProxy;

    public class PropertyChangedInterceptor : EmptyInterceptor
    {
        private readonly bool _entitiesCallPropertyChanged;
        private static readonly ProxyFactory _factory = new ProxyFactory();
        private ISession _session;

        public PropertyChangedInterceptor(bool entitiesCallPropertyChanged)
        {
            _entitiesCallPropertyChanged = entitiesCallPropertyChanged;
        }

        public override void SetSession(ISession session)
        {
            _session = session;
            base.SetSession(session);
        }

        public override Object Instantiate(String clazz, EntityMode entityMode, Object id)
        {
            var entityType = Type.GetType(clazz);
            var proxy =
                (IProxy)
                    _factory.CreateProxy(
                        entityType,
                        new NotifyPropertyChangedDynamicProxyInterceptor(_entitiesCallPropertyChanged),
                        typeof(INotifyPropertyChanged));

            var interceptor = (NotifyPropertyChangedDynamicProxyInterceptor)proxy.Interceptor;
            interceptor.Proxy = _session.SessionFactory.GetClassMetadata(entityType).Instantiate(id, entityMode);

            _session.SessionFactory.GetClassMetadata(entityType).SetIdentifier(proxy, id, entityMode);

            return (proxy);
        }

        private class NotifyPropertyChangedDynamicProxyInterceptor : NHibernate.Proxy.DynamicProxy.IInterceptor
        {
            private readonly bool _entityCallsPropertyChanged;
            private PropertyChangedEventHandler _changed = delegate { };
            private object _target;
            private object _proxy;

            public NotifyPropertyChangedDynamicProxyInterceptor(bool entityCallsPropertyChanged)
            {
                _entityCallsPropertyChanged = entityCallsPropertyChanged;
            }

            public object Proxy
            {
                get { return _proxy; }
                set
                {
                    _proxy = value;
                    if (_proxy != null)
                    {
                        ((INotifyPropertyChanged)_proxy).PropertyChanged += OnEntityPropertyChanged;
                    }
                }
            }

            #region IInterceptor Members

            public object Intercept(InvocationInfo info)
            {
                _target = info.Target;

                object returnValue = null;

                if (info.TargetMethod.Name == "add_PropertyChanged")
                {
                    var propertyChangedEventHandler =
                        info.Arguments[0] as PropertyChangedEventHandler;
                    _changed += propertyChangedEventHandler;
                }
                else if (info.TargetMethod.Name == "remove_PropertyChanged")
                {
                    var propertyChangedEventHandler = info.Arguments[0] as PropertyChangedEventHandler;
                    _changed -= propertyChangedEventHandler;
                }
                else
                {
                    returnValue = info.TargetMethod.Invoke(Proxy, info.Arguments);
                }

                if (!_entityCallsPropertyChanged && info.TargetMethod.Name.StartsWith("set_"))
                {
                    var propertyName = info.TargetMethod.Name.Substring("set_".Length);
                    _changed(info.Target, new PropertyChangedEventArgs(propertyName));
                }

                return returnValue;
            }

            private void OnEntityPropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                _changed(_target, e);
            }

            #endregion
        }
    }
}
