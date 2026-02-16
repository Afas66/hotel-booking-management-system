-- =====================================================
-- ROOM BOOKING MANAGEMENT SYSTEM DATABASE SCHEMA
-- Compatible with MySQL 8.0.38
-- =====================================================

-- Drop database if exists and create fresh
DROP DATABASE IF EXISTS RoomBookingDB;
CREATE DATABASE RoomBookingDB CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE RoomBookingDB;

-- =====================================================
-- TABLE: Users (for authentication and authorization)
-- =====================================================
CREATE TABLE Users (
    UserID INT AUTO_INCREMENT PRIMARY KEY,
    Username VARCHAR(50) UNIQUE NOT NULL,
    Password VARCHAR(255) NOT NULL,
    FullName VARCHAR(100) NOT NULL,
    Email VARCHAR(100),
    Phone VARCHAR(20),
    Role ENUM('Admin', 'Manager', 'Staff') DEFAULT 'Staff',
    IsActive BOOLEAN DEFAULT TRUE,
    CreatedDate DATETIME DEFAULT CURRENT_TIMESTAMP,
    LastLogin DATETIME NULL,
    INDEX idx_username (Username),
    INDEX idx_role (Role)
) ENGINE=InnoDB;

-- =====================================================
-- TABLE: Areas (Room locations/buildings)
-- =====================================================
CREATE TABLE Areas (
    AreaID INT AUTO_INCREMENT PRIMARY KEY,
    AreaName VARCHAR(50) UNIQUE NOT NULL,
    AreaCode VARCHAR(10) UNIQUE NOT NULL,
    Floor INT NOT NULL,
    Description TEXT,
    IsActive BOOLEAN DEFAULT TRUE,
    INDEX idx_area_code (AreaCode)
) ENGINE=InnoDB;

-- =====================================================
-- TABLE: RoomTypes (Different categories of rooms)
-- =====================================================
CREATE TABLE RoomTypes (
    RoomTypeID INT AUTO_INCREMENT PRIMARY KEY,
    TypeName VARCHAR(50) UNIQUE NOT NULL,
    Description TEXT,
    Capacity INT NOT NULL,
    BasePrice DECIMAL(10,2) NOT NULL,
    Amenities TEXT,
    IsActive BOOLEAN DEFAULT TRUE
) ENGINE=InnoDB;

-- =====================================================
-- TABLE: Rooms/Beds
-- =====================================================
CREATE TABLE Beds (
    BedID INT AUTO_INCREMENT PRIMARY KEY,
    AreaID INT NOT NULL,
    RoomTypeID INT NOT NULL,
    BedNumber VARCHAR(20) UNIQUE NOT NULL,
    Price DECIMAL(10,2) NOT NULL,
    Status ENUM('Available', 'Occupied', 'Maintenance', 'Reserved') DEFAULT 'Available',
    Description TEXT,
    LastMaintenance DATE,
    CreatedDate DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (AreaID) REFERENCES Areas(AreaID) ON DELETE RESTRICT,
    FOREIGN KEY (RoomTypeID) REFERENCES RoomTypes(RoomTypeID) ON DELETE RESTRICT,
    INDEX idx_status (Status),
    INDEX idx_area (AreaID),
    INDEX idx_bed_number (BedNumber)
) ENGINE=InnoDB;

-- =====================================================
-- TABLE: Customers
-- =====================================================
CREATE TABLE Customers (
    CustomerID INT AUTO_INCREMENT PRIMARY KEY,
    FullName VARCHAR(100) NOT NULL,
    Email VARCHAR(100) UNIQUE,
    Phone VARCHAR(20) NOT NULL,
    Address TEXT,
    IDNumber VARCHAR(50),
    EmergencyContact VARCHAR(100),
    EmergencyPhone VARCHAR(20),
    DateOfBirth DATE,
    Gender ENUM('Male', 'Female', 'Other'),
    Occupation VARCHAR(100),
    CreatedDate DATETIME DEFAULT CURRENT_TIMESTAMP,
    IsActive BOOLEAN DEFAULT TRUE,
    INDEX idx_phone (Phone),
    INDEX idx_email (Email),
    INDEX idx_fullname (FullName)
) ENGINE=InnoDB;

-- =====================================================
-- TABLE: Bookings
-- =====================================================
CREATE TABLE Bookings (
    BookingID INT AUTO_INCREMENT PRIMARY KEY,
    CustomerID INT NOT NULL,
    BedID INT NOT NULL,
    CheckInDate DATE NOT NULL,
    CheckOutDate DATE NULL,
    BookingDate DATETIME DEFAULT CURRENT_TIMESTAMP,
    MonthlyRent DECIMAL(10,2) NOT NULL,
    DepositAmount DECIMAL(10,2) DEFAULT 0.00,
    Status ENUM('Pending', 'Active', 'Completed', 'Cancelled') DEFAULT 'Pending',
    Notes TEXT,
    CreatedBy INT,
    FOREIGN KEY (CustomerID) REFERENCES Customers(CustomerID) ON DELETE RESTRICT,
    FOREIGN KEY (BedID) REFERENCES Beds(BedID) ON DELETE RESTRICT,
    FOREIGN KEY (CreatedBy) REFERENCES Users(UserID) ON DELETE SET NULL,
    INDEX idx_customer (CustomerID),
    INDEX idx_bed (BedID),
    INDEX idx_status (Status),
    INDEX idx_checkin (CheckInDate)
) ENGINE=InnoDB;

