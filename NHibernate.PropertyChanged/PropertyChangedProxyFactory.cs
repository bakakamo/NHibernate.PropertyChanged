namespace NHibernate.PropertyChanged
{
    using System;
    using NHibernate;
    using NHibernate.Engine;
    using NHibernate.Intercept;
    using NHibernate.Proxy;
    using NHibernate.Proxy.DynamicProxy;

    public class PropertyChangedProxyFactory : AbstractProxyFactory
    {
        private readonly bool _entitiesCallPropertyChanged;
        private readonly ProxyFactory _factory = new ProxyFactory();
        protected static readonly IInternalLogger _log = LoggerProvider.LoggerFor(typeof(PropertyChangedProxyFactory));

        public PropertyChangedProxyFactory(bool entitiesCallPropertyChanged)
        {
            _entitiesCallPropertyChanged = entitiesCallPropertyChanged;
        }

        public override INHibernateProxy GetProxy(object id, ISessionImplementor session)
        {
            try
            {
                var initializer = new PropertyChangedLazyInitializer(
                    EntityName,
                    PersistentClass,
                    id,
                    GetIdentifierMethod,
                    SetIdentifierMethod,
                    ComponentIdType,
                    session,
                    _entitiesCallPropertyChanged);

                object proxyInstance = IsClassProxy
                    ? _factory.CreateProxy(PersistentClass, initializer, Interfaces)
                    : _factory.CreateProxy(Interfaces[0], initializer, Interfaces);

                return (INHibernateProxy)proxyInstance;
            }
            catch (Exception ex)
            {
                _log.Error("Creating a proxy instance failed", ex);
                throw new HibernateException("Creating a proxy instance failed", ex);
            }
        }

        public override object GetFieldInterceptionProxy(object instanceToWrap)
        {
            var interceptor = new DefaultDynamicLazyFieldInterceptor(instanceToWrap);
            return _factory.CreateProxy(PersistentClass, interceptor, new[] { typeof(IFieldInterceptorAccessor) });
        }
    }
}