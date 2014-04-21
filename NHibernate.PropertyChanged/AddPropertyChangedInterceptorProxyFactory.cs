namespace NHibernate.PropertyChanged
{
    using System;
    using System.ComponentModel;
    using NHibernate.Proxy.DynamicProxy;

    public class AddPropertyChangedInterceptorProxyFactory
    {
        private static readonly ProxyFactory _factory = new ProxyFactory();

        public static TEntityType Create<TEntityType>()
            where TEntityType : new()
        {
            return (TEntityType)Create(typeof(TEntityType), new TEntityType());
        }

        public static object Create(Type entityType, object entity)
        {
            return _factory.CreateProxy(
                entityType,
                new NotifyPropertyChangedDynamicProxyInterceptor(entity),
                typeof(INotifyPropertyChanged));
        }

        private class NotifyPropertyChangedDynamicProxyInterceptor : NHibernate.Proxy.DynamicProxy.IInterceptor
        {
            private readonly object _proxy;
            private PropertyChangedEventHandler _changed = delegate { };

            public NotifyPropertyChangedDynamicProxyInterceptor(object proxy)
            {
                _proxy = proxy;
            }

            #region IInterceptor Members

            public object Intercept(InvocationInfo info)
            {
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
                    returnValue = info.TargetMethod.Invoke(_proxy, info.Arguments);
                }

                if (info.TargetMethod.Name.StartsWith("set_"))
                {
                    var propertyName = info.TargetMethod.Name.Substring("set_".Length);
                    _changed(info.Target, new PropertyChangedEventArgs(propertyName));
                }

                return returnValue;
            }

            #endregion
        }
    }
}
