using System.Collections.Generic;
using Core.Entities;

namespace Core.Specifications
{
    public class FormGridHeaderSpecification : BaseSpecification<FormGridHeader>
    {
        public FormGridHeaderSpecification()
        {
        }

        public FormGridHeaderSpecification(string formname, int userid): base(x => x.FormName == formname)
        {
            List<int?> UserIdList = new List<int?>();
            UserIdList.Add(-1);
            //  UserIdList.Add(0);
            if (userid > 0)
            {
                UserIdList.Add(userid);
            }

            //AddInclude(x => UserIdList.Contains(x.OfficeUserId));
            AddOrderByDescending(x => x.OfficeUserId);
        }
    }
}