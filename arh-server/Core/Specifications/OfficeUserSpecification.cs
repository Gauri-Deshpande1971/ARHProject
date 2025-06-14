using System.Collections.Generic;
using System.Linq;
using Core.Entities;

namespace Core.Specifications
{
    public class OfficeUsersSpecification : BaseSpecification<OfficeUser>
    {
        public OfficeUsersSpecification()
        {
            AddInclude(x => x.Department);
            AddInclude(x => x.AppRole);
        }

        public OfficeUsersSpecification(int id) : base(x => x.Id == id)
        {
            AddInclude(x => x.Department);
            AddInclude(x => x.AppRole);
        }
        public OfficeUsersSpecification(List<int> ids) : base(x => ids.Contains(x.Id))
        {
            AddInclude(x => x.Department);
            AddInclude(x => x.AppRole);
        }

        public OfficeUsersSpecification(string WorkEmail) : base(x => x.WorkEmail == WorkEmail)
        {
            AddInclude(x => x.Department);
            AddInclude(x => x.AppRole);
        }

        public OfficeUsersSpecification(int something, string AppUserId) : base(x => x.AppUserId == AppUserId)
        {
            AddInclude(x => x.Department);
            AddInclude(x => x.AppRole);
        }

        public OfficeUsersSpecification(string AppRoleName, int something)
                : base(x => x.AppRole.AppRoleName == AppRoleName)
        {
            AddInclude(x => x.Department);
            AddInclude(x => x.AppRole);
        }
    }
}