-- =====================================================
-- TABLE: Payments
-- =====================================================
CREATE TABLE Payments (
    PaymentID INT AUTO_INCREMENT PRIMARY KEY,
    BookingID INT NOT NULL,
    PaymentDate DATE NOT NULL,
    Amount DECIMAL(10,2) NOT NULL,
    PaymentMethod ENUM('Cash', 'Bank Transfer', 'Credit Card', 'Debit Card', 'Mobile Payment') DEFAULT 'Cash',
    PaymentFor ENUM('Deposit', 'Monthly Rent', 'Utilities', 'Maintenance', 'Other') DEFAULT 'Monthly Rent',
    TransactionReference VARCHAR(100),
    Notes TEXT,
    ReceivedBy INT,
    CreatedDate DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (BookingID) REFERENCES Bookings(BookingID) ON DELETE RESTRICT,
    FOREIGN KEY (ReceivedBy) REFERENCES Users(UserID) ON DELETE SET NULL,
    INDEX idx_booking (BookingID),
    INDEX idx_payment_date (PaymentDate)
) ENGINE=InnoDB;

-- =====================================================
-- TABLE: Maintenance
-- =====================================================
CREATE TABLE Maintenance (
    MaintenanceID INT AUTO_INCREMENT PRIMARY KEY,
    BedID INT NOT NULL,
    IssueType VARCHAR(100) NOT NULL,
    Description TEXT NOT NULL,
    ReportedDate DATETIME DEFAULT CURRENT_TIMESTAMP,
    ReportedBy INT,
    Status ENUM('Pending', 'In Progress', 'Completed', 'Cancelled') DEFAULT 'Pending',
    Priority ENUM('Low', 'Medium', 'High', 'Critical') DEFAULT 'Medium',
    AssignedTo VARCHAR(100),
    CompletedDate DATETIME NULL,
    Cost DECIMAL(10,2) DEFAULT 0.00,
    Notes TEXT,
    FOREIGN KEY (BedID) REFERENCES Beds(BedID) ON DELETE RESTRICT,
    FOREIGN KEY (ReportedBy) REFERENCES Users(UserID) ON DELETE SET NULL,
    INDEX idx_bed (BedID),
    INDEX idx_status (Status),
    INDEX idx_priority (Priority)
) ENGINE=InnoDB;

-- =====================================================
-- TABLE: AuditLog (Track all important actions)
-- =====================================================
CREATE TABLE AuditLog (
    LogID INT AUTO_INCREMENT PRIMARY KEY,
    UserID INT,
    Action VARCHAR(100) NOT NULL,
    TableName VARCHAR(50),
    RecordID INT,
    OldValue TEXT,
    NewValue TEXT,
    IPAddress VARCHAR(45),
    Timestamp DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE SET NULL,
    INDEX idx_user (UserID),
    INDEX idx_timestamp (Timestamp)
) ENGINE=InnoDB;

-- =====================================================
-- INSERT SAMPLE DATA
-- =====================================================

-- Users (Password: 'admin123' for all)
INSERT INTO Users (Username, Password, FullName, Email, Phone, Role, IsActive) VALUES
('admin', 'admin123', 'System Administrator', 'admin@roombooking.com', '+1234567890', 'Admin', TRUE),
('manager1', 'admin123', 'John Manager', 'john.manager@roombooking.com', '+1234567891', 'Manager', TRUE),
('staff1', 'admin123', 'Sarah Staff', 'sarah.staff@roombooking.com', '+1234567892', 'Staff', TRUE),
('staff2', 'admin123', 'Mike Johnson', 'mike.johnson@roombooking.com', '+1234567893', 'Staff', TRUE);

-- Areas
INSERT INTO Areas (AreaName, AreaCode, Floor, Description, IsActive) VALUES
('North Wing', 'NW', 1, 'Main building north wing - Ground floor', TRUE),
('South Wing', 'SW', 1, 'Main building south wing - Ground floor', TRUE),
('East Block', 'EB', 2, 'East building - Second floor', TRUE),
('West Block', 'WB', 2, 'West building - Second floor', TRUE),
('Premium Tower', 'PT', 3, 'Premium tower - Third floor with city view', TRUE);

