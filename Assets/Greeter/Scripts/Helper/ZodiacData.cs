using System.Collections.Generic;

public class Date
{
    public string content { get; set; }
    public string item { get; set; }
    public int money { get; set; }
    public int total { get; set; }
    public int job { get; set; }
    public string color { get; set; }
    public object day { get; set; }
    public int love { get; set; }
    public int rank { get; set; }
    public string sign { get; set; }
}

public class Horoscope
{
    public List<Date> Dates { get; set; }
}

public class RootObject
{
    public Horoscope horoscope { get; set; }
}