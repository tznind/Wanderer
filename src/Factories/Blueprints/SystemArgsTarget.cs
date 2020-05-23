namespace Wanderer.Factories.Blueprints
{
    public enum SystemArgsTarget
    {
        /// <summary>
        /// The primary initiating actor e.g. the one performing the action, the one initiating the dialogue
        /// </summary>
        Aggressor,

        /// <summary>
        /// The secondary object (room, actor, item etc) being targeted by the action.  In dialogue the one being talked to, in actions, the target of the action etc
        /// </summary>
        Recipient,

        /// <summary>
        /// The location in which the event is taking place
        /// </summary>
        Room,

        /// <summary>
        /// Only applies to Behaviour event handlers, the owner of the behaviour (item, actor, room etc)
        /// </summary>
        Owner
    }
}