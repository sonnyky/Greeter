using System;
using System.IO;

public static class EnvironmentVariables
{
    public static string GetVariable(string variable)
    {
        string value = "";

        // Check whether the environment variable exists.
        value = Environment.GetEnvironmentVariable(variable);

        if (value == null)
        {
            Environment.SetEnvironmentVariable(variable, "None");

            // Now retrieve it.
            value = Environment.GetEnvironmentVariable(variable);
        }
        // Display the value.
        Console.WriteLine(variable +  ": {0}\n", value);
        return value;
    }
}
