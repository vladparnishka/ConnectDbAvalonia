using ConnectDbAvalonia.Models;

namespace ConnectDbAvalonia.Services
{
    public static class Session
    {
        public static User? CurrentUser { get; set; }

        public static bool IsGuest => CurrentUser == null;

        public static bool IsStaff => CurrentUser?.Role?.RoleName == "Менеджер" || CurrentUser?.Role?.RoleName == "Администратор";

        public static bool IsAdmin => CurrentUser?.Role?.RoleName == "Администратор";
    }
}