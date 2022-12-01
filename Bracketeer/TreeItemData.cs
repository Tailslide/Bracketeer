namespace Bracketeer
{
    /// <summary>
    /// Hold state for mudblazor tree items.. adapted from mudblazor sample
    /// </summary>
    public class TreeItemData
    {
        public string Text { get; set; }

        /// <summary>
        /// Indentation level
        /// </summary>
        public int Level { get; set; } 

        public bool IsExpanded { get; set; } = true;

        public bool HasChild => TreeItems != null && TreeItems.Count > 0;

        public HashSet<TreeItemData> TreeItems { get; set; } = new HashSet<TreeItemData>();

        public TreeItemData(string text, int level)
        {
            Level = level;
            Text = text;
        }
    }
}
