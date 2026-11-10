using Avalonia.Controls;
using Avalonia.Interactivity;
using ConnectDbAvalonia.Data;
using ConnectDbAvalonia.Models;
using ConnectDbAvalonia.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace ConnectDbAvalonia
{
    public partial class MainWindow : Window
    {
        private House? _selectedHouse;

        public MainWindow()
        {
            InitializeComponent();
            LoadHouses();
            LoadUsers();
        }

        private void LoadHouses()
        {
            using var db = Db.Create();
            HousesList.ItemsSource = db.Houses.ToList();
        }

        private void LoadUsers()
        {
            using var db = Db.Create();
            UsersCombo.ItemsSource = db.Users.Include(u => u.Role).ToList();
        }

        private void LoginClick(object? sender, RoutedEventArgs e)
        {
            if (UsersCombo.SelectedItem is User selectedUser)
            {
                Session.CurrentUser = selectedUser;
                UpdateUI();
            }
        }

        private void LogoutClick(object? sender, RoutedEventArgs e)
        {
            Session.CurrentUser = null;
            UsersCombo.SelectedItem = null;
            UpdateUI();
            BookingPanel.IsVisible = false;
        }

        private void UpdateUI()
        {
            if (Session.IsGuest)
            {
                LoginPanel.IsVisible = true;
                UserInfoPanel.IsVisible = false;
                NavPanel.IsVisible = false;
            }
            else
            {
                LoginPanel.IsVisible = false;
                UserInfoPanel.IsVisible = true;
                NavPanel.IsVisible = true;

                UserText.Text = $"{Session.CurrentUser?.LastName} {Session.CurrentUser?.FirstName} ({Session.CurrentUser?.Role?.RoleName})";

                // ИЗМЕНЕНИЕ: Показываем кнопку "Мои брони" ВСЕМ, кто вошел в систему
                BookingsButton.IsVisible = true;

                // При входе всегда показываем каталог
                CatalogGrid.IsVisible = true;
                BookingsGrid.IsVisible = false;
            }
        }

        // --- ЛОГИКА БРОНИРОВАНИЯ ---
        private void HouseSelected(object? sender, SelectionChangedEventArgs e)
        {
            if (HousesList.SelectedItem is House house)
            {
                _selectedHouse = house;
                SelectedHouseName.Text = house.HouseName;
                BookingPanel.IsVisible = true;
                CalculatePrice(null, null);
            }
        }

        private void CalculatePrice(object? sender, NumericUpDownValueChangedEventArgs? e)
        {
            if (_selectedHouse == null) return;

            int nights = (int)(NightsInput.Value ?? 1);
            int seats = (int)(SeatsInput.Value ?? 1);

            decimal basePrice = _selectedHouse.BasePrice;
            decimal total = basePrice * nights * seats;

            // Скидка 10%, если ночей >= 7
            if (nights >= 7) total = total * 0.9m;

            PriceOutput.Text = $"Итого: {total:N2} руб.";
        }

        private void BookClick(object? sender, RoutedEventArgs e)
        {
            if (Session.IsGuest) return;
            if (_selectedHouse == null) return;

            int nights = (int)(NightsInput.Value ?? 1);
            int seats = (int)(SeatsInput.Value ?? 1);

            using var db = Db.Create();
            var shift = db.Shifts.FirstOrDefault(s => s.HouseId == _selectedHouse.HouseId && s.AvailableSeats >= seats);

            if (shift == null)
            {
                Console.WriteLine("ОШИБКА: Нет свободных мест в этом домике!");
                return;
            }

            bool success = BookingService.CreateBooking(shift.ShiftId, Session.CurrentUser!.UserId, seats, nights);
            Console.WriteLine($"РЕЗУЛЬТАТ БРОНИРОВАНИЯ: {success}"); // <--- ДОБАВЬ ЭТО

            if (success)
            {
                LoadHouses();
                LoadBookings();
            }
        }

        // --- НАВИГАЦИЯ ---
        private void CatalogClick(object? sender, RoutedEventArgs e)
        {
            CatalogGrid.IsVisible = true;
            BookingsGrid.IsVisible = false;
        }

        private void BookingsClick(object? sender, RoutedEventArgs e)
        {
            CatalogGrid.IsVisible = false;
            BookingsGrid.IsVisible = true;
            LoadBookings(); // Загружаем брони при переходе
        }

        private void LoadBookings()
        {
            if (Session.CurrentUser == null) return;

            try
            {
                var bookings = BookingService.GetBookings(Session.CurrentUser!.UserId, Session.IsStaff);

                // <--- ДОБАВЬ ЭТИ ДВЕ СТРОЧКИ
                Console.WriteLine($"Найдено броней для пользователя {Session.CurrentUser.Login}: {bookings.Count}");

                BookingsList.ItemsSource = bookings;
            }
            catch (Exception ex)
            {
                // <--- ДОБАВЬ ЭТО, ЧТОБЫ УВИДЕТЬ ОШИБКУ, ЕСЛИ ОНА ЕСТЬ
                Console.WriteLine("ОШИБКА ПРИ ЗАГРУЗКЕ БРОНЕЙ: " + ex.Message);
            }
        }
    }
}