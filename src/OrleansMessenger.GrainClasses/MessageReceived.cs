namespace OrleansMessenger.GrainClasses
{
    public class MessageReceived : UserEvent
    {
        public string Message { get; set; }

        public string From { get; set; }
    }
}
