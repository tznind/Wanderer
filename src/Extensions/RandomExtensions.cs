using System;
using System.Collections.Generic;
using System.Linq;

namespace Wanderer.Extensions
{
    public static class RandomExtensions
    {
        public static T GetRandom<T>(this IList<T> arr, Random r)
        {
            if (arr == null || !arr.Any())
                return default;

            return arr[r.Next(arr.Count)];
        }
    }
}