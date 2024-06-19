using System.Globalization;

namespace ParkingSystem.Utils
{
    public static class DateUtils
    {
        public static string[] monthNames =
        {
            "Enero",
            "Febrero",
            "Marzo",
            "Abril",
            "Mayo",
            "Junio",
            "Julio",
            "Agosto",
            "Septiembre",
            "Octubre",
            "Noviembre",
            "Diciembre"
        };

        public static int CurrentWeekNumber()
        {
            DateTimeFormatInfo dfi = DateTimeFormatInfo.CurrentInfo;
            Calendar cal = dfi.Calendar;

            int currentWeekNumber = cal.GetWeekOfYear(
                                        DateTime.Now,
                                        CalendarWeekRule.FirstDay,
                                        DayOfWeek.Monday
                                    );

            return currentWeekNumber;
        }

        public static bool IsDateInWeek(string dateString, int WeekNumber)
        {
            DateTimeFormatInfo dfi = DateTimeFormatInfo.CurrentInfo;
            Calendar cal = dfi.Calendar;
            DateTime dateNow = DateTime.Now;

            if (DateTime.TryParse(dateString, out DateTime date))
            {
                int weekNumber = cal.GetWeekOfYear(date, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
                return date.Year == dateNow.Year && date.Month == dateNow.Month && weekNumber == WeekNumber;
            }
            return false;
        }

        public static bool IsDateInMonth(string dateString, int MonthNumber)
        {
            DateTime dateNow = DateTime.Now;

            if (DateTime.TryParse(dateString, out DateTime date))
                return date.Year == dateNow.Year && date.Month == MonthNumber;

            return false;
        }

        public static bool IsDateInDate(string dateString, DateTime dateCompare)
        {
            if (DateTime.TryParse(dateString, out DateTime date))
                return date.Year == dateCompare.Year && date.Month == dateCompare.Month && date.DayOfYear == dateCompare.DayOfYear;

            return false;
        }
    }
}
