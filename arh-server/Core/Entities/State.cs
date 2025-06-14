namespace Core.Entities
{
    public class State : BaseEntity
    {
        public string StateName {get; set; }
        public string ShortCode { get; set; }
        public string StateGSTCode { get; set; }
    }
}
