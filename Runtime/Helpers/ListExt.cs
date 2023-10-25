using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

namespace traVRsal.SDK
{
    public static class ListExt
    {
        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = ThreadSafeRandom.ThisThreadsRandom.Next(n + 1);
                (list[k], list[n]) = (list[n], list[k]);
            }
        }

        public static void Shuffle<T>(this IList<T> list, Random random)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                (list[k], list[n]) = (list[n], list[k]);
            }
        }

        public static T[] ShiftRight<T>(this T[] arr)
        {
            T[] result = new T[arr.Length];
            for (int i = 0; i < arr.Length; i++)
            {
                result[(i + 1) % arr.Length] = arr[i];
            }
            return result;
        }

        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (T obj in source)
            {
                action(obj);
            }
            return source;
        }

        public static T[] Append<T>(this T[] data, T element)
        {
            Array.Resize(ref data, data.Length + 1);
            data[data.Length - 1] = element;

            return data;
        }

        public static void AddRange<T>(this IList<T> list, IEnumerable<T> collection)
        {
            if (list is List<T> result)
            {
                result.AddRange(collection);
            }
            else
            {
                foreach (T obj in collection)
                {
                    list.Add(obj);
                }
            }
        }

        public static bool AreEqual(Vector2[] list1, Vector2[] list2)
        {
            if (list1 == null && list2 == null) return true;
            if (list1 == null || list2 == null) return false;
            if (list1.Length != list2.Length) return false;
            for (int i = 0; i < list1.Length; i++)
            {
                if (!list1[i].ApproximatelyEqual(list2[i])) return false;
            }

            return true;
        }

        public static bool AreEqual(Vector3[] list1, Vector3[] list2)
        {
            if (list1 == null && list2 == null) return true;
            if (list1 == null || list2 == null) return false;
            if (list1.Length != list2.Length) return false;
            for (int i = 0; i < list1.Length; i++)
            {
                if (!list1[i].ApproximatelyEqual(list2[i])) return false;
            }

            return true;
        }

        public static bool AreEqual(Vector3[] list1, Vector3[] list2, float tolerance)
        {
            if (list1 == null && list2 == null) return true;
            if (list1 == null || list2 == null) return false;
            if (list1.Length != list2.Length) return false;
            for (int i = 0; i < list1.Length; i++)
            {
                if (!list1[i].ApproximatelyEqual(list2[i], tolerance)) return false;
            }

            return true;
        }

        public static T[] Slice<T>(this T[] source, int start, int end)
        {
            if (source == null) return null;
            if (end < 0) end = source.Length + end;

            int len = end - start;

            T[] res = new T[len];
            for (int i = 0; i < len; i++)
            {
                res[i] = source[i + start];
            }

            return res;
        }

        public static void AddRange<T>(this HashSet<T> hashSet, IEnumerable<T> range)
        {
            foreach (T obj in range)
            {
                hashSet.Add(obj);
            }
        }

        public static void SetLength<T>(this IList<T> list, int length)
        {
            if (list == null) throw new ArgumentNullException(nameof(list));
            if (length < 0) throw new ArgumentException("Length must be larger than or equal to 0.");
            if (list.GetType().IsArray) throw new ArgumentException("Cannot use the SetLength extension method on an array. Use Array.Resize or the ListUtilities.SetLength(ref IList<T> list, int length) overload.");
            while (list.Count < length)
            {
                list.Add(default);
            }
            while (list.Count > length)
            {
                list.RemoveAt(list.Count - 1);
            }
        }

#if UNITY_2023_1_OR_NEWER
#else
        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source) => new HashSet<T>(source);
#endif
        
        /// <summary>
        /// Topological Sorting (Kahn's algorithm) 
        /// </summary>
        /// <remarks>https://en.wikipedia.org/wiki/Topological_sorting</remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="nodes">All nodes of directed acyclic graph.</param>
        /// <param name="edges">All edges of directed acyclic graph.</param>
        /// <returns>Sorted node in topological order.</returns>
        public static List<T> TopologicalSort<T>(HashSet<T> nodes, HashSet<Tuple<T, T>> edges) where T : IEquatable<T>
        {
            // Empty list that will contain the sorted elements
            List<T> l = new List<T>();

            // Set of all nodes with no incoming edges
            HashSet<T> s = new HashSet<T>(nodes.Where(n => edges.All(e => e.Item2.Equals(n) == false)));

            // while S is non-empty do
            while (s.Any())
            {
                //  remove a node n from S
                T n = s.First();
                s.Remove(n);

                // add n to tail of L
                l.Add(n);

                // for each node m with an edge e from n to m do
                foreach (Tuple<T, T> e in edges.Where(e => e.Item1.Equals(n)).ToList())
                {
                    T m = e.Item2;

                    // remove edge e from the graph
                    edges.Remove(e);

                    // if m has no other incoming edges then
                    if (edges.All(me => me.Item2.Equals(m) == false))
                    {
                        // insert m into S
                        s.Add(m);
                    }
                }
            }

            // if graph has edges then
            if (edges.Any())
            {
                // return error (graph has at least one cycle)
                return null;
            }

            // return L (a topologically sorted order)
            return l;
        }
    }
}