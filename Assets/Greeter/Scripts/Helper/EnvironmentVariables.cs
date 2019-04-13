﻿using System;
using System.IO;
using UnityEngine;

public static class EnvironmentVariables
{
    public static string GetVariable(string variable)
    {
        string value = "";

        // Check whether the environment variable exists.
        value = Environment.GetEnvironmentVariable(variable, EnvironmentVariableTarget.User);

        Debug.Log(value);
        if (value == null)
        {
            Environment.SetEnvironmentVariable(variable, "None");

            // Now retrieve it.
            value = Environment.GetEnvironmentVariable(variable);
        }
        // Display the value.
        Debug.Log(value);
        return value;
    }
}