-- Room Types
INSERT INTO RoomTypes (TypeName, Description, Capacity, BasePrice, Amenities, IsActive) VALUES
('Single Room', 'Single occupancy room with basic amenities', 1, 500.00, 'Bed, Desk, Chair, Wardrobe, WiFi', TRUE),
('Double Room', 'Double occupancy room with shared facilities', 2, 800.00, 'Beds, Desks, Chairs, Wardrobes, WiFi, Shared Bathroom', TRUE),
('Deluxe Single', 'Premium single room with private bathroom', 1, 750.00, 'Bed, Desk, Chair, Wardrobe, WiFi, Private Bathroom, AC', TRUE),
('Suite', 'Spacious suite with living area', 2, 1200.00, 'Beds, Living Area, Kitchen, Private Bathroom, AC, WiFi, TV', TRUE),
('Studio', 'Self-contained studio apartment', 1, 950.00, 'Bed, Kitchenette, Private Bathroom, AC, WiFi, Work Desk', TRUE);

-- Beds/Rooms (50 rooms across different areas)
INSERT INTO Beds (AreaID, RoomTypeID, BedNumber, Price, Status, Description, LastMaintenance) VALUES
-- North Wing (10 rooms)
(1, 1, 'NW-101', 500.00, 'Available', 'Ground floor single room', '2024-01-15'),
(1, 1, 'NW-102', 500.00, 'Occupied', 'Ground floor single room', '2024-01-15'),
(1, 2, 'NW-103', 800.00, 'Available', 'Ground floor double room', '2024-01-20'),
(1, 2, 'NW-104', 800.00, 'Occupied', 'Ground floor double room', '2024-01-20'),
(1, 3, 'NW-105', 750.00, 'Available', 'Deluxe single with garden view', '2024-02-01'),
(1, 1, 'NW-106', 500.00, 'Maintenance', 'Under renovation', '2024-02-10'),
(1, 2, 'NW-107', 800.00, 'Available', 'Corner double room', '2024-01-25'),
(1, 3, 'NW-108', 750.00, 'Reserved', 'Reserved for VIP', '2024-02-05'),
(1, 1, 'NW-109', 500.00, 'Available', 'Single room near entrance', '2024-01-15'),
(1, 2, 'NW-110', 800.00, 'Occupied', 'Double room with balcony', '2024-01-30'),

-- South Wing (10 rooms)
(2, 1, 'SW-201', 500.00, 'Available', 'Quiet single room', '2024-01-15'),
(2, 1, 'SW-202', 500.00, 'Occupied', 'Single room', '2024-01-15'),
(2, 2, 'SW-203', 800.00, 'Available', 'Double room', '2024-01-20'),
(2, 3, 'SW-204', 750.00, 'Available', 'Deluxe single', '2024-02-01'),
(2, 1, 'SW-205', 500.00, 'Occupied', 'Single room', '2024-01-15'),
(2, 2, 'SW-206', 800.00, 'Available', 'Spacious double', '2024-01-25'),
(2, 3, 'SW-207', 750.00, 'Occupied', 'Deluxe with bathroom', '2024-02-05'),
(2, 1, 'SW-208', 500.00, 'Available', 'Single room', '2024-01-15'),
(2, 2, 'SW-209', 800.00, 'Available', 'Double room', '2024-01-30'),
(2, 3, 'SW-210', 750.00, 'Available', 'Deluxe corner room', '2024-02-01'),

-- East Block (10 rooms)
(3, 3, 'EB-301', 750.00, 'Available', 'Deluxe with city view', '2024-02-01'),
(3, 4, 'EB-302', 1200.00, 'Occupied', 'Premium suite', '2024-02-10'),
(3, 3, 'EB-303', 750.00, 'Available', 'Deluxe single', '2024-02-01'),
(3, 4, 'EB-304', 1200.00, 'Available', 'Executive suite', '2024-02-10'),
(3, 5, 'EB-305', 950.00, 'Occupied', 'Modern studio', '2024-02-15'),
(3, 3, 'EB-306', 750.00, 'Available', 'Deluxe room', '2024-02-01'),
(3, 4, 'EB-307', 1200.00, 'Reserved', 'VIP suite', '2024-02-10'),
(3, 5, 'EB-308', 950.00, 'Available', 'Studio apartment', '2024-02-15'),
(3, 3, 'EB-309', 750.00, 'Occupied', 'Deluxe with balcony', '2024-02-01'),
(3, 4, 'EB-310', 1200.00, 'Available', 'Luxury suite', '2024-02-10'),

-- West Block (10 rooms)
(4, 5, 'WB-401', 950.00, 'Available', 'Studio with kitchen', '2024-02-15'),
(4, 3, 'WB-402', 750.00, 'Occupied', 'Deluxe single', '2024-02-01'),
(4, 5, 'WB-403', 950.00, 'Available', 'Modern studio', '2024-02-15'),
(4, 4, 'WB-404', 1200.00, 'Available', 'Family suite', '2024-02-10'),
(4, 5, 'WB-405', 950.00, 'Occupied', 'Studio apartment', '2024-02-15'),
(4, 3, 'WB-406', 750.00, 'Available', 'Deluxe room', '2024-02-01'),
(4, 5, 'WB-407', 950.00, 'Available', 'Studio with workspace', '2024-02-15'),
(4, 4, 'WB-408', 1200.00, 'Occupied', 'Premium suite', '2024-02-10'),
(4, 5, 'WB-409', 950.00, 'Available', 'Studio apartment', '2024-02-15'),
(4, 3, 'WB-410', 750.00, 'Available', 'Deluxe single', '2024-02-01'),

