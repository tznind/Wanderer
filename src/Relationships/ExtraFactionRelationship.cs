﻿using Wanderer.Actors;

namespace Wanderer.Relationships
{
    /// <summary>
    /// Models a relationship between all members of one faction against
    /// anyone not a member of their faction.
    /// </summary>
    public class ExtraFactionRelationship : FactionRelationship
    {
        public ExtraFactionRelationship(IFaction hostFaction, double attitude) : base(hostFaction, attitude)
        {
        }

        public override bool AppliesTo(IActor observer, IActor observed)
        {
            //if you are in the faction and they are not
            return observer.FactionMembership.Contains(HostFaction) && !observed.FactionMembership.Contains(HostFaction);
        }

        public override bool AppliesTo(IFaction other)
        {
            return other != HostFaction;
        }
    }
}