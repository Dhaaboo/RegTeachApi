using System.Text.RegularExpressions;

namespace RegTeachApi.Helpers
{
    public static class PasswordValidator
    {
        public static bool IsValid(string password)
        {
            return Regex.IsMatch(
                password,
                @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$");
        }
    }
}
