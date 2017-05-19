using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public static class TypeHelpers {

    public static Type[] GetAttributedTypes(Type attributeType) {
        List<Type> types = new List<Type>();
        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies()) {
            foreach (Type type in assembly.GetTypes()) {
                if (type.GetCustomAttributes(attributeType, true).Length > 0) {
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
}
