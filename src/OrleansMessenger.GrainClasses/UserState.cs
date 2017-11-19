using System.Collections.Generic;

namespace OrleansMessenger.GrainClasses
{
    public class UserState 
    {
        private readonly List<string> _history;

        public UserState()
        {
            _history = new List<string>();
        }

        public IEnumerable<string> History => _history;

        public void Apply(MessageReceived @event)
        {
            _history.Add($"{@event.From}: {@event.Message}");
        }

        public void Apply(MessageSent @event)
        {
            _history.Add($"{@event.To}: {@event.Message}");
        }
    }
}
