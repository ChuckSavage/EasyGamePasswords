namespace ToClipboard.Model
{
    public enum SortType
    {
        /// <summary>
        /// Ordered alphabetically
        /// </summary>
        Ascending,
        /// <summary>
        /// Ordered reverse alphabetically
        /// </summary>
        Descending,
        /// <summary>
        /// Ordered by the time the item was last used
        /// </summary>
        LastUsed,
        /// <summary>
        /// Ordered, descending, by the count of uses
        /// </summary>
        MostUsed,
        /// <summary>
        /// Order the items were added to the database
        /// </summary>
        OrderAdded,
        /// <summary>
        /// Ordered by a percent of uses and last used combination, weights set by user
        /// </summary>
        Percentage
    }
}