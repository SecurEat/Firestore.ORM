using Firestore.ORM.Logging.Incidents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firestore.ORM.Logging
{
    [Serializable]
    public class ORMException : Exception
    {
        public ORMException(Incident incident) : base(incident.Message)
        {
        }

    }
}
