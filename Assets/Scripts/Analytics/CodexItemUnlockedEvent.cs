namespace Analytics
{
    public class CodexItemUnlockedEvent : GameEvent
    {
        private const string EVENT_NAME = "CodexItemUnlocked";
        private readonly float _completionPercentage;
        private readonly string _itemName;
        
        public CodexItemUnlockedEvent(string itemName, float completionPercentage) : base(EVENT_NAME)
        {
            _itemName = itemName;
            _completionPercentage = completionPercentage;
        }

        protected override bool ValidateEvent()
        {
            if (string.IsNullOrEmpty(_itemName) || _completionPercentage < 0f || _completionPercentage > 100f)
            {
                return false;
            }
            
            SetParameter("codexCompletion", _completionPercentage);
            SetParameter("codexItemName", _itemName);

            return true;
        }
    }
}