using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecretNamesBackend.Models
{
    public class Card
    {
        public string Word { get; set; }
        public bool HasBeenRevealed { get; set; }
        public string Agent { get; set; }
    }
}
