using Wanderer.Factories.Blueprints;
using Wanderer.Plans;
using Wanderer.Systems;

namespace Wanderer.Factories
{

    public class PlanFactory
    {
        public Plan Create(PlanBlueprint blueprint)
        {
            var plan = new Plan
            {
                Name = blueprint.Name,
                Identifier = blueprint.Identifier,
                Weight = blueprint.Weight
            };

            foreach (var blue in blueprint.Condition)
                plan.Condition.AddRange(blue.Create());
            
            plan.Do = blueprint.Do.Create();

            return plan;
        }
        
    }

    
}