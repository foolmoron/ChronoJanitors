﻿using System;

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public static class ArrayExtensions {

    public static T Random<T>(this IList<T> array) {
        return array[Mathf.FloorToInt(UnityEngine.Random.value * array.Count)];
    }

    public static T RandomSeeded<T>(this IEnumerable<T> array, System.Random random) {
        return array.ElementAt(random.Next(array.Count()));
    }

    public static int Count<T>(this IList<T> array, Func<T, bool> countPredicate) {
        var count = 0;
        for (int i = 0; i < array.Count; i++) {
            if (countPredicate(array[i]))
                count++;
        }
        return count;
    }

    public static void ForEach<T>(this IList<T> array, Action<T> action) {
        for (int i = 0; i < array.Count; i++) {
            action(array[i]);
        }
    }

    public static TReturn[] Map<TArray, TReturn>(this IList<TArray> array, Func<TArray, TReturn> map) {
        var newArray = new TReturn[array.Count];
        for (int i = 0; i < array.Count; i++) {
            newArray[i] = map(array[i]);
        }
        return newArray;
    }

    public static TReturn[] MapWith<TArray1, TArray2, TReturn>(this IList<TArray1> array1, IList<TArray2> array2, Func<TArray1, TArray2, TReturn> map) {
        var newArray = new TReturn[array1.Count];
        for (int i = 0; i < array1.Count; i++) {
            newArray[i] = map(array1[i], array2[i]);
        }
        return newArray;
    }

    public static TReturn[] Map<TArray, TReturn>(this IList<TArray> array, Func<TArray, TReturn> map, TReturn[] preallocatedArray) {
        for (int i = 0; i < preallocatedArray.Length; i++) {
            preallocatedArray[i] = i < array.Count ? map(array[i]) : default(TReturn);
        }
        return preallocatedArray;
    }

    public static int IndexOf<T>(this IList<T> array, T item) {
        for (int i = 0; i < array.Count; i++) {
            if ((array[i] == null && item == null) || (array[i] != null && array[i].Equals(item)))
                return i;
        }
        return -1;
    }

    public static bool Contains<T>(this IList<T> array, T item) {
        for (int i = 0; i < array.Count; i++) {
            if ((array[i] == null && item == null) || (array[i] != null && array[i].Equals(item)))
                return true;
        }
        return false;
    }

    public static bool Contains<TArray, TItem>(this IList<TArray> array, TItem item, Func<TArray, TItem, bool> comparator) {
        for (int i = 0; i < array.Count; i++) {
            if (comparator(array[i], item))
                return true;
        }
        return false;
    }

    public static bool ContainsIgnoreCase(this IList<string> array, string item) {
        for (int i = 0; i < array.Count; i++) {
            if (string.Compare(array[i], item, StringComparison.OrdinalIgnoreCase) == 0)
                return true;
        }
        return false;
    }

    public static IList<T> Shuffle<T>(this IList<T> array) {
        // fisher-yates
        for (int i = 0; i < array.Count - 1; i++) {
            // random index [i, n)
            var index = Mathf.FloorToInt(UnityEngine.Random.value * (array.Count - i)) + i;
            // swap
            var swap = array[i];
            array[i] = array[index];
            array[index] = swap;
        }
        return array;
    }

    public static int FindNearestSorted<T>(this IList<T> array, float value, Func<T, float> metric) {
        // binary search, requires already sorted list
        var first = 0;
        var last = array.Count - 1;
        while (((last - first) / 2) > 0) {
            var middle = first + (last - first) / 2;
            if (metric(array[middle]) < value)
                first = middle + 1;
            else
                last = middle;
        }
        var diffPrev = first > 0 ? Math.Abs(metric(array[first - 1]) - value) : Mathf.Infinity;
        var diffFirst = Math.Abs(metric(array[first]) - value);
        var diffLast = Math.Abs(metric(array[last]) - value);
        return (diffFirst <= diffLast) ? (diffPrev < diffFirst) ? first - 1 : first : last;
    }
}