using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Wanderer.Actions;
using Wanderer.Actors;
using Wanderer.Behaviours;
using Wanderer.Factories.Blueprints;
using Wanderer.Stats;
using Wanderer.Systems;

namespace Wanderer.Factories
{
    public abstract class HasStatsFactory<T1,T2> : IHasStatsFactory where T1:HasStatsBlueprint where T2 : IHasStats
    {
        public HashSet<Guid> UniquesSpawned { get; set; } = new HashSet<Guid>();
        
        public List<T1> Blueprints { get; set; } = new List<T1>();
       
        public List<BehaviourBlueprint> DefaultBehaviours { get; set; } = new List<BehaviourBlueprint>();
        public List<ActionBlueprint> DefaultActions { get; set; } = new List<ActionBlueprint>();

        /// <summary>
        /// True if the blueprint should be included in randomization choices
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public virtual bool Spawnable(HasStatsBlueprint b)
        {
            //don't return fixed location stuff as a random choice
            if (b is RoomBlueprint r && r.FixedLocation != null)
                return false;

            if (!b.Unique)
                return true;

            return !UniquesSpawned.Contains(b.Identifier ?? Guid.Empty);
        }

        /// <summary>
        /// Copies the basic properties shared by all <see cref="HasStatsBlueprint"/> onto the target (<paramref name="onto"/>)
        /// </summary>
        /// <param name="world"></param>
        /// <param name="onto"></param>
        /// <param name="blueprint"></param>
        /// <param name="defaultDialogueVerb">What do you do to initiate dialogue with this T, e.g. talk, read, look around etc</param>
        protected virtual void AddBasicProperties(IWorld world,T2 onto, T1 blueprint, string defaultDialogueVerb)
        {
            if (blueprint.Unique)
                UniquesSpawned.Add(blueprint.Identifier ?? Guid.Empty);

            AddActions(world,blueprint,onto);

            AddBehaviours(world,blueprint,onto);

            onto.Color = blueprint.Color;
            onto.Unique = blueprint.Unique;

            if (blueprint.Identifier.HasValue)
                onto.Identifier = blueprint.Identifier;
            
            if (blueprint.Stats != null)
                onto.BaseStats.Increase(blueprint.Stats.Clone());

            //make sure the stats listed on blueprints actually exist
            foreach(Stat s in onto.BaseStats.Keys)
                if(!world.AllStats.Contains(s))
                    throw new Exception($"Unrecognized stat '{s}' on blueprint '{onto}'.  Add the stat to stats.yaml or IWorld.AllStats");

            if (blueprint.Dialogue != null)
            {
                onto.Dialogue = blueprint.Dialogue;
                if (onto.Dialogue.Verb == null)
                    onto.Dialogue.Verb = defaultDialogueVerb;
            }
            
            if (blueprint.InjurySystem != null)
            {
                IInjurySystem system;
                try
                {
                    system = (IInjurySystem) world.GetSystem(blueprint.InjurySystem.Value);
                }
                catch (Exception e)
                {
                    throw new GuidNotFoundException(
                        $"Error looking up Injury System {blueprint.InjurySystem} for {blueprint}", e,
                        blueprint.InjurySystem.Value);
                }

                onto.InjurySystem = system;
            }
        }

        private void AddBehaviours(IWorld world,T1 blueprint, T2 onto)
        {
            //Clear defaults that come for free already on the class
            if(blueprint.SkipDefaultBehaviours)
                onto.BaseBehaviours = new List<IBehaviour>();

            foreach(var behaviourBlueprint in blueprint.Behaviours)
                world.BehaviourFactory.Create(world,onto,behaviourBlueprint);
            
            //Add default behaviours
            if(!blueprint.SkipDefaultBehaviours)
                foreach (var behaviourBlueprint in DefaultBehaviours)
                    if(behaviourBlueprint.SuitsFaction(blueprint.Faction))
                        world.BehaviourFactory.Create(world, onto, behaviourBlueprint);
        }

        private void AddActions(IWorld world,T1 blueprint, T2 onto)
        {
            //Clear defaults that come for free already on the class
            if(blueprint.SkipDefaultActions)
                onto.BaseActions = new List<IAction>();

            foreach(var actionBlueprint in blueprint.Actions)
                world.ActionFactory.Create(world,onto,actionBlueprint);

            //Add default actions
            if(!blueprint.SkipDefaultActions)
                foreach (var actionBlueprint in DefaultActions)
                    if(actionBlueprint.SuitsFaction(blueprint.Faction))
                        world.ActionFactory.Create(world, onto, actionBlueprint);
        }

        protected virtual T1 GetBlueprint(Guid guid)
        {
            return Blueprints.FirstOrDefault(b => b.Is(guid)) ??
                   throw new GuidNotFoundException($"Could not find {typeof(T1).Name} with Guid {guid}",guid);
        }

        protected virtual T1 GetBlueprint(string name)
        {
            //if the user passed a string that was actually a Guid
            if (Guid.TryParse(name, out Guid g))
                return GetBlueprint(g);

            return Blueprints.FirstOrDefault(b => b.Is(name)) ??
                   throw new NamedObjectNotFoundException($"Could not find {typeof(T1).Name} Named {name}", name);
        }
        
        /// <summary>
        /// Returns the first blueprint that matches the name (which can be a guid).  Includes sub elements of the blueprint e.g. <see cref="RoomBlueprint.MandatoryItems"/>
        /// </summary>
        public HasStatsBlueprint TryGetBlueprint(string name)
        {
            return Blueprints.Select(b=> b.TryGetBlueprint(name)).FirstOrDefault(b=>b!= null);
        }

        protected void HandleInheritance(HasStatsBlueprint blueprint)
        {
            if (blueprint.Ref != null)
            {
                var baseBlue = GetBlueprint(blueprint.Ref);

                if(!string.IsNullOrWhiteSpace(baseBlue.Ref))
                    throw new NotSupportedException($"Ref blueprints cannot have their own base blueprint (maximum inheritance depth is 1).  Bad base blueprint was '{baseBlue}'");
                
                //copy properties from the base blueprint
                foreach (var prop in typeof(T1).GetProperties())
                {
                    var currentVal = prop.GetValue(blueprint);
                    var baseVal = prop.GetValue(baseBlue);

                    //where the current blueprint doesn't have a value for it yet (is null/empty)
                    if (currentVal == null)
                        prop.SetValue(blueprint, baseVal);
                    else
                    if(currentVal is IList l && l.Count == 0 && baseVal != null)
                        prop.SetValue(blueprint, baseVal);
                }

                //now clear the Ref so we don't do it again later!
                blueprint.Ref = null;
            }
        }
    }
}