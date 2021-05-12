using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public static class Extensions
{
    public const int NULL = int.MinValue;
    //
    // Adding only a unique item to an array
    //
    public static bool AddUniqueItem<T>(this List<T> list, T item)
    {
        if (list.Contains(item) == false)
        {
            list.Add(item);
            return true;
        }

        return false;
    }

    public static void SetMask(this int[] array, int value)
    {
        array[value / 31] ^= 1 << (value % 31);
    }

    public static int BitCount(this int value)
    {
        value -= (value >> 1) & 0x55555555;
        value = ((value >> 2) & 0x33333333) + (value & 0x33333333);
        value = ((((value >> 4) + value) & 0x0F0F0F0F) * 0x01010101) >> 24;
        return value;
    }

    public static int BitCount(this int[] array)
    {
        int result = 0;
        for (int i = 0; i < array.Length; i++)
            result += array[i].BitCount();
        return result;
    }

    public static int BitPos(this int[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] > 0)
            {
                if (array[i] > 0x00FFFFFF)
                {
                    return (array.BitPos(i, 24) + i * 31);
                }
                else if (array[i] > 0x0000FFFF)
                {
                    return (array.BitPos(i, 16) + i * 31);
                }
                else if (array[i] > 0x000000FF)
                {
                    return (array.BitPos(i, 8) + i * 31);
                }
                else if (array[i] > 0x00000000)
                {
                    return (array.BitPos(i, 0) + i * 31);
                }
            }
        }
        return NULL;
    }

    public static int BitPos(this int[] array, int index, int from)
    {
        int oldValue = array[index];
        do
        {
            if (((array[index] >> from) & 1) > 0)
            {
                array[index] ^= 1 << from;
                return from;
            }
            if (((array[index] >> ++from) & 1) > 0)
            {
                array[index] ^= 1 << from;
                return from;
            }
            if (((array[index] >> ++from) & 1) > 0)
            {
                array[index] ^= 1 << from;
                return from;
            }
            if (((array[index] >> ++from) & 1) > 0)
            {
                array[index] ^= 1 << from;
                return from;
            }
            if (((array[index] >> ++from) & 1) > 0)
            {
                array[index] ^= 1 << from;
                return from;
            }
            if (((array[index] >> ++from) & 1) > 0)
            {
                array[index] ^= 1 << from;
                return from;
            }
            if (((array[index] >> ++from) & 1) > 0)
            {
                array[index] ^= 1 << from;
                return from;
            }
            if (((array[index] >> ++from) & 1) > 0)
            {
                array[index] ^= 1 << from;
                return from;
            }
        } while (Interlocked.CompareExchange(ref array[index], 0, oldValue) != oldValue);
        return NULL;
    }
}