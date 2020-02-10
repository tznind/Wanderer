using System;

namespace StarshipWanderer.Effects
{
    public class SetNextDialogue<T> : IEffect<IHasStats>
    {
        public Guid? Guid { get; set; }

        public SetNextDialogue(Guid? guid)
        {
            Guid = guid;
        }

        public void Apply(IHasStats forTarget)
        {
            forTarget.Dialogue.Next = Guid;
        }

        public string? SerializeAsConstructorCall()
        {
            return $"SetNextDialogue<{typeof(T).Name}>({Guid?.ToString() ?? "null"})";
        }

    }
}