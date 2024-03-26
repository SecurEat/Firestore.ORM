using Firestore.ORM.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Firestore.ORM.Logging.Incidents
{
    [Serializable]
    public class MissingFieldIncident : Incident
    {
        public override string Message => $"<{Document.GetType().Name}> Missing field '{MissingProperty.Name}' on {Document.Reference.Path}";

        public PropertyInfo MissingProperty
        {
            get;
            private set;
        }

        public FieldAttribute MissingPropertyAttribute
        {
            get;
            private set;
        }

        public FirestoreDocument Document
        {
            get;
            private set;
        }
        public MissingFieldIncident(FirestoreDocument document, PropertyInfo missingProperty,FieldAttribute missingPropertyAttribute)
        {
            this.MissingProperty = missingProperty;
            this.Document = document;
            this.MissingPropertyAttribute = missingPropertyAttribute;
        }
    }
}
