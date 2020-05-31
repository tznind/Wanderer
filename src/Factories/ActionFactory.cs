using System;
using System.Linq;
using Wanderer.Actions;
using Wanderer.Compilation;
using Wanderer.Factories.Blueprints;
using Wanderer.Systems;
using Action = Wanderer.Actions.Action;

namespace Wanderer.Factories
{
    /// <summary>
    /// Creates instances of <see cref="IAction"/> based on <see cref="ActionBlueprint"/>
    /// </summary>
    public class ActionFactory : HasStatsFactory<ActionBlueprint, IAction>, IActionFactory
    {
        private TypeCollection _types;

        /// <summary>
        /// Creates a new factory for turning <see cref="ActionBlueprint"/> into <see cref="IAction"/> instances
        /// </summary>
        public ActionFactory()
        {
            _types = Compiler.Instance.TypeFactory.Create<IAction>();

            //add the basic Type blueprints
            foreach (var type in _types)
            {
                Blueprints.Add(new ActionBlueprint()
                {
                    Name = type.Name.Replace("Action", ""),
                    Type = type.Name
                });
            }
        }


        /// <inheritdoc />
        public IAction Create(IWorld world, IHasStats onto, ActionBlueprint blueprint)
        {
            HandleInheritance(blueprint);
            IAction action;

            if(string.IsNullOrWhiteSpace(blueprint.Type))
                action = new Action(onto);
            else
                try
                {
                    var type = _types.GetTypeNamed(blueprint.Type);
                    action = (IAction)Activator.CreateInstance(type,onto);   
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error creating base Type for ActionBlueprint {blueprint}",ex);
                }

            AddBasicProperties(world,action,blueprint,"use");

            if(!string.IsNullOrWhiteSpace(blueprint.Name))
                action.Name = blueprint.Name;

            action.Owner = onto;

            if(blueprint.HotKey.HasValue)
                action.HotKey = blueprint.HotKey.Value;

            action.Effect.AddRange(blueprint.Effect.SelectMany(e=>e.Create()));
            
            if(blueprint.Targets != null)
                action.Targets = blueprint.Targets;

            action.TargetPrompt = blueprint.TargetPrompt;

            if (action is FightAction fight && blueprint.InjurySystem.HasValue)
                fight.InjurySystem = (IInjurySystem) world.GetSystem(blueprint.InjurySystem.Value);

            onto.BaseActions.Add(action);
            return action;
        }

        /// <summary>
        /// Overload that creates the named blueprint
        /// </summary>
        /// <param name="world"></param>
        /// <param name="onto"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public IAction Create(IWorld world, IHasStats onto, string name)
        {
            return Create(world, onto, GetBlueprint(name));
        }


        /// <summary>
        /// Overload that creates the referenced blueprint
        /// </summary>
        /// <param name="world"></param>
        /// <param name="onto"></param>
        /// <param name="g"></param>
        /// <returns></returns>
        public IAction Create(IWorld world, IHasStats onto, Guid g)
        {
            return Create(world, onto, GetBlueprint(g));
        }
    }
}