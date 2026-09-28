using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication3.Models
{
    public static class Data
    {
        public static List<Room> Rooms = new List<Room>();
        public static List<Employee> Employees = new List<Employee>();
        public static List<Booking> Bookings = new List<Booking>();
        public static List<BookingLog> Logs = new List<BookingLog>();

        public static int NextRoomId = 1, NextEmployeeId = 1, NextBookingId = 1, NextLogId = 1;

        public static void Log(int bookingId, string action, string details)
        {
            Logs.Add(new BookingLog { Id = NextLogId++, BookingId = bookingId, CreatedAt = DateTime.Now, Action = action, Details = details });
        }
        public static bool Overlaps(int roomId, DateTime start, DateTime end, int ignoreBookingId)
        {
            return Bookings.Any(b => b.RoomId == roomId
                && b.Id != ignoreBookingId
                && (b.Status == BookingStatus.Planned || b.Status == BookingStatus.Started)
                && start < b.EndTime && b.StartTime < end);
        }
        public static object ToDto(Booking b)
        {
            Room room = Rooms.FirstOrDefault(r => r.Id == b.RoomId);
            Employee emp = Employees.FirstOrDefault(e => e.Id == b.EmployeeId);
            return new
            {
                b.Id,
                b.RoomId,
                RoomName = room?.Name,
                b.EmployeeId,
                EmployeeFullName = emp?.FullName,
                b.Topic,
                b.StartTime,
                b.EndTime,
                b.ParticipantsCount,
                b.Status
            };
        }

        static void AddRoom(string name, int capacity, bool projector, bool board, bool active)
        {
            Rooms.Add(new Room { Id = NextRoomId++, Name = name, Capacity = capacity, HasProjector = projector, HasWhiteboard = board, IsActive = active });
        }

        static void AddEmployee(string fullName, string department, string email)
        {
            Employees.Add(new Employee { Id = NextEmployeeId++, FullName = fullName, Department = department, Email = email });
        }
        static void AddBooking(int room, int emp, int dayOffset, int hourFrom, int hourTo, int people, string topic, BookingStatus status)
        {
            DateTime day = DateTime.Today.AddDays(dayOffset);
            var b = new Booking
            {
                Id = NextBookingId++,
                RoomId = room,
                EmployeeId = emp,
                StartTime = day.AddHours(hourFrom),
                EndTime = day.AddHours(hourTo),
                ParticipantsCount = people,
                Topic = topic,
                Status = status
            };
            Bookings.Add(b);
            Log(b.Id, "Created", "Начальные данные");
        }

        static Data()
        {
            AddRoom("Альфа", 4, false, true, true);
            AddRoom("Бета", 8, true, true, true);
            AddRoom("Гамма", 12, true, true, true);
            AddRoom("Дельта", 2, false, false, true);
            AddRoom("Эпсилон", 6, true, false, true);
            AddRoom("Омега", 20, true, true, false); 

            AddEmployee("Иванов Иван Иванович", "Разработка", "ivanov@corp.local");
            AddEmployee("Петрова Анна Сергеевна", "Аналитика", "petrova@corp.local");
            AddEmployee("Сидоров Максим Олегович", "Тестирование", "sidorov@corp.local");
            AddEmployee("Козлова Елена Викторовна", "HR", "kozlova@corp.local");
            AddEmployee("Смирнов Дмитрий Андреевич", "Бухгалтерия", "smirnov@corp.local");
            AddEmployee("Новикова Ольга Павловна", "Маркетинг", "novikova@corp.local");
            AddEmployee("Морозов Алексей Игоревич", "Поддержка", "morozov@corp.local");
            AddEmployee("Волкова Мария Николаевна", "Юридический отдел", "volkova@corp.local");

            AddBooking(1, 1, -1, 10, 11, 3, "Ретроспектива спринта", BookingStatus.Finished);  
            AddBooking(2, 2, -1, 14, 15, 6, "Демо продукта", BookingStatus.Finished);          
            AddBooking(3, 3, 0, 8, 9, 10, "Планирование тестов", BookingStatus.Cancelled);    
            AddBooking(1, 4, 1, 10, 11, 4, "Собеседование", BookingStatus.Planned);            
            AddBooking(2, 5, 1, 12, 13, 5, "Бюджет квартала", BookingStatus.Planned);          
            AddBooking(2, 1, 1, 13, 14, 7, "Архитектурный совет", BookingStatus.Planned);      
            AddBooking(3, 6, 2, 9, 11, 12, "Маркетинговая стратегия", BookingStatus.Planned);  
            AddBooking(4, 7, 2, 15, 16, 2, "Один на один", BookingStatus.Cancelled);           
            AddBooking(5, 8, 3, 11, 12, 6, "Юридический разбор", BookingStatus.Planned);       
            AddBooking(5, 2, 0, 0, 1, 4, "Ночной релиз", BookingStatus.Started);               
        }
    }
}