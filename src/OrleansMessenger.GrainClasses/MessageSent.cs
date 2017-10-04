namespace OrleansMessenger.GrainClasses
{
    public class MessageSent : UserEvent
    {
        public string Message { get; set; }

        public string To { get; set; }
    }
}
