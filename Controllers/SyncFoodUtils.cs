using System.Text.RegularExpressions;

namespace SyncFoodApi.Controllers
{
    public static class SyncFoodUtils
    {
        public static bool AllowedName(string userName)
        {
            /* Regex regex = new Regex("^[a-zA-Z0-9]*$");
             return regex.IsMatch(userName);*/
            // Désactivé temporairement le temps du dev
            return true;

        }
    }
}
