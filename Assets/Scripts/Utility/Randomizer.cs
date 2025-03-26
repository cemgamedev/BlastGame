using System.Collections.Generic;
using URandom = UnityEngine.Random;
using System;
using System.Linq;
using UnityEngine;
using Ebleme.Extensions;

namespace Ebleme.Utility {
    public static class Randomizer {
        public static T GetRandomElement<T>(this IEnumerable<T> enumerable) {
            var array = enumerable as T[] ?? enumerable.ToArray();
            return array.Any() ? array.ElementAt(GetRandomNumber(0, array.Length)) : default;
        }

        public static float GetRandomNumber(float min, float max) {
            InitRandomSeed();
            return URandom.Range(min, max);
        }

        public static long GetRandomNumber(long min, long max) {
            InitRandomSeed();
            return (long)URandom.Range(min, max);
        }

        public static int GetRandomNumber(int min, int max) {
            InitRandomSeed();
            return URandom.Range(min, max);
        }

        public static float GetRandomNumber((float min, float max) range) {
            InitRandomSeed();
            return URandom.Range(range.min, range.max);
        }

        public static int GetRandomNumber((int min, int max) range) {
            InitRandomSeed();
            return URandom.Range(range.min, range.max + 1);
        }

        public static int GetRandomNumberExcept(int min, int max, int exceptValue) {
            var random = GetRandomNumber(min, max - 1);
            return random == exceptValue ? (max - 1) : random;
        }

        private static void InitRandomSeed() {
            var seed = Guid.NewGuid().GetHashCode();
            URandom.InitState(seed);
        }

        public static bool GetRandomDecision(float percentChance) {
            return GetRandomNumber(0f, 1f) <= percentChance;
        }

        public static T GetRandomElement<T>(this List<T> list) {
            return list.Count == 0 ? default : list[GetRandomNumber(0, list.Count)];
        }

        public static T GetRandomElement<T>(this T[] array) {
            return array.Length == 0 ? default : array[GetRandomNumber(0, array.Length)];
        }

        public static T GetRandomElement<T>(this List<T> list, params T[] exceptions) {
            if (list.Count == 0)
                return default;

            var available = list.Where(i => !exceptions.Contains(i)).ToList();
            return available[GetRandomNumber(0, available.Count)];
        }

        public static T GetRandomElement<T>(this IEnumerable<T> enumerable, params T[] exceptions) {
            var array = enumerable as T[] ?? enumerable.ToArray();
            if (!array.Any()) return default;

            var available = array.Where(i => !exceptions.Contains(i));
            return available.ElementAt(GetRandomNumber(0, array.Count()));
        }

        public static T GetRandomElement<T>(this T[] array, params T[] exceptions) {
            if (array.Length == 0)
                return default;

            var available = array.Where(i => !exceptions.Contains(i)).ToArray();
            return available[GetRandomNumber(0, available.Length)];
        }

        public static TValue GetRandomValue<TKey, TValue>(this Dictionary<TKey, TValue> dict) {
            return dict.IsNullOrEmpty() ? default : dict.Values.GetRandomElement();
        }

        public static TValue
            GetRandomValue<TKey, TValue>(this Dictionary<TKey, TValue> dict, params TKey[] exceptions) {
            if (dict.IsNullOrEmpty()) {
                return default;
            }

            var available = dict.Keys.Where(i => !exceptions.Contains(i));
            return dict[available.GetRandomElement()];
        }

        public static bool GetRandomDecision() {
            return GetRandomNumber(0, 2) == 0;
        }

        // public static Quaternion GetRandomYRotation(float maxAngle = Constants.MaxEulerAnglesRotation) {
        //     return Quaternion.AngleAxis(GetRandomNumber(0f, maxAngle), Vector3.up);
        // }

        public static Vector3 GetRandomPositionInTorusShape(float minRadius, float maxRadius, Vector3 orgin) {
            float rot = GetRandomNumber(1f, 360f);
            Vector3 direction = Quaternion.AngleAxis(rot, Vector3.up) * Vector3.forward;
            Ray ray = new Ray(orgin, direction);
            return ray.GetPoint(GetRandomNumber(minRadius, maxRadius));
        }
    }
}