using System;
using System.Linq.Expressions;
using System.Reflection;

namespace NRules.Config
{
    public class ComponentConfig<T>
    {
        private readonly IContainer _container;

        internal ComponentConfig(IContainer container)
        {
            _container = container;
        }

        public ComponentConfig<T> ConfigureProperty(Expression<Func<T, object>> expression, object value)
        {
            var member = expression.Body as MemberExpression;
            if (member == null)
            {
                throw new ArgumentException("Expression is not a property expression");
            }
            var propInfo = member.Member as PropertyInfo;
            if (propInfo == null)
            {
                throw new ArgumentException("Expression is not a property expression");
            }
            _container.ConfigureProperty(typeof (T), propInfo.Name, value);
            return this;
        }
    }
}