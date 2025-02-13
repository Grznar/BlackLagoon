using BlackLagoon.Application.Common.Interfaces;
using BlackLagoon.Domain.Entities;
using System.Linq.Expressions;

namespace BlackLagoon.Common.Interfaces
{
    public interface IAmenityRepository : IRepository<Amenity>
    {
      
        void Update(Amenity entity);
        
        
    }
}
