using System;

namespace API.Dtos
{
    public class BaseDto
    {
        //public int Id { get; set; }
        public DateTime CreatedOn { get; set; }
        //public int CreatedById { get; set; }
        public string? CreatedByName { get; set; }
        public bool IsActive { get; set; }

        public bool IsUpdated { get; set; }
        public bool IsDeleted { get; set; } = false;
        public bool IsExisting { get; set; }

        public Guid UCode { get; set; }
        public int SequenceNo { get; set; }
        public int? ExtraId1 { get; set; }
        public int? ExtraId2 { get; set; }

        public string? ExtraValue1 { get; set; }
        public string? ExtraValue2 { get; set; }

        public string? JsonData { get; set; }
        //public string LogHistory { get; set; }

        public bool IsValidForSave()
        {
            return true;
        }

        public DateTime? ReturnAsDate(string dateValue)
        {
            if (string.IsNullOrWhiteSpace(dateValue))
                return null;

            DateTime d = DateTime.UtcNow;

            if (dateValue.Length < 10)
                return null;

            if (dateValue.Length == 10)
            {
                var arrd = dateValue.Split('-');
                if (arrd.Length == 3)
                {
                    //  if (int.TryParse() arrd[0])
                }
            }

            return null;
        }
    }
}
