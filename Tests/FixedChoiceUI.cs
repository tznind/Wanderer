using System;
using System.Linq;
using StarshipWanderer;
using StarshipWanderer.Actors;

namespace Tests
{
    /// <summary>
    /// Test helper class which handles calls to <see cref="IUserinterface.GetChoice{T}"/> by returning fixed values in order
    /// (uses out parameter so cannot easily use SetupSequence)
    /// </summary>
    class FixedChoiceUI : IUserinterface
    {
        private readonly object[] _getChoiceReturns;
        private int _index;
        public void NewGame()
        {
            throw new NotImplementedException();
        }

        public EventLog Log { get; } = new EventLog();

        public FixedChoiceUI(params object[] getChoiceReturns)
        {
            _getChoiceReturns = getChoiceReturns;
            _index = 0;
            Log.Register();
        }

        public void ShowActorStats(IActor actor)
        {
            throw new NotImplementedException();
        }

        public bool GetChoice<T>(string title, string body, out T chosen, params T[] options)
        {
            if(_index >= _getChoiceReturns.Length)
                throw new Exception($"Did not have an answer for GetChoice of:{title} ({body})");
            try
            {
                chosen = (T) _getChoiceReturns[_index++];
            }
            catch (InvalidCastException)
            {
                throw new Exception($"Chosen answer for Test did not match Type requested for GetChoice of:{title} ({body}).  Required Type was {typeof(T)}");
            }

            if(!options.Contains(chosen))
                throw new Exception($"Chosen test answer was not one of the listed options for GetChoice of:{title} ({body})");
            return true;
        }

        public void Refresh()
        {
        }

        public void ShowMessage(string title, string body)
        {
        }

        public void ShowMessage(string title, LogEntry showThenLog)
        {
            Log.Info(showThenLog);
        }
    }
}