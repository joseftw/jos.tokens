using System;

namespace JOS.Tokens
{
    public class Company
    {
        public Company(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public string Name { get; }
    }
}
