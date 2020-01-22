﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace StarshipWanderer.Extensions
{
    public static class RandomExtensions
    {
        public static IList<T> Shuffle<T>(this IList<T> list, Random r)  
        {  
            int n = list.Count;  
            while (n > 1) {  
                n--;  
                int k = r.Next(n + 1);
                T value = list[k];  
                list[k] = list[n];  
                list[n] = value;  
            }

            return list;
        }

        public static T GetRandom<T>(this IList<T> arr, Random r)
        {
            if (arr == null || !arr.Any())
                return default;

            return arr[r.Next(arr.Count)];
        }
    }
}