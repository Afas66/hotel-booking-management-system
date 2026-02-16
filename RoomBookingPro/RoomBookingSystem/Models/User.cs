namespace RoomBookingSystem.Models
{
    public class User
    {
        public int UserID { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? LastLogin { get; set; }

        // Static property to track current logged-in user
        public static User? CurrentUser { get; set; }

        // Helper method to check if user is admin
        public static bool IsAdmin() => CurrentUser?.Role == "Admin";
        
        // Helper method to check if user is manager
        public static bool IsManager() => CurrentUser?.Role == "Manager";
        
        // Helper method to check if user is logged in
        public static bool IsLoggedIn() => CurrentUser != null;
    }
}
