using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Firestore.ORM
{
    public enum MappingBehavior
    {
        /// <summary>
        /// Throw if the fetched data is corrupted
        /// </summary>
        Strict,
        /// <summary>
        /// Ignore corrupted elements, notifying on IncidentManager.OnIncident
        /// </summary>
        Souple,
    }
}
