using Firestore.ORM.Logging.Incidents;
using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firestore.ORM
{
    public abstract class FirestoreDocument
    {
        public FirestoreDocument(DocumentReference reference)
        {
            Reference = reference;
            Incidents = new List<Incident>();
        }
        public string Id => Reference.Id;
        public string Path => Reference.Path;
        public DocumentReference Reference
        {
            get;
            set;
        }
        public List<Incident> Incidents
        {
            get;
            protected set;
        }
        public override string ToString()
        {
            return Path;
        }
    }
}
