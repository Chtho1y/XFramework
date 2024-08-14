namespace Native.Event
{
    public class ManagerOrderChange : EventArgsBase
    {
        private static string _eventName = "ManagerOrderChange";

        private static long _eventId = 0;
        public static long EventId
        {
            get
            {
                _eventId = _eventId == 0 ? Native.Component.LaunchComponent.Event.GetEventId(_eventName) : _eventId;
                return _eventId;
            }
        }
        public override void Clear()
        {
            base.Clear();
        }
        public static ManagerOrderChange Create()
        {
            var data = ReferencePool.Acquire<ManagerOrderChange>();
            return data;
        }
    }
}