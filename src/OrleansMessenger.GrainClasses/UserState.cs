using Orleans;
using System.Collections.Generic;

namespace OrleansMessenger.GrainClasses
{
    public class UserState : GrainState
    {
        public List<string> History { get; set; }

        public UserState()
        {
            History = new List<string>();
        }
    }
}
