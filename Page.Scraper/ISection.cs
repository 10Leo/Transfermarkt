namespace Page.Scraper.Contracts
{
    /// <summary>
    /// Section of a Page.
    /// </summary>
    public interface ISection
    {
        /// <summary>
        /// A name that describes the Section.
        /// </summary>
        string Name { get; set; }

        Children ChildrenType { get; }
        ParseLevel ParseLevel { get; set; }

        void Parse(bool parseChildren);
    }

    public enum Children
    {
        NO,
        SAME_PAGE,
        DIFF_PAGE
    }
}
