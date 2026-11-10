using ConnectDbAvalonia.Data;
using ConnectDbAvalonia.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace ConnectDbAvalonia.Services
{
    public static class BookingService
    {
        public static bool CreateBooking(int shiftId, int clientId, int quantity, int nights)
        {
            if (quantity <= 0) return false;

            using var db = Db.Create();

            var shift = db.Shifts.Include(s => s.House).FirstOrDefault(s => s.ShiftId == shiftId);

            if (shift == null || shift.AvailableSeats < quantity) return false;

            decimal totalPrice = shift.House.BasePrice * nights * quantity;
            if (nights >= 7) totalPrice = totalPrice * 0.9m;

            var booking = new Booking
            {
                BookingDate = DateOnly.FromDateTime(DateTime.Today),
                ClientId = clientId
            };
            db.Bookings.Add(booking);
            db.SaveChanges();

            var bookingItem = new BookingItem
            {
                BookingId = booking.BookingId,
                ShiftId = shiftId,
                Quantity = quantity,
                UnitPrice = totalPrice
            };
            db.BookingItems.Add(bookingItem);

            shift.AvailableSeats -= quantity;

            db.SaveChanges();
            return true;
        }
        public static System.Collections.Generic.List<Booking> GetBookings(int userId, bool isStaff)
        {
            using var db = Db.Create();
            var query = db.Bookings
                .Include(b => b.Client)
                .Include(b => b.BookingItems)
                .ThenInclude(bi => bi.Shift)
                .ThenInclude(s => s.House)
                .AsQueryable();

            if (!isStaff)
            {
                query = query.Where(b => b.ClientId == userId);
            }

            return query.ToList();
        }
    }
}