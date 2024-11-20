using System.Text.RegularExpressions;

namespace Sediin.PraticheRegionali.DOM
{
    public static class Extension
    {
        public static string TrimAll(this object val)
        {
            try
            {
                var _val = val;

                if (_val != null)
                {
                    _val = _val.ToString().Trim();
                    _val = _val.ToString().TrimStart();
                    _val = _val.ToString().TrimEnd();
                    _val = _val.ToString().Replace("  ", " ");
                }

                return _val?.ToString();
            }
            catch
            {
                return val?.ToString();
            }
        }

        public static string RemoveWhiteSpace(this object val)
        {
            try
            {
                var _val = val;

                if (_val != null)
                {
                    return Regex.Replace(_val.ToString(), @"\s+", "");
                }

                return _val?.ToString();
            }
            catch
            {
                return val?.ToString();
            }
        }

    }
}
