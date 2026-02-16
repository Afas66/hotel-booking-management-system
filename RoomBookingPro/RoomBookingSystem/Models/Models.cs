namespace RoomBookingSystem.Models
{
    public class Customer
    {
        public int CustomerID { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string IDNumber { get; set; } = string.Empty;
        public string EmergencyContact { get; set; } = string.Empty;
        public string EmergencyPhone { get; set; } = string.Empty;
        public DateTime? DateOfBirth { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string Occupation { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }
    }

    public class Bed
    {
        public int BedID { get; set; }
        public int AreaID { get; set; }
        public int RoomTypeID { get; set; }
        public string BedNumber { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime? LastMaintenance { get; set; }
        public DateTime CreatedDate { get; set; }
        
        // Navigation properties
        public string AreaName { get; set; } = string.Empty;
        public string RoomTypeName { get; set; } = string.Empty;
    }

    public class Booking
    {
        public int BookingID { get; set; }
        public int CustomerID { get; set; }
        public int BedID { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime? CheckOutDate { get; set; }
        public DateTime BookingDate { get; set; }
        public decimal MonthlyRent { get; set; }
        public decimal DepositAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public int? CreatedBy { get; set; }
        
        // Navigation properties
        public string CustomerName { get; set; } = string.Empty;
        public string BedNumber { get; set; } = string.Empty;
        public string AreaName { get; set; } = string.Empty;
    }

    public class Payment
    {
        public int PaymentID { get; set; }
        public int BookingID { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string PaymentFor { get; set; } = string.Empty;
        public string TransactionReference { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public int? ReceivedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        
        // Navigation properties
        public string CustomerName { get; set; } = string.Empty;
        public string BedNumber { get; set; } = string.Empty;
    }

    public class Area
    {
        public int AreaID { get; set; }
        public string AreaName { get; set; } = string.Empty;
        public string AreaCode { get; set; } = string.Empty;
        public int Floor { get; set; }
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

    public class RoomType
    {
        public int RoomTypeID { get; set; }
        public string TypeName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public decimal BasePrice { get; set; }
        public string Amenities { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

    public class Maintenance
    {
        public int MaintenanceID { get; set; }
        public int BedID { get; set; }
        public string IssueType { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime ReportedDate { get; set; }
        public int? ReportedBy { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public string AssignedTo { get; set; } = string.Empty;
        public DateTime? CompletedDate { get; set; }
        public decimal Cost { get; set; }
        public string Notes { get; set; } = string.Empty;
        
        // Navigation properties
        public string BedNumber { get; set; } = string.Empty;
        public string AreaName { get; set; } = string.Empty;
    }

    public class DashboardStats
    {
        public int AvailableRooms { get; set; }
        public int OccupiedRooms { get; set; }
        public int ActiveBookings { get; set; }
        public int TotalCustomers { get; set; }
        public decimal MonthlyRevenue { get; set; }
        public int PendingMaintenance { get; set; }
    }
}
