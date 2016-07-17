using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;
using System.Web;

namespace Utilities
{
    public static class CommonHelpers
    {
        public enum UserRole
        {
            SuperAdmin = 1, Reporter = 2
        }

        public enum ArticleStatus
        {
            Pending = 1, Approved = 2, Disapproved = 3
        }
        /// <summary>
        /// Replace all special characters
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ReplaceSpecialChar(string input)
        {

            Regex rgx = new Regex("[^a-zA-Z0-9]");
            input = rgx.Replace(input, "-");
            return input;
        }


        public static long ConvertToUnixTimestamp(this DateTime target)
        {
            var date = new DateTime(1970, 1, 1, 0, 0, 0, target.Kind);
            var unixTimestamp = System.Convert.ToInt64((target - date).TotalSeconds);

            return unixTimestamp;
        }

        public static DateTime ConvertToDateTime(this DateTime target, long timestamp)
        {
            var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, target.Kind);

            return dateTime.AddSeconds(timestamp);
        }

        public static DateTime ConvertUNIXTimeStampTODateTime(long date)
        {
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(date).ToLocalTime();
            return dtDateTime;
        }

        /// <summary>
        /// Convert UTC Date to Local TimeZone  
        /// </summary>
        /// <param name="date"></param>
        /// <param name="ClientTimeZoneoffset"></param>
        /// <returns></returns>
        public static DateTime getClientTime(string date, object ClientTimeZoneoffset)
        {

            if (ClientTimeZoneoffset != null)
            {
                string Temp = ClientTimeZoneoffset.ToString().Trim();
                if (!Temp.Contains("+") && !Temp.Contains("-"))
                {
                    Temp = Temp.Insert(0, "+");
                }
                //Retrieve all system time zones available into a collection
                ReadOnlyCollection<TimeZoneInfo> timeZones = TimeZoneInfo.GetSystemTimeZones();
                DateTime startTime = DateTime.Parse(date);
                DateTime _now = DateTime.Parse(date);
                foreach (TimeZoneInfo timeZoneInfo in timeZones)
                {
                    if (timeZoneInfo.ToString().Contains(Temp))
                    {
                        TimeZoneInfo tst = TimeZoneInfo.FindSystemTimeZoneById(timeZoneInfo.Id);
                        _now = TimeZoneInfo.ConvertTime(startTime, TimeZoneInfo.Utc, tst);
                        if (tst.SupportsDaylightSavingTime)
                        {
                            _now = _now.AddHours(-1);
                        }
                        break;
                    }
                }
                return _now;
            }
            else
                return DateTime.Parse(date);
        }

        /// <summary>
        /// Convert UTc Date timt to localtime based on timezone
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime UtcToLocalTime(string date)
        {
            string ClientTimeZoneoffset;

            ClientTimeZoneoffset = CookieHelper.getCurrentTimeZone();

            if (!string.IsNullOrEmpty(ClientTimeZoneoffset))
            {
                string Temp = ClientTimeZoneoffset.ToString().Trim();
                if (!Temp.Contains("+") && !Temp.Contains("-"))
                {
                    Temp = Temp.Insert(0, "+");
                }
                //Retrieve all system time zones available into a collection
                ReadOnlyCollection<TimeZoneInfo> timeZones = TimeZoneInfo.GetSystemTimeZones();
                DateTime startTime = DateTime.Parse(date);
                DateTime _now = DateTime.Parse(date);
                foreach (TimeZoneInfo timeZoneInfo in timeZones)
                {
                    if (timeZoneInfo.ToString().Contains(Temp))
                    {
                        TimeZoneInfo tst = TimeZoneInfo.FindSystemTimeZoneById(timeZoneInfo.Id);
                        _now = TimeZoneInfo.ConvertTime(startTime, TimeZoneInfo.Utc, tst);
                        if (tst.SupportsDaylightSavingTime)
                        {
                            _now = _now.AddHours(-1);
                        }
                        break;
                    }
                }
                return _now;
            }
            else
                return DateTime.Parse(date);
        }

