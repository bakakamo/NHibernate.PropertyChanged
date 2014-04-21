namespace NHibernate.PropertyChanged
{
    using System;
    using NHibernate;

    public class AddPropertyChangedInterceptor : EmptyInterceptor
    {
        private ISession _session;

        public override void SetSession(ISession session)
        {
            _session = session;
            base.SetSession(session);
        }

        public override Object Instantiate(String clazz, EntityMode entityMode, Object id)
        {
            if (entityMode != EntityMode.Poco)
                return base.Instantiate(clazz, entityMode, id);

            var classMetadata = _session.SessionFactory.GetClassMetadata(clazz);
            var entityType = classMetadata.GetMappedClass(entityMode);
            var entity = classMetadata.Instantiate(id, entityMode);
            return AddPropertyChangedInterceptorProxyFactory.Create(entityType, entity);
        }
    }
}
