namespace ToClipboard.Model
{
    public interface IJumpList
    {
        long JumpListId { get; set; }
        string Name { get; set; }
        SortType SortBy { get; set; }
    }
}
