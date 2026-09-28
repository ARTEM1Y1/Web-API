using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication3.Models
{
    public enum BookingStatus { Planned, Started, Finished, Cancelled }

    public class Room
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Capacity { get; set; }
        public bool HasProjector { get; set; }
        public bool HasWhiteboard { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class Employee
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Department { get; set; }
        public string Email { get; set; }
    }

    public class Booking
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public int EmployeeId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int ParticipantsCount { get; set; }
        public string Topic { get; set; }
        public BookingStatus Status { get; set; }
    }

    public class BookingLog
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Action { get; set; }
        public string Details { get; set; }
    }
    public class BookingRequest
    {
        public int RoomId { get; set; }
        public int EmployeeId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int ParticipantsCount { get; set; }
        public string Topic { get; set; }
    }

    public class StatusRequest
    {
        public string Status { get; set; }
    }
}