-- Premium Tower (10 rooms)
(5, 4, 'PT-501', 1200.00, 'Available', 'Penthouse suite', '2024-02-20'),
(5, 5, 'PT-502', 950.00, 'Occupied', 'Premium studio', '2024-02-20'),
(5, 4, 'PT-503', 1200.00, 'Available', 'Executive suite with view', '2024-02-20'),
(5, 5, 'PT-504', 950.00, 'Available', 'Modern studio', '2024-02-20'),
(5, 4, 'PT-505', 1200.00, 'Occupied', 'Luxury suite', '2024-02-20'),
(5, 5, 'PT-506', 950.00, 'Available', 'Studio with balcony', '2024-02-20'),
(5, 4, 'PT-507', 1200.00, 'Reserved', 'VIP penthouse', '2024-02-20'),
(5, 5, 'PT-508', 950.00, 'Available', 'Premium studio', '2024-02-20'),
(5, 4, 'PT-509', 1200.00, 'Available', 'Executive suite', '2024-02-20'),
(5, 4, 'PT-510', 1200.00, 'Occupied', 'Luxury penthouse', '2024-02-20');

-- Customers (30 customers)
INSERT INTO Customers (FullName, Email, Phone, Address, IDNumber, EmergencyContact, EmergencyPhone, DateOfBirth, Gender, Occupation, IsActive) VALUES
('James Anderson', 'james.anderson@email.com', '+1234567001', '123 Main St, City', 'ID001', 'Mary Anderson', '+1234567002', '1995-03-15', 'Male', 'Software Engineer', TRUE),
('Emily Roberts', 'emily.roberts@email.com', '+1234567003', '456 Oak Ave, City', 'ID002', 'Tom Roberts', '+1234567004', '1998-07-22', 'Female', 'Teacher', TRUE),
('Michael Brown', 'michael.brown@email.com', '+1234567005', '789 Pine Rd, City', 'ID003', 'Lisa Brown', '+1234567006', '1993-11-08', 'Male', 'Accountant', TRUE),
('Sarah Davis', 'sarah.davis@email.com', '+1234567007', '321 Elm St, City', 'ID004', 'David Davis', '+1234567008', '1997-05-30', 'Female', 'Nurse', TRUE),
('Robert Wilson', 'robert.wilson@email.com', '+1234567009', '654 Maple Dr, City', 'ID005', 'Jennifer Wilson', '+1234567010', '1990-09-12', 'Male', 'Business Analyst', TRUE),
('Jessica Taylor', 'jessica.taylor@email.com', '+1234567011', '987 Cedar Ln, City', 'ID006', 'Mark Taylor', '+1234567012', '1996-02-18', 'Female', 'Marketing Manager', TRUE),
('David Martinez', 'david.martinez@email.com', '+1234567013', '147 Birch Ct, City', 'ID007', 'Maria Martinez', '+1234567014', '1994-08-25', 'Male', 'Graphic Designer', TRUE),
('Linda Garcia', 'linda.garcia@email.com', '+1234567015', '258 Spruce Way, City', 'ID008', 'Carlos Garcia', '+1234567016', '1992-12-03', 'Female', 'HR Manager', TRUE),
('Christopher Lee', 'chris.lee@email.com', '+1234567017', '369 Ash Blvd, City', 'ID009', 'Michelle Lee', '+1234567018', '1999-04-14', 'Male', 'Student', TRUE),
('Amanda White', 'amanda.white@email.com', '+1234567019', '741 Walnut St, City', 'ID010', 'Steven White', '+1234567020', '1991-10-27', 'Female', 'Sales Executive', TRUE),
('Daniel Harris', 'daniel.harris@email.com', '+1234567021', '852 Cherry Ave, City', 'ID011', 'Nancy Harris', '+1234567022', '1995-06-09', 'Male', 'Data Analyst', TRUE),
('Michelle Clark', 'michelle.clark@email.com', '+1234567023', '963 Poplar Rd, City', 'ID012', 'Richard Clark', '+1234567024', '1998-01-21', 'Female', 'Pharmacist', TRUE),
('Kevin Rodriguez', 'kevin.rodriguez@email.com', '+1234567025', '159 Hickory Dr, City', 'ID013', 'Patricia Rodriguez', '+1234567026', '1993-07-16', 'Male', 'Civil Engineer', TRUE),
('Karen Lewis', 'karen.lewis@email.com', '+1234567027', '357 Magnolia Ln, City', 'ID014', 'Donald Lewis', '+1234567028', '1997-03-04', 'Female', 'Financial Advisor', TRUE),
('Brian Walker', 'brian.walker@email.com', '+1234567029', '486 Dogwood Ct, City', 'ID015', 'Susan Walker', '+1234567030', '1990-11-19', 'Male', 'Architect', TRUE),
('Nicole Hall', 'nicole.hall@email.com', '+1234567031', '624 Sycamore Way, City', 'ID016', 'George Hall', '+1234567032', '1996-09-07', 'Female', 'Lawyer', TRUE),
('Jason Allen', 'jason.allen@email.com', '+1234567033', '735 Beech Blvd, City', 'ID017', 'Barbara Allen', '+1234567034', '1994-05-23', 'Male', 'Mechanical Engineer', TRUE),
('Stephanie Young', 'stephanie.young@email.com', '+1234567035', '846 Willow St, City', 'ID018', 'Paul Young', '+1234567036', '1992-02-11', 'Female', 'Dentist', TRUE),
('Andrew King', 'andrew.king@email.com', '+1234567037', '957 Fir Ave, City', 'ID019', 'Helen King', '+1234567038', '1999-08-29', 'Male', 'Photographer', TRUE),
('Rebecca Wright', 'rebecca.wright@email.com', '+1234567039', '168 Palm Rd, City', 'ID020', 'Kenneth Wright', '+1234567040', '1991-04-06', 'Female', 'Journalist', TRUE),
('Timothy Lopez', 'timothy.lopez@email.com', '+1234567041', '279 Redwood Dr, City', 'ID021', 'Dorothy Lopez', '+1234567042', '1995-12-14', 'Male', 'Chef', TRUE),
('Angela Hill', 'angela.hill@email.com', '+1234567043', '381 Sequoia Ln, City', 'ID022', 'Larry Hill', '+1234567044', '1998-10-01', 'Female', 'Interior Designer', TRUE),
('Patrick Scott', 'patrick.scott@email.com', '+1234567045', '492 Cypress Ct, City', 'ID023', 'Sandra Scott', '+1234567046', '1993-06-18', 'Male', 'Real Estate Agent', TRUE),
('Melissa Green', 'melissa.green@email.com', '+1234567047', '513 Juniper Way, City', 'ID024', 'Raymond Green', '+1234567048', '1997-01-25', 'Female', 'Psychologist', TRUE),
('Gregory Adams', 'gregory.adams@email.com', '+1234567049', '624 Cedar Blvd, City', 'ID025', 'Sharon Adams', '+1234567050', '1990-09-02', 'Male', 'IT Consultant', TRUE),
('Deborah Baker', 'deborah.baker@email.com', '+1234567051', '735 Pine St, City', 'ID026', 'Gary Baker', '+1234567052', '1996-05-13', 'Female', 'Veterinarian', TRUE),
('Ronald Nelson', 'ronald.nelson@email.com', '+1234567053', '846 Oak Ave, City', 'ID027', 'Carol Nelson', '+1234567054', '1994-11-28', 'Male', 'Electrician', TRUE),
('Cynthia Carter', 'cynthia.carter@email.com', '+1234567055', '957 Elm Rd, City', 'ID028', 'Jeffrey Carter', '+1234567056', '1992-07-05', 'Female', 'Pharmacist', TRUE),
('Steven Mitchell', 'steven.mitchell@email.com', '+1234567057', '168 Maple Dr, City', 'ID029', 'Betty Mitchell', '+1234567058', '1999-03-20', 'Male', 'Plumber', TRUE),
('Donna Perez', 'donna.perez@email.com', '+1234567059', '279 Birch Ln, City', 'ID030', 'Frank Perez', '+1234567060', '1991-12-08', 'Female', 'Scientist', TRUE);

