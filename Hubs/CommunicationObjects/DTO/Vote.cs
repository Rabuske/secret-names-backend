﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecretNamesBackend.Hubs.CommunicationObjects.DTO
{
    public class Vote
    {
        public string UserName { get; set; }
        public string Word { get; set; }
        public bool Pass { get; set; }
    }
}
