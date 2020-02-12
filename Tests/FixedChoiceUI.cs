using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Wanderer;
using Wanderer.Actors;

namespace Tests
{
    /// <summary>
    /// Test helper class which handles calls to <see cref="IUserinterface.GetChoice{T}"/> by returning fixed values in order
    /// (uses out parameter so cannot easily use SetupSequence)
    /// </summary>
    public class FixedChoiceUI : IUserinterface
    {
        private readonly object[] _getChoiceReturns;
        private int _index;
        
        public List<string> MessagesShown = new List<string>();

        /// <summary>
        /// List of calls to <see cref="ShowStats"/>
        /// </summary>
        public List<IHasStats> StatsShown = new List<IHasStats>();

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

        public void ShowStats(IHasStats of)
        {
            StatsShown.Add(of);
        }

        public bool GetChoice<T>(string title, string body, out T chosen, params T[] options)
        {
            //make sure nobody ever presents empty options
            foreach (var o in options) 
                Assert.IsFalse(string.IsNullOrWhiteSpace(o.ToString()));

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
                throw new OptionNotAvailableException($"Chosen test answer was not one of the listed options for GetChoice of:{title} ({body})");
            return true;
        }

        public void Refresh()
        {
        }

        public void ShowMessage(string title, string body)
        {
            MessagesShown.Add(body);
        }

        public void ShowMessage(string title, LogEntry showThenLog)
        {
            Log.Info(showThenLog);
        }
    }
}