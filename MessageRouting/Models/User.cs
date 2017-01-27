namespace MessageRouting.Models
{
    using Microsoft.Bot.Builder.Dialogs;

    public class User
    {
        public User(Address address, string name)
        {
            this.AddressInformation = address;
            this.Name = name;
        }

        public Address AddressInformation { get; private set; }
        
        public string Name { get; private set; }
    }
}