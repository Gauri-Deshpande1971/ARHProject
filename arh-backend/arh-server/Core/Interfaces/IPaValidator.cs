using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entities;
using Core.Entities.Identity;
using Core.Models;
using Microsoft.AspNetCore.Identity;

namespace Core.Interfaces
{
    public interface IPaValidator
    {
        Task<bool> ValidateUserPasswordAsync(string password, List<string> pat, List<string> nopat,int minlen,int maxlen);
        Task<bool> ValidatePasswordAsync(AppUser appuser, string password); // 
    }
}

