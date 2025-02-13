using BlackLagoon.Domain.Entities;

namespace BlackLagoon.ViewModels
{
    public class HomePageVM
    {
        public IEnumerable<Villa>? VillaList { get; set; }
        public DateOnly CheckInDate { get; set; }
        public DateOnly? CheckOutDate { get; set; }
        public int NumberOfNights { get; set; }

    }
}
