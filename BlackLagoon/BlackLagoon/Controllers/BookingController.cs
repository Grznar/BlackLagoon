using BlackLagoon.Application.Common.Interfaces;
using BlackLagoon.Application.Common.Utility;
using BlackLagoon.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BlackLagoon.Controllers
{
    
    public class BookingController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public BookingController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [Authorize]
        public IActionResult FinalizeBooking(int villaId,DateOnly checkInDate,int nights)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ApplicationUser user = _unitOfWork.Users.Get(u => u.Id == userId);

            Booking booking = new Booking()
            {
                VillaId = villaId,
                CheckInDate = checkInDate,
                Nights = nights,
                Villa=_unitOfWork.Villas.Get(u=>u.Id==villaId,includeProperties: "VillaAmenity"),
                CheckOutDate = checkInDate.AddDays(nights),
                UserId=userId,
                Phone = user.PhoneNumber,
                Email = user.Email,
                Name = user.NameOfUser

            };
            booking.TotalCost = booking.Villa.Price * nights;
            return View(booking);
        }
        [HttpPost]
        [Authorize]
        public IActionResult FinalizeBooking(Booking booking)
        {
           

            var villa = _unitOfWork.Villas.Get(u => u.Id == booking.VillaId);
            
            booking.TotalCost=villa.Price*booking.Nights;
            booking.Status = SD.StatusPending;
            booking.BookingDate = DateTime.Now;

            _unitOfWork.Bookings.Add(booking);
            _unitOfWork.Save();
            return RedirectToAction(nameof(BookingConfirmation),new {bookingId = booking.Id});
        }
       
        [Authorize]
        public IActionResult BookingConfirmation(int bookingId)
        {


            
            return View(bookingId);
        }
    }
}