-- Bookings (20 active bookings)
INSERT INTO Bookings (CustomerID, BedID, CheckInDate, CheckOutDate, MonthlyRent, DepositAmount, Status, Notes, CreatedBy) VALUES
(1, 2, '2024-01-01', NULL, 500.00, 500.00, 'Active', 'Long-term tenant', 1),
(2, 4, '2024-01-05', NULL, 800.00, 800.00, 'Active', 'Student accommodation', 1),
(3, 10, '2024-01-10', NULL, 800.00, 800.00, 'Active', 'Working professional', 2),
(4, 12, '2024-01-15', NULL, 500.00, 500.00, 'Active', 'Quiet tenant', 2),
(5, 15, '2024-01-20', NULL, 500.00, 500.00, 'Active', 'Early morning worker', 1),
(6, 17, '2024-01-25', NULL, 750.00, 750.00, 'Active', 'Prefers deluxe', 1),
(7, 22, '2024-02-01', NULL, 1200.00, 1200.00, 'Active', 'VIP customer', 1),
(8, 25, '2024-02-05', NULL, 950.00, 950.00, 'Active', 'Business traveler', 2),
(9, 29, '2024-02-10', NULL, 750.00, 750.00, 'Active', 'Extended stay', 1),
(10, 32, '2024-02-15', NULL, 750.00, 750.00, 'Active', 'Monthly payment', 2),
(11, 38, '2024-02-20', NULL, 1200.00, 1200.00, 'Active', 'Executive suite', 1),
(12, 42, '2024-02-25', NULL, 950.00, 950.00, 'Active', 'Studio preference', 1),
(13, 45, '2024-03-01', NULL, 950.00, 950.00, 'Active', 'Remote worker', 2),
(14, 50, '2024-03-05', NULL, 1200.00, 1200.00, 'Active', 'Premium customer', 1),
(15, 3, '2024-01-01', '2024-06-30', 800.00, 800.00, 'Pending', 'Future booking', 1),
(16, 5, '2024-03-15', NULL, 750.00, 750.00, 'Pending', 'Waiting for approval', 2),
(17, 21, '2023-06-01', '2023-12-31', 750.00, 750.00, 'Completed', 'Contract ended', 1),
(18, 23, '2023-07-01', '2024-01-15', 1200.00, 1200.00, 'Completed', 'Moved out', 1),
(19, 26, '2023-08-01', '2024-02-01', 750.00, 750.00, 'Completed', 'Lease completed', 2),
(20, 31, '2024-02-01', '2024-02-05', 750.00, 0.00, 'Cancelled', 'Customer cancelled', 1);

