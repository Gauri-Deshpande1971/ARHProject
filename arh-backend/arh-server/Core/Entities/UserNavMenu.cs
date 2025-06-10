namespace Core.Entities
{
    public class UserNavMenu : BaseEntity
    {
        public int NavMenuId { get; set; }
        public string AppUserId { get; set; }
        public string FormOptions { get; set; }
        public string AppMode { get; set; } = "WEB,MOBILE,";

        public void CopyInternalToExternal(UserNavMenu src, bool IsUpdate)
        {
            //Id = src.Id;
            NavMenuId = src.NavMenuId;
            AppUserId = src.AppUserId;
            FormOptions = src.FormOptions;
            if (!IsUpdate)
            {
                CreatedOn = src.CreatedOn;
                CreatedById = src.CreatedById;
                CreatedByName = src.CreatedByName;
            }
            IsDeleted = src.IsDeleted;
            IsActive = src.IsActive;
            UCode = src.UCode;
            SequenceNo = src.SequenceNo;
            ExtraId1 = src.ExtraId1;
            ExtraId2 = src.ExtraId2;
            ExtraValue1 = src.ExtraValue1;
            ExtraValue2 = src.ExtraValue2;
            LogHistory = src.LogHistory;
            AppMode = src.AppMode;
        }
    }
}