using Firestore.ORM.Reflect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firestore.ORM.Attributes
{
    public class FieldAttribute : Attribute
    {
        public string Name
        {
            get;
            private set;
        }

        public FieldNullability Nullability
        {
            get;
            private set;
        }
        public FieldAttribute(string name, FieldNullability nullability = FieldNullability.NonNullable)
        {
            this.Name = name;
            this.Nullability = nullability;
        }
    }
}
