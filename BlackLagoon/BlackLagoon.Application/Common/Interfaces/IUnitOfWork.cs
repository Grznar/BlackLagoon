﻿using BlackLagoon.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackLagoon.Application.Common.Interfaces
{
    public interface IUnitOfWork 
    {
        IVillaRepository Villas { get; }
        IVillaNumberRepository VillaNumbers { get; }
        IAmenityRepository Amenities { get; }
        void Save();
    }
}
