using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Wanderer.Actions;
using Wanderer.Adjectives;
using Wanderer.Behaviours;
using Wanderer.Factories.Blueprints;
using Wanderer.Items;
using Wanderer.Rooms;
using Wanderer.Relationships;
using Wanderer.Stats;
using Wanderer.Systems;

namespace Wanderer.Actors
{
    /// <inheritdoc cref="IActor"/>
    public abstract class Actor : HasStats,IActor
    {
        public bool Dead { get; set; }
        public string FightVerb { get; set; } = "Fists";
        
        /// <inheritdoc/>
        public IRoom CurrentLocation { get; set; }
        
        public List<IItem> Items { get; set; } = new List<IItem>();
        public HashSet<IFaction> FactionMembership { get; set; } = new HashSet<IFaction>();

        public SlotCollection AvailableSlots { get; set; } = new SlotCollection();
        
        private ConsoleColor _explicitColor = DefaultColor;


        public bool CanInitiateDialogue {get;set;}
        public bool CanInspect {get;set;}

        public override ConsoleColor Color
        {
            //use the faction color unless we have an explicit room color set
            get =>
                _explicitColor != DefaultColor ? _explicitColor : FactionMembership.FirstOrDefault()?.Color ?? _explicitColor;
            set => _explicitColor = value;
        } 

        /// <summary>
        /// Do not use, internal constructor for JSON serialization
        /// </summary>
        [JsonConstructor]
        protected Actor()
        {

        }

        /// <summary>
        /// Creates a new actor with the given <paramref name="name"/> and adds him to the <paramref name="currentLocation"/> (and world population)
        /// </summary>
        /// <param name="name"></param>
        /// <param name="currentLocation"></param>
        public Actor(string name,IRoom currentLocation)
        {
            Name = name;
            CurrentLocation = currentLocation;
            CurrentLocation.World.Population.Add(this);
            
            //basic actions everyone can do (by default)
            BaseActions.Add(new LeaveAction(this));

            //if there are ways to be injured then there are ways to fight
            if(currentLocation.World.InjurySystems.Any())
                BaseActions.Add(new FightAction(this));
            
            BaseActions.Add(new EquipmentAction(this));
            BaseActions.Add(new PickUpAction(this));
            BaseBehaviours.Add(new MergeStacksBehaviour(this));
        }


        /// <summary>
        /// Returns all <see cref="IAction"/> which the <see cref="Actor"/> can undertake in it's <see cref="CurrentLocation"/> (this includes 
        /// <see cref="HasStats.BaseActions"/> but also any location or item specific actions.
        /// </summary>
        /// <param name="forActor"></param>
        /// <returns></returns>
        public override List<IAction> GetFinalActions(IActor forActor)
        {
            return new List<IAction>(BaseActions
                .Union(Adjectives.SelectMany(a => a.GetFinalActions(forActor)))
                .Union(CurrentLocation.GetFinalActions(forActor))
                .Union(FactionMembership.SelectMany(f=>f.GetFinalActions(forActor)))
                .Union(Items.SelectMany(i => i.GetFinalActions(forActor)))
                .Union(BuildDialogueTargets(forActor))
                .Union(BuildInspectTargets(forActor)));
        }

        private IEnumerable<IAction> BuildDialogueTargets(IActor forActor)
        {
            var toReturn = new List<IAction>();

            if(!CanInitiateDialogue)
                return toReturn;

            toReturn.Add(BuildDialogueTarget(forActor.CurrentLocation));

            toReturn.AddRange(forActor.GetCurrentLocationSiblings(false).Select(BuildDialogueTarget));
        
            toReturn.AddRange(forActor.Items.Select(BuildDialogueTarget));

            return toReturn.Where(a=>a!=null);
        }
        private DialogueAction BuildDialogueTarget(IHasStats possibleTarget)
        {
            if(possibleTarget == null || possibleTarget.Dialogue.IsEmpty)
                return null;

            return new DialogueAction(possibleTarget);
        }