-- Payments (40 payment records)
INSERT INTO Payments (BookingID, PaymentDate, Amount, PaymentMethod, PaymentFor, TransactionReference, ReceivedBy) VALUES
-- Booking 1 payments
(1, '2024-01-01', 500.00, 'Cash', 'Deposit', 'DEP-001', 1),
(1, '2024-01-01', 500.00, 'Cash', 'Monthly Rent', 'RENT-001-JAN', 1),
(1, '2024-02-01', 500.00, 'Bank Transfer', 'Monthly Rent', 'RENT-001-FEB', 1),

-- Booking 2 payments
(2, '2024-01-05', 800.00, 'Credit Card', 'Deposit', 'DEP-002', 1),
(2, '2024-01-05', 800.00, 'Credit Card', 'Monthly Rent', 'RENT-002-JAN', 1),
(2, '2024-02-05', 800.00, 'Bank Transfer', 'Monthly Rent', 'RENT-002-FEB', 1),

-- Booking 3 payments
(3, '2024-01-10', 800.00, 'Cash', 'Deposit', 'DEP-003', 2),
(3, '2024-01-10', 800.00, 'Cash', 'Monthly Rent', 'RENT-003-JAN', 2),
(3, '2024-02-10', 800.00, 'Mobile Payment', 'Monthly Rent', 'RENT-003-FEB', 2),

-- Booking 4 payments
(4, '2024-01-15', 500.00, 'Cash', 'Deposit', 'DEP-004', 2),
(4, '2024-01-15', 500.00, 'Cash', 'Monthly Rent', 'RENT-004-JAN', 2),
(4, '2024-02-15', 500.00, 'Bank Transfer', 'Monthly Rent', 'RENT-004-FEB', 2),

-- Booking 5 payments
(5, '2024-01-20', 500.00, 'Debit Card', 'Deposit', 'DEP-005', 1),
(5, '2024-01-20', 500.00, 'Debit Card', 'Monthly Rent', 'RENT-005-JAN', 1),
(5, '2024-02-20', 500.00, 'Debit Card', 'Monthly Rent', 'RENT-005-FEB', 1),

-- Booking 6 payments
(6, '2024-01-25', 750.00, 'Cash', 'Deposit', 'DEP-006', 1),
(6, '2024-01-25', 750.00, 'Cash', 'Monthly Rent', 'RENT-006-JAN', 1),
(6, '2024-02-25', 750.00, 'Bank Transfer', 'Monthly Rent', 'RENT-006-FEB', 1),

-- Booking 7 payments
(7, '2024-02-01', 1200.00, 'Bank Transfer', 'Deposit', 'DEP-007', 1),
(7, '2024-02-01', 1200.00, 'Bank Transfer', 'Monthly Rent', 'RENT-007-FEB', 1),

-- Booking 8 payments
(8, '2024-02-05', 950.00, 'Credit Card', 'Deposit', 'DEP-008', 2),
(8, '2024-02-05', 950.00, 'Credit Card', 'Monthly Rent', 'RENT-008-FEB', 2),

-- Booking 9 payments
(9, '2024-02-10', 750.00, 'Cash', 'Deposit', 'DEP-009', 1),
(9, '2024-02-10', 750.00, 'Cash', 'Monthly Rent', 'RENT-009-FEB', 1),

-- Booking 10 payments
(10, '2024-02-15', 750.00, 'Mobile Payment', 'Deposit', 'DEP-010', 2),
(10, '2024-02-15', 750.00, 'Mobile Payment', 'Monthly Rent', 'RENT-010-FEB', 2),

