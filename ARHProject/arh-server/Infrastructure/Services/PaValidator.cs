using System.Text.RegularExpressions;
using Core.Entities.Identity;
using Core.Interfaces;
using Core.Entities;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;
using System.Net.Http.Headers;
namespace Infrastructure.Services
{
    public class PaValidator: IPaValidator
    {
        private readonly IUnitOfWork _unitOfWork;
        private UserManager<AppUser> _userManager;
        private IConfiguration _config;
       

        public int RequiredLength { get; set; }
        public PaValidator(IUnitOfWork unitOfWork, UserManager<AppUser> userManager, IConfiguration config)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _config = config;
        }

       // public async Task<string> ValidatePassLength(int minlen,int maxlen, string password)
        // {
            // if(minlen != 0 && password.Length < minlen)
            // {
            //     return "Password should be at least "+ minlen +" character";
            // }
            // if(maxlen != 0 && password.Length > maxlen)
            // {
            //     return "Password should not be greater than "+ maxlen +" character";
            // }

        //     return "Success";
        // }

        /*
         * logic to validate password: I am using regex to count how many
         * types of characters exists in the password
        */
        public async Task<bool> ValidateUserPasswordAsync(string password, List<string> pat,List<string> notpat,int minlen,int maxlen)
        {
           string pwdpattern = pat.ToString();
         
            if (String.IsNullOrEmpty(password) || String.IsNullOrEmpty(pwdpattern)) 
            {
                return false;
            }
            if(minlen != 0 && password.Length < minlen)
            {
                return false;
            }
            if(maxlen != 0 && password.Length > maxlen)
            {
                return false;
            }

            int counter = 0;
            // List<string> patterns = new List<string>();
            // patterns.Add(@"[a-z]");                                          // lowercase
            // patterns.Add(@"[A-Z]");                                          // uppercase
            // patterns.Add(@"[0-9]");                                          // digits
            // // don't forget to include white space in special symbols
            // patterns.Add(@"[!@#$%^&*\(\)_\+\-\={}<>,\.\|""'~`:;\\?\/\[\] ]"); // special symbols

            // count type of different chars in password
            foreach (string p in pat)
            {
                if (Regex.IsMatch(password, p))
                {
                    counter++;
                }
                else
                {
                    counter =0;
                    break;
                }
            }
            foreach (string pn in notpat)
            {
                if (Regex.IsMatch(password, pn))
                {
                    counter = 0;
                    break;
                }

            }

            if (counter < 2)
            {
                return false;
            } 
                     
            return true;
        }
        public async Task<bool> ValidatePasswordAsync(AppUser appuser, string password)
        {
            bool isvalid = false;
            var r = await _unitOfWork.Repository<SysData>().ListAllAsync();
           
            int min = Convert.ToInt32(r.Where(x => x.FieldName == "Min_Password_Length").FirstOrDefault().FieldValue);
            int max =  Convert.ToInt32(r.Where(x => x.FieldName == "Max_Password_Length").FirstOrDefault().FieldValue);
            bool digitdata = bool.Parse(r.Where(x => x.FieldName == "Digit_Required").FirstOrDefault().FieldValue);
            bool upperdata =  bool.Parse(r.Where(x => x.FieldName == "Uppercase_Letter_Required").FirstOrDefault().FieldValue);
            bool lowerdata =  bool.Parse(r.Where(x => x.FieldName == "Lowercase_Letter_Required").FirstOrDefault().FieldValue);
            bool specialcdata =  bool.Parse(r.Where(x => x.FieldName == "Special_Character_Required").FirstOrDefault().FieldValue);
             
            List<string> patterns = new List<string>();
    
                if(digitdata)
                {
                    patterns.Add(@"[0-9]");                                           // digits
                }         
                if(upperdata)
                {
                    patterns.Add(@"[A-Z]");                                          // uppercase
                }
                if(lowerdata)
                {
                    patterns.Add(@"[a-z]");                                          // lowercase
                }
                if(specialcdata)
                {
                    // don't forget to include white space in special symbols
                    patterns.Add(@"[!@#$%^&*\(\)_\+\-\={}<>,\.\|""'~`:;\\?\/\[\] ]"); // special symbols
                }
            List<string> notpatterns = new List<string>();
    
                if(!digitdata)
                {
                    notpatterns.Add(@"[0-9]");                                           // digits
                }         
                if(!upperdata)
                {
                    notpatterns.Add(@"[A-Z]");                                          // uppercase
                }
                if(!lowerdata)
                {
                    notpatterns.Add(@"[a-z]");                                          // lowercase
                }
                if(!specialcdata)
                {
                    // don't forget to include white space in special symbols
                    notpatterns.Add(@"[!@#$%^&*\(\)_\+\-\={}<>,\.\|""'~`:;\\?\/\[\] ]"); // special symbols
                }
            
            if(!String.IsNullOrEmpty(password))
            {
                isvalid = await ValidateUserPasswordAsync(password, patterns, notpatterns, min, max);
            }
           
            return isvalid;
        }
    }
}