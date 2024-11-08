namespace ZoDream.Shared.Database
{
    public interface IGroupEntity
    {
        public int Id { get; set; }

        public int ParentId { get; set; }

        public string Name { get; set; }
    }
}