-- Additional utility and maintenance payments
(1, '2024-01-31', 50.00, 'Cash', 'Utilities', 'UTIL-001-JAN', 1),
(2, '2024-01-31', 60.00, 'Cash', 'Utilities', 'UTIL-002-JAN', 1),
(3, '2024-01-31', 60.00, 'Cash', 'Utilities', 'UTIL-003-JAN', 2),
(4, '2024-01-31', 50.00, 'Cash', 'Utilities', 'UTIL-004-JAN', 2),
(1, '2024-02-05', 100.00, 'Cash', 'Maintenance', 'MAINT-001', 1),
(3, '2024-02-10', 75.00, 'Cash', 'Maintenance', 'MAINT-003', 2),
(11, '2024-02-20', 1200.00, 'Bank Transfer', 'Deposit', 'DEP-011', 1),
(11, '2024-02-20', 1200.00, 'Bank Transfer', 'Monthly Rent', 'RENT-011-FEB', 1),
(12, '2024-02-25', 950.00, 'Credit Card', 'Deposit', 'DEP-012', 1),
(12, '2024-02-25', 950.00, 'Credit Card', 'Monthly Rent', 'RENT-012-FEB', 1),
(13, '2024-03-01', 950.00, 'Cash', 'Deposit', 'DEP-013', 2),
(13, '2024-03-01', 950.00, 'Cash', 'Monthly Rent', 'RENT-013-MAR', 2),
(14, '2024-03-05', 1200.00, 'Bank Transfer', 'Deposit', 'DEP-014', 1),
(14, '2024-03-05', 1200.00, 'Bank Transfer', 'Monthly Rent', 'RENT-014-MAR', 1);

-- Maintenance Records
INSERT INTO Maintenance (BedID, IssueType, Description, ReportedBy, Status, Priority, AssignedTo, CompletedDate, Cost) VALUES
(6, 'Plumbing', 'Leaking faucet in bathroom', 1, 'Completed', 'Medium', 'John Plumber', '2024-02-11', 75.00),
(2, 'Electrical', 'Light fixture not working', 1, 'Completed', 'High', 'Mike Electrician', '2024-01-17', 50.00),
(10, 'Furniture', 'Broken chair needs replacement', 2, 'Completed', 'Low', 'Furniture Team', '2024-02-02', 120.00),
(15, 'AC/Heating', 'Air conditioning not cooling properly', 1, 'In Progress', 'High', 'HVAC Specialist', NULL, 0.00),
(22, 'Painting', 'Walls need repainting', 1, 'Pending', 'Low', NULL, NULL, 0.00),
(25, 'Plumbing', 'Shower drain clogged', 2, 'In Progress', 'Medium', 'John Plumber', NULL, 0.00),
(29, 'Electrical', 'Power outlet not working', 1, 'Completed', 'High', 'Mike Electrician', '2024-02-13', 40.00),
(32, 'General', 'Door lock needs repair', 2, 'Completed', 'Medium', 'Maintenance Team', '2024-02-18', 65.00),
(38, 'Cleaning', 'Deep cleaning required', 1, 'Completed', 'Low', 'Cleaning Service', '2024-02-22', 100.00),
(42, 'AC/Heating', 'Heater making noise', 2, 'Pending', 'Medium', 'HVAC Specialist', NULL, 0.00);

-- =====================================================
-- CREATE VIEWS FOR REPORTING
-- =====================================================

-- View: Current Occupancy Status
CREATE VIEW vw_OccupancyStatus AS
SELECT 
    a.AreaName,
    rt.TypeName,
    COUNT(CASE WHEN b.Status = 'Available' THEN 1 END) AS Available,
    COUNT(CASE WHEN b.Status = 'Occupied' THEN 1 END) AS Occupied,
    COUNT(CASE WHEN b.Status = 'Maintenance' THEN 1 END) AS Maintenance,
    COUNT(CASE WHEN b.Status = 'Reserved' THEN 1 END) AS Reserved,
    COUNT(*) AS Total
FROM Beds b
JOIN Areas a ON b.AreaID = a.AreaID
JOIN RoomTypes rt ON b.RoomTypeID = rt.RoomTypeID
GROUP BY a.AreaName, rt.TypeName;

-- View: Monthly Revenue Report
CREATE VIEW vw_MonthlyRevenue AS
SELECT 
    DATE_FORMAT(PaymentDate, '%Y-%m') AS Month,
    SUM(CASE WHEN PaymentFor = 'Monthly Rent' THEN Amount ELSE 0 END) AS RentRevenue,
    SUM(CASE WHEN PaymentFor = 'Deposit' THEN Amount ELSE 0 END) AS DepositRevenue,
    SUM(CASE WHEN PaymentFor = 'Utilities' THEN Amount ELSE 0 END) AS UtilityRevenue,
    SUM(Amount) AS TotalRevenue,
    COUNT(DISTINCT BookingID) AS TotalTransactions
FROM Payments
GROUP BY DATE_FORMAT(PaymentDate, '%Y-%m')
ORDER BY Month DESC;

