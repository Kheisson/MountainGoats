namespace Analytics
{
    public class UpgradePurchaseEvent : GameEvent
    {
        private const string EVENT_NAME = "UpgradePurchase";
        private readonly int _upgradeProgression;
        private readonly string _upgradeType;
        
        public UpgradePurchaseEvent(int upgradeProgression, string upgradeType) : base(EVENT_NAME)
        {
            _upgradeProgression = upgradeProgression;
            _upgradeType = upgradeType;
        }

        protected override bool ValidateEvent()
        {
            if (_upgradeProgression <= 0 || string.IsNullOrEmpty(_upgradeType))
            {
                return false;
            }
            
            SetParameter("upgradeProgression", _upgradeProgression);
            SetParameter("upgradeType", _upgradeType);

            return true;
        }
    }
}