namespace UI
{
    public readonly struct ClicksBlockedData
    {
        public bool IsBlocked { get; }

        public ClicksBlockedData(bool isBlocked)
        {
            IsBlocked = isBlocked;
        }
    }
}