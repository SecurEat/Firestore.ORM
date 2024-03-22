using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firestore.ORM.Extensions
{
    public static class CollectionExtensions
    {
        public static bool SequenceEqualsSafe<T>(this IEnumerable<T> t1, IEnumerable<T> t2)
        {
            if (t1 == null && t2 == null)
            {
                return true;
            }

            if (t1 == null || t2 == null)
            {
                return false;
            }

            return t1.SequenceEqual(t2);
        }
    }
}
