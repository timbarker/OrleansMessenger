using System.Collections.Generic;

namespace OrleansMessenger.GrainClasses
{
    public class UserState 
    {
        public List<string> History { get; set; }

        public UserState()
        {
            History = new List<string>();
        }

        public void Apply(MessageReceived @event)
        {
            History.Add($"{@event.From}: {@event.Message}");
        }

        public void Apply(MessageSent @event)
        {
            History.Add($"{@event.To}: {@event.Message}");
        }
    }
}
