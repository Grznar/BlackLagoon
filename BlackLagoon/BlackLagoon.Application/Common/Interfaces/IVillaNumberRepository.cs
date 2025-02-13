using BlackLagoon.Application.Common.Interfaces;
using BlackLagoon.Domain.Entities;
using System.Linq.Expressions;

namespace BlackLagoon.Common.Interfaces
{
    public interface IVillaNumberRepository : IRepository<VillaNumber>
    {
      
        void Update(VillaNumber entity);
        
        
    }
}
