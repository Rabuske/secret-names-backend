using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecretNamesBackend.Models.Exceptions
{
    public class TeamNotCompleteException : Exception
    {
        public TeamNotCompleteException() : base("Teams must have at least two participants each")
        {

        }
    }
}
