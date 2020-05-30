using System;
using System.Linq;
using Wanderer.Actors;
using Wanderer.Rooms;

namespace Wanderer
{
    /// <summary>
    /// Example implementation of <see cref="IUserinterface"/> based on <see cref="Console.WriteLine()"/> and <see cref="Console.ReadLine"/>
    /// </summary>
    public class ExampleUserInterface : IUserinterface
    {
        /// <inheritdoc />
        public virtual EventLog Log { get; set; } = new EventLog();

        /// <inheritdoc />
        public virtual void ShowStats(IHasStats of)
        {
            Console.WriteLine(of.ToString());
            Console.WriteLine("Adjectives:" + string.Join(",",of.Adjectives));

            Console.WriteLine("Stats:" + string.Join(Environment.NewLine,of.BaseStats.Select(s=>s.Key.ToString() + ':' + s.Value)));
                
            if(of is IActor a)
                Console.WriteLine("Items:" + string.Join(",",a.Items));
            if(of is IRoom r)
                Console.WriteLine("Items:" + string.Join(",",r.Items));
        }

        public virtual bool GetChoice<T>(string title, string body, out T chosen, params T[] options)
        {
            ShowMessage(title,body);
                
            for (int i = 0; i < options.Length; i++) 
                Console.WriteLine(i + ":" + options[i]);

            Console.Write("Option:");
                
            while(true)
            {
                var entered = Console.ReadLine();
                while(int.TryParse(entered, out int idx))
                {
                    if (idx < options.Length && idx >= 0)
                    {
                        chosen = options[idx];
                        return true;
                    }           
                }
            }
        }

        public virtual void ShowMessage(string title, string body)
        {
            Console.WriteLine(title);

            if(!string.IsNullOrEmpty(body))
                Console.WriteLine(body);
        }
    }
}
