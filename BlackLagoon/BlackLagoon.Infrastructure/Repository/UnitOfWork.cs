using BlackLagoon.Application.Common.Interfaces;
using BlackLagoon.Common.Interfaces;
using BlackLagoon.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackLagoon.Infrastructure.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;
        public IVillaRepository Villas { get; private set; }

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            Villas = new VillaRepository(_db);
        }
        void Save()
        {
            _db.SaveChanges();
        }
    }
}
