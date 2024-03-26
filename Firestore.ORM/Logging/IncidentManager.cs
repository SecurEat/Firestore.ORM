using Firestore.ORM.Logging.Incidents;
using Firestore.ORM.Reflect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firestore.ORM.Logging
{
    public class IncidentManager
    {
        public static event Action<Incident> OnIncident;

        public static void DeclareIncident(Incident incident)
        {
            if (FirestoreManager.Instance.MappingBehavior == MappingBehavior.Strict)
            {
                throw new ORMException(incident);
            }

            OnIncident?.Invoke(incident);
        }
    }
}
