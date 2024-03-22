using Firestore.ORM.Reflect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Firestore.ORM.Extensions
{
    static class TypeExtensions
    {

        public static bool IsListType(this Type type)
        {
            return type.IsGenericType &&
                        type.GetGenericTypeDefinition() == typeof(List<>);
        }
    }
}
