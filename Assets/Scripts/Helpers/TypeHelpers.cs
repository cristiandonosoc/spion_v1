using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public static class TypeHelpers {

    public static T_To TypeToType<T_From, T_To>(T_From from) {
        return (T_To)Convert.ChangeType(from, typeof(T_To));
    }

    public static bool TypeHasAttribute(Type type, Type attributeType) {
        return (type.GetCustomAttributes(attributeType, false).Length > 0);
    }

    public static Type[] GetAttributedTypes(Type attributeType) {
        List<Type> types = new List<Type>();
        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies()) {
            foreach (Type type in assembly.GetTypes()) {
                if (TypeHasAttribute(type, attributeType)) {
                    types.Add(type);
                }
            }
        }
        return types.ToArray();
    }

    public static string[] GetNames<T>(T[] objs) {
        string[] names = new string[objs.Length];
        for (int i = 0; i < objs.Length; i++) {
            names[i] = objs[i].ToString();
        }
        return names;
    }

    public static int[] GetEnumValues(Type enumType) {
        Array values = Enum.GetValues(enumType);
        int[] intValues = new int[values.Length];
        for (int i = 0; i < values.Length; i++) {
            intValues[i] = (int)values.GetValue(i);
        }
        return intValues;
    }

    public static Dictionary<int, string> GetEnumValueNameMap(Type enumType) {
        int[] enumValues = GetEnumValues(enumType);
        string[] enumNames = Enum.GetNames(enumType);

        Dictionary<int, string> enumMap = new Dictionary<int, string>();
        for (int i = 0; i < enumValues.Length; i++) {
            enumMap.Add(enumValues[i], enumNames[i]);
        }
        return enumMap;
    }

    /// <summary>
    /// Searches the element in the array. Uses the defined == operator.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <param name="array"></param>
    /// <returns>0-based index if found, -1 otherwise.</returns>
    public static int GetIndexInArray<T, U>(T obj, U[] array) where T : class
                                                              where U : class {
        int index = -1;
        for (int i = 0; i < array.Length; i++) {
            if (array[i] == obj) {
                return i;
            }
        }
        return index;
    }

    /// <summary>
    /// Searches the element in the array. Uses the defined == operator.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <param name="array"></param>
    /// <returns>0-based index if found, -1 otherwise.</returns>
    public static int GetIndexInArray(int obj, int[] array) {
        int index = -1;
        for (int i = 0; i < array.Length; i++) {
            if (array[i] == obj) {
                return i;
            }
        }
        return index;
    }
}
