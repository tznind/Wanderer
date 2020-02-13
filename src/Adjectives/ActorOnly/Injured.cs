﻿using System;
using System.Collections.Generic;
using System.Linq;
using Wanderer.Actors;
using Wanderer.Adjectives.RoomOnly;
using Wanderer.Behaviours;
using Wanderer.Stats;
using Wanderer.Systems;

namespace Wanderer.Adjectives.ActorOnly
{
    public class Injured : Adjective, IBehaviour, IInjured
    {
        public IInjurySystem InjurySystem { get; set; }
        public InjuryRegion Region { get; set; }
        public double Severity { get; set; }
        public bool IsInfected { get; set; }

        /// <summary>
        /// Tracks how many rounds have gone by
        /// </summary>
        readonly HashSet<Guid> _roundsSeen = new HashSet<Guid>();

        public IActor OwnerActor { get; set; }


        public void Worsen(IUserinterface ui, Guid round)
        {
            InjurySystem.Worsen(this, ui, round);
        }

        public void Heal(IUserinterface ui, Guid round)
        {
            InjurySystem.Heal(this, ui, round);
        }

        public Injured(string name,IActor actor, double severity,InjuryRegion region,IInjurySystem system):base(actor)
        {
            Severity = severity;
            Region = region;
            IsPrefix = false;
            InjurySystem = system;

            BaseStats[Stat.Fight] = -5 * severity;
            Name = name;
            OwnerActor = actor;

            BaseBehaviours.Add(this);
        }
        
        public void OnPush(IWorld world, IUserinterface ui, ActionStack stack, Frame frame)
        {
            _roundsSeen.Add(stack.Round);
            
            //light wounds
            if (InjurySystem.ShouldNaturallyHeal(this, _roundsSeen.Count))
                Heal(ui, stack.Round);
            else
            //heavy wounds
            if (InjurySystem.ShouldWorsen(this, _roundsSeen.Count))
            {
                Worsen(ui, stack.Round); //make injury worse
                _roundsSeen.Clear(); //and start counting again from 0
            }
        }

        public void OnPop(IWorld world, IUserinterface ui, ActionStack stack, Frame frame)
        {
            
        }

        public void OnRoundEnding(IUserinterface ui, Guid round)
        {
            if(InjurySystem.HasFatalInjuries(OwnerActor,out string reason))
                OwnerActor.Kill(ui,round, reason);
        }
        
        public override IEnumerable<string> GetDescription()
        {
            yield return "Reduces Stats";
            yield return "Leads to Death";
        }

        public bool IsHealableBy(IActor actor, out string reason)
        {
            return InjurySystem.IsHealableBy(actor, this, out reason);
        }

        public bool AreIdentical(IBehaviour other)
        {
            return other is IAdjective oa && this.AreIdentical(oa);
        }
    }
}