        public enum Status { Active = 1, Inactive = 2, Delete = 3 };

        public enum ErrorType
        {
            EmailError = 1, General = 2
        }

        public enum EmailStatus
        {
            PendingToSent = 1, Sent = 2
        }


        public static string RemoveSpecialCharacters(string str)
        {

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < str.Length; i++)
            {
                if ((str[i] >= '0' && str[i] <= '9') || (str[i] >= 'A' && str[i] <= 'z' || (str[i] == '_' || str[i] == ' ')))
                    sb.Append(str[i]);
            }

            return sb.ToString();
        }
        /// <summary>
        /// create folder
        /// </summary>
        /// <param name="strpath">Specify the Fullpath folderpath to create folder</param>
        /// <returns></returns>

        public static bool CreateFolder(string strpath)
        {
            try
            {
                if (!Directory.Exists(strpath))
                {
                    Directory.CreateDirectory(strpath);
                    return true;
                }
                else
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Delete Folder
        /// </summary>
        /// <param name="folderpath">Full folder path</param>
        /// <returns></returns>
        public static bool DeleteFolder(string folderpath)
        {
            bool boolRet = false;
            try
            {
                if (Directory.Exists(folderpath))
                {
                    Directory.Delete(folderpath);
                    boolRet = true;
                }
            }
            catch
            {
                boolRet = false;
            }
            return boolRet;
        }

        /// <summary>
        /// Delete file
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public static bool DeleteFile(string filepath)
        {
            bool isDelete = false;
            try
            {
                if (File.Exists(filepath))
                {
                    File.Delete(filepath);
                    isDelete = true;
                }
            }
            catch
            {
            }
            return isDelete;
        }



        public static string GetShortDescription(string Description)
        {
            string strDesc = string.Empty;

            try
            {
                if (Convert.ToString(Description) != string.Empty)
                {
                    if (Description.Trim().Length > 100)
                    {
                        strDesc = Description.Trim().Substring(0, 100) + "...";
                    }
                    else
                    {
                        strDesc = Description.Trim();
                    }

                }
            }
            catch (Exception)
            {


            }
            return strDesc;

        }

        public static string GetShortTitle(string Title)
        {
            try
            {
                string strDesc = string.Empty;
                if (Convert.ToString(Title) != string.Empty)
                {
                    if (Title.Trim().Length > 40)
                    {
                        strDesc = Title.Trim().Substring(0, 40);
                    }
                    else
                    {
                        strDesc = Title.Trim();
                    }

                }
                return strDesc;
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// IMAGE RESIZE PROPORTIONALLY
        /// Return a size object of the newly calculated proportional width/height params based
        /// on the original image size
        /// </summary>
        /// <param name="OrgWidth"></param>
        /// <param name="OrgHeight"></param>
        /// <returns></returns>
        private static Size ImageResizeProportional(Size OrgSize, Size NewSize)
        {
            int sourceWidth = OrgSize.Width;
            int sourceHeight = OrgSize.Height;

            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;

            nPercentW = ((float)NewSize.Width / (float)sourceWidth);
            nPercentH = ((float)NewSize.Height / (float)sourceHeight);

            if (nPercentH < nPercentW)
                nPercent = nPercentH;
            else
                nPercent = nPercentW;

            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);

            Size newSize = new Size(destWidth, destHeight);
            return newSize;
        }
    }

    public class CookieHelper
    {
        public static string _cookiename;

        //static readonly string Cookiename = "partylife";
        public static string Cookiename
        {
            get { return _cookiename; }
            set { _cookiename = value; }
        }

        static CookieHelper()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        /// <summary>
        /// Get current time zone from client 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string getCurrentTimeZone(string key)
        {
            HttpCookie httpCookie = HttpContext.Current.Request.Cookies.Get(key);
            if (httpCookie != null)
            {
                return httpCookie.Value;

            }
            else return string.Empty;

        }

        public static string getCurrentTimeZone()
        {
            HttpCookie httpCookie = HttpContext.Current.Request.Cookies.Get("TimeOffset");
            if (httpCookie != null)
            {
                return httpCookie.Value;
            }
            else return string.Empty;

        }

        public static bool AddItemToCookie(string key, string value)
        {
            bool isSuccessful = false;
            try
            {
                HttpCookie httpCookie = HttpContext.Current.Request.Cookies.Get(Cookiename);
                if (httpCookie == null)
                {
                    httpCookie = new HttpCookie(Cookiename);
                    httpCookie.Values[key] = value;
                }
                else httpCookie.Values[key] = value;

                httpCookie.Expires = DateTime.Now.AddMinutes(30);
                HttpContext.Current.Response.Cookies.Add(httpCookie);
                isSuccessful = true;
            }
            catch (Exception) { isSuccessful = false; }
            return isSuccessful;
        }

        public static string GetItemFromCookie(string key)
        {
            HttpCookie httpCookie = HttpContext.Current.Request.Cookies.Get(Cookiename);
            if (httpCookie != null)
            {
                if (httpCookie.HasKeys) return httpCookie[key];
                else return string.Empty;
            }
            else return string.Empty;
        }

        public static void DeleteItemFromCookie(string key)
        {
            HttpCookie httpCookie = HttpContext.Current.Request.Cookies.Get(Cookiename);
            if (httpCookie != null)
            {
                httpCookie.Values[key] = string.Empty;
            }
        }

        public static void UpdateCookieExpiry()
        {
            HttpCookie httpCooke = HttpContext.Current.Request.Cookies.Get(Cookiename);
            if (httpCooke != null)
            {
                httpCooke.Expires.AddMinutes(30);
            }
        }

        public static void DeleteCookie()
        {
            HttpCookie httpCookie = HttpContext.Current.Request.Cookies.Get(Cookiename);
            if (httpCookie != null)
            {
                httpCookie.Expires = DateTime.Now.AddDays(-1);
                HttpContext.Current.Response.Cookies.Add(httpCookie);
            }
        }
    }

    #region Generate SEO Friendly URl like StackOverflow
    #region Create permananet link link or SEO Friendly url in mvc
    public static class Slug
    {

        #region  Solution-1

        /// <summary>
        /// Produces optional, URL-friendly version of a title, "like-this-one". 
        /// hand-tuned for speed, reflects performance refactoring contributed
        /// by John Gietzen (user otac0n) 
        /// </summary>
        public static string URLFriendly(string title)
        {
            if (title == null) return "";

            const int maxlen = 80;
            int len = title.Length;
            bool prevdash = false;
            var sb = new StringBuilder(len);
            char c;

            for (int i = 0; i < len; i++)
            {
                c = title[i];
                if ((c >= 'a' && c <= 'z') || (c >= '0' && c <= '9'))
                {
                    sb.Append(c);
                    prevdash = false;
                }
                else if (c >= 'A' && c <= 'Z')
                {
                    // tricky way to convert to lowercase
                    sb.Append((char)(c | 32));
                    prevdash = false;
                }
                else if (c == ' ' || c == ',' || c == '.' || c == '/' ||
                    c == '\\' || c == '-' || c == '_' || c == '=')
                {
                    if (!prevdash && sb.Length > 0)
                    {
                        sb.Append('-');
                        prevdash = true;
                    }
                }
                else if ((int)c >= 128)
                {
                    int prevlen = sb.Length;
                    sb.Append(RemapInternationalCharToAscii(c));
                    if (prevlen != sb.Length) prevdash = false;
                }
                if (i == maxlen) break;
            }

            if (prevdash)
                return sb.ToString().Substring(0, sb.Length - 1);
            else
                return sb.ToString();
        }

        public static string RemapInternationalCharToAscii(char c)
        {
            string s = c.ToString().ToLowerInvariant();
            if ("àåáâäãåą".Contains(s))
            {
                return "a";
            }
            else if ("èéêëę".Contains(s))
            {
                return "e";
            }
            else if ("ìíîïı".Contains(s))
            {
                return "i";
            }
            else if ("òóôõöøőð".Contains(s))
            {
                return "o";
            }
            else if ("ùúûüŭů".Contains(s))
            {
                return "u";
            }
            else if ("çćčĉ".Contains(s))
            {
                return "c";
            }
            else if ("żźž".Contains(s))
            {
                return "z";
            }
            else if ("śşšŝ".Contains(s))
            {
                return "s";
            }
            else if ("ñń".Contains(s))
            {
                return "n";
            }
            else if ("ýÿ".Contains(s))
            {
                return "y";
            }
            else if ("ğĝ".Contains(s))
            {
                return "g";
            }
            else if (c == 'ř')
            {
                return "r";
            }
            else if (c == 'ł')
            {
                return "l";
            }
            else if (c == 'đ')
            {
                return "d";
            }
            else if (c == 'ß')
            {
                return "ss";
            }
            else if (c == 'Þ')
            {
                return "th";
            }
            else if (c == 'ĥ')
            {
                return "h";
            }
            else if (c == 'ĵ')
            {
                return "j";
            }
            else
            {
                return "";
            }
        }
        #endregion

        #region  Solution-2

        public static string Create(bool toLower, params string[] values)
        {
            return Create(toLower, String.Join("-", values));
        }

        /// <summary>
        /// Creates a slug.
        /// References:
        /// http://www.unicode.org/reports/tr15/tr15-34.html
        /// http://meta.stackexchange.com/questions/7435/non-us-ascii-characters-dropped-from-full-profile-url/7696#7696
        /// http://stackoverflow.com/questions/25259/how-do-you-include-a-webpage-title-as-part-of-a-webpage-url/25486#25486
        /// http://stackoverflow.com/questions/3769457/how-can-i-remove-accents-on-a-string
        /// </summary>
        /// <param name="toLower"></param>
        /// <param name="normalised"></param>
        /// <returns></returns>
        public static string Create(bool toLower, string value)
        {
            if (value == null)
                return "";

            var normalised = value.Normalize(NormalizationForm.FormKD);

            const int maxlen = 80;
            int len = normalised.Length;
            bool prevDash = false;
            var sb = new StringBuilder(len);
            char c;

            for (int i = 0; i < len; i++)
            {
                c = normalised[i];
                if ((c >= 'a' && c <= 'z') || (c >= '0' && c <= '9'))
                {
                    if (prevDash)
                    {
                        sb.Append('-');
                        prevDash = false;
                    }
                    sb.Append(c);
                }
                else if (c >= 'A' && c <= 'Z')
                {
                    if (prevDash)
                    {
                        sb.Append('-');
                        prevDash = false;
                    }
                    // Tricky way to convert to lowercase
                    if (toLower)
                        sb.Append((char)(c | 32));
                    else
                        sb.Append(c);
                }
                else if (c == ' ' || c == ',' || c == '.' || c == '/' || c == '\\' || c == '-' || c == '_' || c == '=')
                {
                    if (!prevDash && sb.Length > 0)
                    {
                        prevDash = true;
                    }
                }
                else
                {
                    string swap = ConvertEdgeCases(c, toLower);

                    if (swap != null)
                    {
                        if (prevDash)
                        {
                            sb.Append('-');
                            prevDash = false;
                        }
                        sb.Append(swap);
                    }
                }

                if (sb.Length == maxlen)
                    break;
            }
            return sb.ToString();
        }

        static string ConvertEdgeCases(char c, bool toLower)
        {
            string swap = null;
            switch (c)
            {
                case 'ı':
                    swap = "i";
                    break;
                case 'ł':
                    swap = "l";
                    break;
                case 'Ł':
                    swap = toLower ? "l" : "L";
                    break;
                case 'đ':
                    swap = "d";
                    break;
                case 'ß':
                    swap = "ss";
                    break;
                case 'ø':
                    swap = "o";
                    break;
                case 'Þ':
                    swap = "th";
                    break;
            }
            return swap;
        }
        #endregion
    }
    #endregion

    #endregion
}