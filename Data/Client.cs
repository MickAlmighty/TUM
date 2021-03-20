using System;
using System.Text.RegularExpressions;

namespace Data
{
    public sealed class Client : IUpdatable<Client>
    {
        private const uint USERNAME_MIN_LENGTH = 3;
        private const string PHONE_NUMBER_REGEX = @"^((\+[0-9]{1,3}\ )?[0-9]{3}\ [0-9]{3}\ [0-9]{3,4})|((\+[0-9]{1,3}-)?[0-9]{3}-[0-9]{3}-[0-9]{3,4})$";
        private string _Username;
        private string _FirstName;
        private string _LastName;
        private string _Street;
        private uint _StreetNumber;
        private string _PhoneNumber;

        [Id]
        public string Username
        {
            get
            {
                return _Username;
            }
            private set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(Username));
                }
                if (value.Length < USERNAME_MIN_LENGTH)
                {
                    throw new ArgumentOutOfRangeException($"Provided {nameof(Username)} length ({value.Length}) is lower than expected minimum ({USERNAME_MIN_LENGTH}): '{value}'.");
                }
                _Username = value;
            }
        }

        public string FirstName
        {
            get
            {
                return _FirstName;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(FirstName));
                }
                if (value.Length == 0)
                {
                    throw new ArgumentOutOfRangeException($"{nameof(FirstName)} cannot be empty!");
                }
                _FirstName = value;
            }
        }

        public string LastName
        {
            get
            {
                return _LastName;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(LastName));
                }
                if (value.Length == 0)
                {
                    throw new ArgumentOutOfRangeException($"{nameof(LastName)} cannot be empty!");
                }
                _LastName = value;
            }
        }

        public string Street
        {
            get
            {
                return _Street;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(Street));
                }
                if (value.Length == 0)
                {
                    throw new ArgumentOutOfRangeException($"{nameof(Street)} cannot be empty!");
                }
                _Street = value;
            }
        }

        public uint StreetNumber
        {
            get
            {
                return _StreetNumber;
            }
            set
            {
                if (value == 0)
                {
                    throw new ArgumentOutOfRangeException($"{nameof(StreetNumber)} cannot be zero!");
                }
                _StreetNumber = value;
            }
        }

        public string PhoneNumber
        {
            get
            {
                return _PhoneNumber;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(PhoneNumber));
                }
                if (!Regex.IsMatch(value, PHONE_NUMBER_REGEX))
                {
                    throw new FormatException($"Provided {nameof(PhoneNumber)} value does not match the expected format! Format examples: +1-200-300-4000, +48 500 500 500.");
                }
                _PhoneNumber = value;
            }
        }

        public Client(string username, string firstName, string lastName, string street, uint streetNumber, string phoneNumber)
        {
            Username = username;
            FirstName = firstName;
            LastName = lastName;
            Street = street;
            StreetNumber = streetNumber;
            PhoneNumber = phoneNumber;
        }

        public void Update(Client client)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }
            if (Username != client.Username)
            {
                throw new ArgumentException(nameof(client));
            }
            FirstName = client.FirstName;
            LastName = client.LastName;
            Street = client.Street;
            StreetNumber = client.StreetNumber;
            PhoneNumber = client.PhoneNumber;
        }
    }
}
