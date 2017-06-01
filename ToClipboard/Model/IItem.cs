using System;

namespace ToClipboard.Model
{
    public interface IItem
    {
        long ItemId { get; set; }

        /// <summary>
        /// Title on Jump List
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// Test to copy to clipboard
        /// </summary>
        string Text { get; set; }
        DateTime DateCreated { get; set; }
        DateTime DateLastUsed { get; set; }
        long CountUsed { get; set; }

        /// <summary>
        /// App to launch after selection
        /// </summary>
        string LaunchApp { get; set; }

        /// <summary>
        /// Launch app after selecting
        /// </summary>
        bool DoLaunchApp { get; set; }
    }
}