        private IEnumerable<IAction> BuildInspectTargets(IActor forActor)
        {
            var toReturn = new List<IAction>();


            if(!CanInspect)
                return toReturn;

            toReturn.Add(new InspectAction(forActor));
            toReturn.Add(new InspectAction(forActor.CurrentLocation));

            toReturn.AddRange(forActor.GetCurrentLocationSiblings(false).Select(o=>new InspectAction(o)));
        
            toReturn.AddRange(forActor.Items.Select(o=>new InspectAction(o)));

            return toReturn.Where(a=>a!=null);
        }

        public abstract bool Decide<T>(IUserinterface ui, string title, string body, out T chosen, T[] options,
            double attitude);

        public virtual void Move(IRoom newLocation)
        {
            CurrentLocation = newLocation;
        }

        public abstract void Kill(IUserinterface ui, Guid round, string reason);

        /// <inheritdoc/>
        public IActor[] GetCurrentLocationSiblings(bool includeDead)
        {
            return CurrentLocation.World.Population.Where(o => o.CurrentLocation == CurrentLocation && o != this && (!o.Dead || includeDead)).ToArray();
        }
        
        public bool Has(string name, bool includeItems)
        {
            if (includeItems)
                return Has(name);
            
            return Is(name) || Adjectives.Any(a => a.Is(name)) || FactionMembership.Any(f=>f.Has(name));
        }

        public override List<IBehaviour> GetFinalBehaviours(IActor forActor)
        {
            //the dead have no behaviours
            if(Dead)
                return new List<IBehaviour>();

            return new List<IBehaviour>(BaseBehaviours
                .Union(Adjectives.SelectMany(a=>a.GetFinalBehaviours(forActor)))
                .Union(CurrentLocation.GetFinalBehaviours(forActor))
                .Union(FactionMembership.SelectMany(f=>f.GetFinalBehaviours(forActor)))
                .Union(Items.SelectMany(i=>i.GetFinalBehaviours(forActor))));
        }

        public override StatsCollection GetFinalStats(IActor forActor)
        {
            var clone = BaseStats.Clone();

            foreach (var adjective in Adjectives) 
                clone.Increase(adjective.GetFinalStats(forActor));

            clone.Increase(CurrentLocation.GetFinalStats(forActor));

            foreach (var faction in FactionMembership) 
                clone.Increase(faction.GetFinalStats(forActor));

            foreach (var item in Items) 
                clone.Increase(item.GetFinalStats(forActor));

            return clone;
        }
        
        public StatsCollection GetFinalStats()
        {
            return GetFinalStats(this);
        }

        
        public IEnumerable<IAction> GetFinalActions()
        {
            return GetFinalActions(this);
        }

        
        public IEnumerable<IBehaviour> GetFinalBehaviours()
        {
            return GetFinalBehaviours(this);
        }

        public bool IsAwareOf(IActor other)
        {
            return CurrentLocation.GetPoint() == other.CurrentLocation.GetPoint();
        }

        public virtual bool CanEquip(IItem item,out string reason)
        {

            return item.CanEquip(this,out reason);
        }


        /// <inheritdoc />
        public bool CanUnEquip(IItem item, out string reason)
        {
            //already equipped, dead etc
            if (!item.IsEquipped)
            {
                reason = "Not Equipped";
                return false;
            }

            
            if (!item.CanUnEquip(this,out reason))
                return false;

            reason = null;
            return true;
        }

        /// <inheritdoc />
        public double AttitudeTo(IActor other)
        {
            return this.CurrentLocation.World.Relationships.SumBetween(this,other);
        }

        /// <summary>
        /// Returns <see cref="HasStats.Name"/> with a prefix of "Dead" if the <see cref="Actor"/> is dead.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return (Dead ? "Dead ":"") + Name;
        }

        /// <summary>
        /// Returns all 'haves' (sub objects), this includes <see cref="Items"/>, Adjectives, <see cref="FactionMembership"/> etc and all 'haves' of those too (recursively).  Does not include equipable items that are not currently equiped.
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<IHasStats> GetAllHaves()
        {
            if(Dead)
                return new IHasStats[0];

            return 
                base.GetAllHaves()
                    .Union(Items.Where(IncludeInHaves))
                    .Union(Items.Where(IncludeInHaves).SelectMany(i=>i.GetAllHaves())
                    .Union(FactionMembership));
        }

