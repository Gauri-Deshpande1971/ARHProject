namespace API.Dtos
{
    public class UserNavMenuDto : BaseDto
    {
        public int NavMenuId { get; set; }
        public string AppUserId { get; set; }
        public string FormOptions { get; set; }
    }
}