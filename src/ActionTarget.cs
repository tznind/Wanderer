using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Wanderer.Actions
{
    public class ActionTarget
    {
        public ReadOnlyCollection<IHasStats> Eligible {get;}

        public IHasStats Confirmed {get;set;}

        /// <summary>
        /// How hostile picking this target will be
        /// </summary>    
        public double Attitude {get;}

        public string Description {get;}

        public ActionTarget(IEnumerable<IHasStats> eligibleTargets, string description,double attitude)
        {
            Description = description;
            Attitude = attitude;
            Eligible = new ReadOnlyCollection<IHasStats>(eligibleTargets.ToList());
        }
    }
}