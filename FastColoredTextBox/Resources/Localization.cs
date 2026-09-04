using System;
using System.Globalization;
using System.Resources;
using System.Reflection;

namespace FastColoredTextBoxNS
{
    internal static class Localization
    {
        private static ResourceManager _resourceManager;
        private static CultureInfo _currentCulture;

        static Localization()
        {
            _resourceManager = new ResourceManager("FastColoredTextBoxNS.Resources.Strings", typeof(Localization).Assembly);
            _currentCulture = CultureInfo.CurrentUICulture;
        }

        public static CultureInfo CurrentCulture
        {
            get => _currentCulture;
            set
            {
                _currentCulture = value ?? CultureInfo.CurrentUICulture;
            }
        }

        public static string GetString(string name)
        {
            try
            {
                return _resourceManager.GetString(name, _currentCulture) ?? name;
            }
            catch
            {
                return name;
            }
        }

        public static string GetString(string name, params object[] args)
        {
            string format = GetString(name);
            try
            {
                return string.Format(_currentCulture, format, args);
            }
            catch
            {
                return format;
            }
        }
    }
}