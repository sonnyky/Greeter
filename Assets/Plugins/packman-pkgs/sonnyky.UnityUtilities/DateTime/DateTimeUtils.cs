
using System;
using System.Globalization;
using UnityEngine;

public static class DateTimeUtils 
{
    public static bool DateTimeFormatCheck(string inputString)
    {
        DateTime dDate;

        bool test = DateTime.TryParseExact(inputString,
                       "yyyy-MM-dd",
                       CultureInfo.InvariantCulture,
                       DateTimeStyles.None,
                       out dDate);
        return test;
    }
}
