using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecretNamesBackend.Models.Exceptions
{
    public class GameHasStartedException : Exception
    {
        public GameHasStartedException() : base("Cannot Join: Game has already started")
        {

        }
    }
}
