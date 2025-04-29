namespace Analytics
{
    public class DepthReachedEvent : GameEvent
    {
        private const string EVENT_NAME = "depthReached";
        private readonly int _depth;
        
        public DepthReachedEvent(int depth) : base(EVENT_NAME)
        {
            _depth = depth;
        }
        
        protected override bool ValidateEvent()
        {
            if (_depth <= 0 || DataStorageService.GetGameData().DepthReached >= _depth)
            {
                return false;
            }
            
            DataStorageService.ModifyGameDataSync(gameData =>
            {
                gameData.DepthReached = _depth;
                SetParameter("depth", _depth);
                
                return true;
            });

            return true;
        }
    }
}