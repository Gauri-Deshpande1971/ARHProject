using System.Collections.Generic;

namespace API.Extensions
{
    public static class SessionManager
    {
        public static Dictionary<string, string> SessionData {get; set; }

        public static void Add(string key, string value)
        {
            if (SessionData == null) 
            {
                SessionData = new Dictionary<string, string>();
            }

            var v = SessionData.GetValueOrDefault(key);
            if (v != null)
            {
                SessionData.Remove(key);
            }
            SessionData.Add(key, value);
        }
        public static void Delete(string key)
        {
            var v = SessionData.GetValueOrDefault(key);
            if (v != null)
            {
                SessionData.Remove(key);
            }
        }
        public static string Get(string key)
        {
            var v = SessionData.GetValueOrDefault(key);
            if (v != null)
            {
                return SessionData.GetValueOrDefault(key);
            }
            return null;
        }
    }
}