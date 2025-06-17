using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Calendar.v3;
using Google.Apis.Services;
using Core.Interfaces;

namespace Infrastructure.Services
{
    public class CalendarServiceHelper : IGoogleCalendarService
    {
        private readonly CalendarService _calendarService;

        public CalendarServiceHelper(string jsonFilePath)
        {
            GoogleCredential credential;//"C:\\Gauri\\googleapikey.json"
            using (var stream = new FileStream(jsonFilePath, FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream)
                    .CreateScoped(CalendarService.Scope.Calendar);
            }

            _calendarService = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Appointment Calendar",
            });
        }

        public async Task<string> AddEventAsync(string summary, string description, DateTime start, DateTime end)
        {
            var calendarId = summary; // Or your custom calendar ID

            var newEvent = new Event()
            {
                Summary = "Appointment Calendar",
                Description = description,
                Start = new EventDateTime() { DateTime = start, TimeZone = "Asia/Kolkata" },
                End = new EventDateTime() { DateTime = end, TimeZone = "Asia/Kolkata" },
            };

            var request = _calendarService.Events.Insert(newEvent, calendarId);
            var createdEvent = await request.ExecuteAsync();

            return createdEvent.HtmlLink; // Return calendar event URL
        }
    }
}

