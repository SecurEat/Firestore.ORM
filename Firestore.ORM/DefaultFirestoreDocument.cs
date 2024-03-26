using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firestore.ORM
{
    public class DefaultFirestoreDocument : FirestoreDocument
    {
        public DefaultFirestoreDocument(DocumentReference reference) : base(reference)
        {
        }

        public Dictionary<string, object> Data
        {
            get;
            set;
        }

    }
}