        private bool IncludeInHaves(IItem item)
        {
            //if it requires equipping
            return item.Slot == null || item.IsEquipped;

            //Note that we cannot use item.RequirementsMet because we would
            //rapidly run into stack overflows e.g. where requirement is that player Has something!
        }

        /// <inheritdoc />
        public double DistanceTo(IActor actor)
        {
            var world = CurrentLocation.World;

            return world.Map.GetPoint(CurrentLocation).Distance(world.Map.GetPoint(actor.CurrentLocation));
        }

        /// <inheritdoc />
        public IActor BestFriend(bool inSameLocation, double threshold)
        {
            var world = CurrentLocation.World;
            var relationships = world.Relationships;
            var consider = inSameLocation ? 
                    GetCurrentLocationSiblings(false)
                    : world.Population.ToArray();

            return consider.OrderByDescending(a=>relationships.SumBetween(this,a))
                            .Where(a=>relationships.SumBetween(this,a) > threshold)
                            .FirstOrDefault();
        }

        /// <inheritdoc />
        public IActor WorstEnemy(bool inSameLocation, double threshold)
        {
            var world = CurrentLocation.World;
            var relationships = world.Relationships;
            var consider = inSameLocation ? 
                    GetCurrentLocationSiblings(false)
                    : world.Population.ToArray();

            return consider.OrderBy(a=>relationships.SumBetween(this,a))
                            .Where(a=>relationships.SumBetween(this,a) < threshold)
                            .FirstOrDefault();
        }

        /// <inheritdoc />
        public IItem SpawnItem(ItemBlueprint blue)
        {
            return SpawnItem(CurrentLocation.World.ItemFactory.Create(CurrentLocation.World,blue));
        }

        /// <inheritdoc />
        public IItem SpawnItem(Guid g)
        {
            return SpawnItem(CurrentLocation.World.GetBlueprint<ItemBlueprint>(g));
        }

        /// <inheritdoc />
        public IItem SpawnItem(string name)
        {
            return SpawnItem(CurrentLocation.World.GetBlueprint<ItemBlueprint>(name));
        }

        /// <inheritdoc />
        public IAction SpawnAction(ActionBlueprint blue)
        {
            return CurrentLocation.World.ActionFactory.Create(CurrentLocation.World,this,blue);
        }

        /// <inheritdoc />
        public IAction SpawnAction(Guid g)
        {
            return CurrentLocation.World.ActionFactory.Create(CurrentLocation.World,this,g);
        }

        /// <inheritdoc />
        public IAction SpawnAction(string name)
        {
            return CurrentLocation.World.ActionFactory.Create(CurrentLocation.World,this,name);
        }

        /// <summary>
        /// Adds the item to the <see cref="Items"/> inventory of the actor
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected virtual IItem SpawnItem(IItem item)
        {
            Items.Add(item);
            return item;
        }

        /// <inheritdoc />
        public IBehaviour SpawnBehaviour(string name)
        {
            return CurrentLocation.World.BehaviourFactory.Create(CurrentLocation.World,this,name);
        }
        /// <inheritdoc />
        public IBehaviour SpawnBehaviour(Guid g)
        {
            return CurrentLocation.World.BehaviourFactory.Create(CurrentLocation.World,this,g);
        }
        /// <inheritdoc />
        public void Equip(IItem item)
        {
            if (CanEquip(item, out _))
                item.IsEquipped = true;
        }
        
        /// <inheritdoc />
        public IInjurySystem GetBestInjurySystem()
        {
            var weaponInjurysystem = Items.Where(i => i.IsEquipped)
                .Select(i => i.GetBestInjurySystem(this))
                .FirstOrDefault(s => s != null);

            return 
                //injury system of your currently equipped item
                weaponInjurysystem ?? 
                //your innate injury system
                InjurySystem ??
                //the default world injury system
                CurrentLocation.World.GetDefaultInjurySystem();
        }

        /// <inheritdoc />
        public void Heal(IUserinterface ui, Guid round,string s)
        {
            Adjectives.OfType<IInjured>().FirstOrDefault(i=>i.Is(s))?.Heal(ui,round);
        }
    }
}
