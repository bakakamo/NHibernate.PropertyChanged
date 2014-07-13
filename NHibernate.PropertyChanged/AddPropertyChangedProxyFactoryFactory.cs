namespace NHibernate.PropertyChanged
{
    using NHibernate.Bytecode;
    using NHibernate.Proxy;

    public class AddPropertyChangedProxyFactoryFactory : IProxyFactoryFactory
    {
        #region IProxyFactoryFactory Members

        public virtual IProxyFactory BuildProxyFactory()
        {
            return new PropertyChangedProxyFactory(false);
        }

        public virtual IProxyValidator ProxyValidator
        {
            get { return new DynProxyTypeValidator(); }
        }

        public virtual bool IsInstrumented(System.Type entityClass)
        {
            return true;
        }

        public virtual bool IsProxy(object entity)
        {
            return entity is INHibernateProxy;
        }

        #endregion
    }
}
