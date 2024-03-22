using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Firestore.ORM.Logging.Incidents
{
    public class InvalidFieldTypeIncident : Incident
    {
        public override string Message => $"Invalid value '{Value}' for field '{Property.Name}' required type : '{Property.PropertyType.Name}' on document '{Document.Reference.Path}'";

        public FirestoreDocument Document
        {
            get;
            private set;
        }
        public PropertyInfo Property
        {
            get;
            private set;
        }
        public object Value
        {
            get;
            private set;
        }
        public InvalidFieldTypeIncident(FirestoreDocument document, PropertyInfo property, object value)
        {
            Document = document;
            Property = property;
            Value = value;
        }
    }
}
