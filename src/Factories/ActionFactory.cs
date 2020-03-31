using System;
using Wanderer.Actions;
using Wanderer.Compilation;
using Wanderer.Factories.Blueprints;
using Wanderer.Systems;
using Action = Wanderer.Actions.Action;

namespace Wanderer.Factories
{

    public class ActionFactory : HasStatsFactory<ActionBlueprint, IAction>, IActionFactory
    {
        private TypeCollection _types;

        public ActionFactory()
        {
            _types = Compiler.Instance.TypeFactory.Create<IAction>();
        }

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

            if(blueprint.HotKey.HasValue)
                action.HotKey = blueprint.HotKey.Value;

            action.Effect = blueprint.Effect;
            
            if(blueprint.Targets != null)
                action.Targets = blueprint.Targets;

            action.TargetPrompt = blueprint.TargetPrompt;

            if (action is FightAction fight && blueprint.InjurySystem.HasValue)
                fight.InjurySystem = (IInjurySystem) world.GetSystem(blueprint.InjurySystem.Value);

            onto.BaseActions.Add(action);
            return action;
        }

        public IAction Create(IWorld world, IHasStats onto, string name)
        {
            return Create(world, onto, GetBlueprint(name));
        }

        public IAction Create(IWorld world, IHasStats onto, Guid g)
        {
            return Create(world, onto, GetBlueprint(g));
        }
    }
}