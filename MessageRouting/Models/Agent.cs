namespace MessageRouting.Models
{
    using Microsoft.Bot.Builder.Dialogs;

    public class Agent
    {
        public Agent(Address address, string name)
        {
            this.AddressInformation = address;
            this.Name = name;
        }

        public Address AddressInformation { get; private set; }

        public string Name { get; private set; }

        public bool IsBusy
        {
            get { return this.AssignedUser != null; }
        }

        public User AssignedUser { get; private set; }

        public void AssignUser(User user)
        {
            this.AssignedUser = user;
        }

        public void SetAvailable()
        {
            this.AssignedUser = null;
        }
    }
}