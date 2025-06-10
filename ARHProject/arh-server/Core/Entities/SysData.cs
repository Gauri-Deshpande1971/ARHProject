namespace Core.Entities
{
    public class SysData : BaseEntity
    {
        public string FieldName { get; set; }
        public string FieldValue { get; set; }
        
        // public void CopyInternalToExternal(SysData src, bool IsUpdate)
        // {
        //     //Id = src.Id;
        //     FieldName = src.FieldName;
        //     FieldValue = src.FieldValue;
            
        //     if (!IsUpdate)
        //     {
        //         CreatedOn = src.CreatedOn;
        //         CreatedById = src.CreatedById;
        //         CreatedByName = src.CreatedByName;
        //     }
        //     IsDeleted = src.IsDeleted;
        //     IsActive = src.IsActive;
        //     UCode = src.UCode;
        //     SequenceNo = src.SequenceNo;
        //     ExtraId1 = src.ExtraId1;
        //     ExtraId2 = src.ExtraId2;
        //     ExtraValue1 = src.ExtraValue1;
        //     ExtraValue2 = src.ExtraValue2;
        //     LogHistory = src.LogHistory;            
        // }

    }
}
