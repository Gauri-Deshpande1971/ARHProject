using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class Designation : BaseEntity
    {
        public string DesignationName { get; set; }
        public string DesignationCode { get; set; }

        public string OrganizationNameInclude { get; set; }
        public string OrganizationNameExclude { get; set; }
        /// <summary>
        /// Whether the designation is applicable for reporting
        /// </summary>
        public bool IsReportingApplicable { get; set; }

    }
}