using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class ActionLog : BaseEntity
    {
        public string ModuleName { get; set; }
        public string Description { get; set; }
        public string EntityName { get; set; }
        public string EntityValue { get; set; }        
        public string ActionName { get; set; }
        public string ClientIP { get; set; }
        public string ClientBrowser { get; set; }
        /// <summary>
        /// Client Type can be Web, Mobile, Desktop
        /// </summary>
        public string? ClientType { get; set; }
    }
}