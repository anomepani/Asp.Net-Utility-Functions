# Asp.Net-Utility-Functions
This project is collection of most useful utility function for Asp.Net WebForm as well as Asp.Net MVC project.
All this method is collected from Internet sources like stackoverflow.com so it will be useful to many developer at one place.

##See listed below most useful functions

### 1. Replace all special character in input string with '-' character
```cs
      public static string ReplaceSpecialChar(string input)
      {
        Regex rgx = new Regex("[^a-zA-Z0-9]");
        input = rgx.Replace(input, "-");
        return input;
      }
```

### 2. Convert Unix long timestamp to C# DateTime object

```cs
     public static DateTime ConvertUNIXTimeStampTODateTime(long date)
     {
        DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
        dtDateTime = dtDateTime.AddSeconds(date).ToLocalTime();
        return dtDateTime;
     }
```
