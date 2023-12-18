using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhiteLagoon.Domain.Entities;
using WhitleLagoon.Web.ViewModels;

namespace WhiteLagoon.Application.Common.Utility
{
    public static class SD
    {
        #region Const Values

        #region Role Values
        public const string Role_Customer = "Customer";
        public const string Role_Admin = "Admin";
        #endregion

        #region Villa Status Values
        public const string StatusPending = "Pending";
        public const string StatusApproved = "Approved";
        public const string StatusCheckedIn = "CheckedIn";
        public const string StatusCompleted = "Completed";
        public const string StatusCanceled = "Canceled";
        public const string StatusRefunded = "Refunded";
        #endregion

        #endregion

        public static int VillaRoomsAvailable_Count(int villaId,
            List<VillaNumber> villaNumberList, DateOnly checkInDate, int nights,
            List<Booking> bookings)
        {
            List<int> bookingInDate = new();
            int finalAvailableRoomForAllNights = int.MaxValue;
            var roomsinVilla = villaNumberList.Where(x=> x.VillaId == villaId).Count();

            for (int i = 0;i < nights; i++)
            {
                var villasBooked = bookings.Where(u => u.CheckInDate <= checkInDate.AddDays(i)
                && u.CheckOutDate > checkInDate.AddDays(i) && u.VillaId==villaId);

                foreach (var booking in villasBooked)
                {
                    if (!bookingInDate.Contains(booking.Id))
                    {
                        bookingInDate.Add(booking.Id);
                    }
                }

                var totalAvailableRooms = roomsinVilla - bookingInDate.Count;
                if (totalAvailableRooms == 0)
                {
                    return 0;
                }
                else
                {
                    if (finalAvailableRoomForAllNights > totalAvailableRooms)
                    {
                        finalAvailableRoomForAllNights = totalAvailableRooms;
                    }
                }
            }

            return finalAvailableRoomForAllNights;

        }

        public static RadialBarChartDto GetRadialChartDataModel(int totalCount, double currentMonthCount, double previousMonthCount)
        {
            RadialBarChartDto RadialBarChartDto = new();

            int increaseDecreaseRatio = 100;

            if (previousMonthCount != 0)
            {
                increaseDecreaseRatio = Convert.ToInt32((currentMonthCount - previousMonthCount) / previousMonthCount * 100);
            }

            RadialBarChartDto.TotalCount = totalCount;
            RadialBarChartDto.CountInCurrentMonth = Convert.ToInt32(currentMonthCount);
            RadialBarChartDto.HasRatioIncreased = currentMonthCount > previousMonthCount;
            RadialBarChartDto.Series = new int[] { increaseDecreaseRatio };

            return RadialBarChartDto;
        }
    }
}
