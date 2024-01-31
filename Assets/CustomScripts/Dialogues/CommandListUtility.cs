using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public class CommandListUtility
{
    private static MethodInfo[] GetMethods => Type.GetType(nameof(DialogueCommands)).GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Static);

    public static List<string> GetMethodList ()
    {
        List<string> methodCategories = new();

        List<string> methodNameList = GetMethods.Select(m => m.Name).ToList();

        MethodInfo [] methodInfo = GetMethods;

        for (int i = 0; i < methodInfo.Length; i++)
        {
            if (Attribute.IsDefined(methodInfo[i], typeof(CommandCategoryAttribute)))
                methodCategories.Add(methodInfo[i].GetCustomAttribute<CommandCategoryAttribute>().CommandCategory + "/");
            else
                methodCategories.Add(String.Empty);
        }

        for (int i = 0; i < methodNameList.Count; i++)
            methodNameList[i] = methodNameList[i].Insert(0, methodCategories[i]);

        return methodNameList;
    }
}
