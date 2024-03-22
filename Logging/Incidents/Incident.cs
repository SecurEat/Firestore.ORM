using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firestore.ORM.Logging.Incidents
{
    public abstract class Incident
    {
        public abstract string Message
        {
            get;
        }

    }
}
