using Firestore.ORM.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Firestore.ORM.Reflect
{
    public class CollectionDefinition
    {
        public Type Type
        {
            get;
            set;
        }
        public Dictionary<FieldAttribute, PropertyInfo> Properties
        {
            get;
            set;
        } = new Dictionary<FieldAttribute, PropertyInfo>();

        public dynamic Converter
        {
            get;
            private set;
        }

        public CollectionDefinition(Type type, Dictionary<FieldAttribute, PropertyInfo> properties)
        {
            Type = type;
            Properties = properties;

            var method = this.GetType().GetMethod("CreateConverter");
            var converter = method.MakeGenericMethod(new Type[] { type }).Invoke(this, new object[0]); ;
            Converter = converter;
        }
        public Func<T, Dictionary<string, object>> CreateConverter<T>() where T : FirestoreDocument
        {
            var type = typeof(T);
            var parameter = Expression.Parameter(type, "input");

            var dictionaryType = typeof(Dictionary<string, object>);
            var addMethod = dictionaryType.GetMethod("Add");

            var expressions = new List<Expression>();
            var dictionaryVariable = Expression.Variable(dictionaryType);
            expressions.Add(Expression.Assign(dictionaryVariable, Expression.New(dictionaryType)));


            foreach (var property in this.Properties)
            {
                var getPropertyValue = Expression.Property(parameter, property.Value);
                var boxedPropertyValue = Expression.Convert(getPropertyValue, typeof(object));
                var addKeyValue = Expression.Call(dictionaryVariable, addMethod, Expression.Constant(property.Key.Name), boxedPropertyValue);
                expressions.Add(addKeyValue);
            }

            expressions.Add(dictionaryVariable);

            var body = Expression.Block(new[] { dictionaryVariable }, expressions);

            var lambda = Expression.Lambda<Func<T, Dictionary<string, object>>>(body, parameter);
            return lambda.Compile();
        }

    }
}
