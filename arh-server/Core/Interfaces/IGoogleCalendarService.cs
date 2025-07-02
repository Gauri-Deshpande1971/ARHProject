using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entities;
using Core.Entities.Identity;
namespace Core.Interfaces
{
    public interface IGoogleCalendarService
    {
        Task<string> AddEventAsync(string summary, string description, DateTime? start, DateTime? end);
    }
}
