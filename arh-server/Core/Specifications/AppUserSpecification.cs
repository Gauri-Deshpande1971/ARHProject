using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Entities;
using Core.Entities.Identity;

namespace Core.Specifications
{
    public class AppUserSpecification : BaseSpecification<AppUser>
    {
        public AppUserSpecification(List<int> ids): base(x => ids.Contains(x.OfficeUserId))
        {
            AddOrderBy(x => x.OfficeUserId);
        }      
    }
}