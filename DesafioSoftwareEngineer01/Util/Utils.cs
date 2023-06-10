using System.Text.RegularExpressions;

namespace DesafioSoftwareEngineer01.Util
{
    public class Utils
    {
        public String formatarStrign(String str)
        {
                string pattern = "[^0-9]";
                string replace = "";
                string stringFinal = Regex.Replace(str, pattern, replace);
                return stringFinal;
        }
    }
}