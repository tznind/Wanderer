using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Wanderer;
using Wanderer.Actors;
using Wanderer.Dialogues;

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

        public bool IsExhausted => _index == _getChoiceReturns.Length;

        public void NewGame()
        {
            throw new NotImplementedException();
        }

        public EventLog Log { get; } = new EventLog();

        public FixedChoiceUI(params object[] getChoiceReturns)
        {
            _getChoiceReturns = getChoiceReturns ?? new object[]{null};
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
                object oChosen = _getChoiceReturns[_index++];

                //allow test harness users to specify strings instead of objects if so desired (e.g. to select a DialogueOption)
                if (oChosen is string)
                    oChosen = options.SingleOrDefault(o => 
                          //allow picking by Name        
                        (o is IHasStats s && s.Name.Equals(oChosen))
                        //allow picking by ToString
                        || o.ToString().Equals(oChosen)) ?? oChosen;

                chosen = (T)oChosen;

                if (chosen == null)
                    return false;
            }
            catch (InvalidCastException)
            {
                throw new OptionNotAvailableException($"Chosen answer for Test did not match Type requested for GetChoice of:{title} ({body}).  Required Type was {typeof(T)} And listed available options were:{Environment.NewLine}{string.Join(Environment.NewLine,options)} " );
            }

            if(!options.Contains(chosen))
                throw new OptionNotAvailableException($"Chosen test answer was not one of the listed options for GetChoice of:{title} ({body})");
            return true;
        }
        
        public void ShowMessage(string title, string body)
        {
            MessagesShown.Add(body);
        }
    }
}