-- View: Active Bookings with Customer Details
CREATE VIEW vw_ActiveBookings AS
SELECT 
    bk.BookingID,
    c.FullName AS CustomerName,
    c.Phone AS CustomerPhone,
    c.Email AS CustomerEmail,
    bd.BedNumber,
    a.AreaName,
    rt.TypeName AS RoomType,
    bk.CheckInDate,
    bk.MonthlyRent,
    bk.Status,
    DATEDIFF(CURDATE(), bk.CheckInDate) AS DaysOccupied,
    u.FullName AS ManagedBy
FROM Bookings bk
JOIN Customers c ON bk.CustomerID = c.CustomerID
JOIN Beds bd ON bk.BedID = bd.BedID
JOIN Areas a ON bd.AreaID = a.AreaID
JOIN RoomTypes rt ON bd.RoomTypeID = rt.RoomTypeID
LEFT JOIN Users u ON bk.CreatedBy = u.UserID
WHERE bk.Status = 'Active';

-- View: Pending Maintenance
CREATE VIEW vw_PendingMaintenance AS
SELECT 
    m.MaintenanceID,
    bd.BedNumber,
    a.AreaName,
    m.IssueType,
    m.Description,
    m.Priority,
    m.Status,
    m.ReportedDate,
    u.FullName AS ReportedBy,
    m.AssignedTo
FROM Maintenance m
JOIN Beds bd ON m.BedID = bd.BedID
JOIN Areas a ON bd.AreaID = a.AreaID
LEFT JOIN Users u ON m.ReportedBy = u.UserID
WHERE m.Status IN ('Pending', 'In Progress')
ORDER BY 
    CASE m.Priority 
        WHEN 'Critical' THEN 1 
        WHEN 'High' THEN 2 
        WHEN 'Medium' THEN 3 
        ELSE 4 
    END,
    m.ReportedDate;

-- =====================================================
-- STORED PROCEDURES
-- =====================================================

DELIMITER //

-- Procedure: Get Available Beds
CREATE PROCEDURE sp_GetAvailableBeds()
BEGIN
    SELECT 
        b.BedID,
        b.BedNumber,
        a.AreaName,
        rt.TypeName,
        b.Price,
        rt.Amenities
    FROM Beds b
    JOIN Areas a ON b.AreaID = a.AreaID
    JOIN RoomTypes rt ON b.RoomTypeID = rt.RoomTypeID
    WHERE b.Status = 'Available'
    ORDER BY a.AreaName, b.BedNumber;
END //

-- Procedure: Get Customer Payment History
CREATE PROCEDURE sp_GetCustomerPayments(IN p_CustomerID INT)
BEGIN
    SELECT 
        p.PaymentDate,
        p.Amount,
        p.PaymentMethod,
        p.PaymentFor,
        p.TransactionReference,
        bd.BedNumber,
        u.FullName AS ReceivedBy
    FROM Payments p
    JOIN Bookings bk ON p.BookingID = bk.BookingID
    JOIN Beds bd ON bk.BedID = bd.BedID
    LEFT JOIN Users u ON p.ReceivedBy = u.UserID
    WHERE bk.CustomerID = p_CustomerID
    ORDER BY p.PaymentDate DESC;
END //

-- Procedure: Get Dashboard Statistics
CREATE PROCEDURE sp_GetDashboardStats()
BEGIN
    SELECT 
        (SELECT COUNT(*) FROM Beds WHERE Status = 'Available') AS AvailableRooms,
        (SELECT COUNT(*) FROM Beds WHERE Status = 'Occupied') AS OccupiedRooms,
        (SELECT COUNT(*) FROM Bookings WHERE Status = 'Active') AS ActiveBookings,
        (SELECT COUNT(*) FROM Customers WHERE IsActive = TRUE) AS TotalCustomers,
        (SELECT COALESCE(SUM(Amount), 0) FROM Payments WHERE MONTH(PaymentDate) = MONTH(CURDATE()) AND YEAR(PaymentDate) = YEAR(CURDATE())) AS MonthlyRevenue,
        (SELECT COUNT(*) FROM Maintenance WHERE Status IN ('Pending', 'In Progress')) AS PendingMaintenance;
END //

DELIMITER ;

-- =====================================================
-- Grant privileges (adjust as needed)
-- =====================================================
-- GRANT ALL PRIVILEGES ON RoomBookingDB.* TO 'your_user'@'localhost';
-- FLUSH PRIVILEGES;

-- =====================================================
-- DATABASE SETUP COMPLETE
-- =====================================================

SELECT 'Database setup completed successfully!' AS Status;
SELECT 'Total Users:', COUNT(*) FROM Users;
SELECT 'Total Areas:', COUNT(*) FROM Areas;
SELECT 'Total Room Types:', COUNT(*) FROM RoomTypes;
SELECT 'Total Beds/Rooms:', COUNT(*) FROM Beds;
SELECT 'Total Customers:', COUNT(*) FROM Customers;
SELECT 'Total Bookings:', COUNT(*) FROM Bookings;
SELECT 'Total Payments:', COUNT(*) FROM Payments;
SELECT 'Total Maintenance Records:', COUNT(*) FROM Maintenance;
