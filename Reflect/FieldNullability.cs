using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firestore.ORM.Reflect
{
    public enum FieldNullability
    {
        /// <summary>
        /// The field can be either null or not exist
        /// </summary>
        NonNullable,
        /// <summary>
        /// The field must be present and have a value
        /// </summary>
        Nullable,
    }
}
