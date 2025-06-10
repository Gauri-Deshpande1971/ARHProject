using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Dtos
{
    public class ActionLogDto : BaseDto
    {
        public string ModuleName { get; set; }
        public string Description { get; set; }
        public string EntityName { get; set; }
        public string EntityValue { get; set; }        
        public string ActionName { get; set; }
        public string ClientIP { get; set; }
        public string ClientBrowser { get; set; }
    }
}