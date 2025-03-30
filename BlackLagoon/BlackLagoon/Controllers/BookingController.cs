using BlackLagoon.Application.Common.Interfaces;
using BlackLagoon.Application.Common.Utility;
using BlackLagoon.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
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
        public IActionResult Index()
        {
            return View();
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


            var domain= Request.Scheme + "://" + Request.Host.Value+"/";
            var options = new Stripe.Checkout.SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = domain + $"booking/bookingconfirmation?bookingId={booking.Id}",
                CancelUrl = domain + "booking/finalizebooking?villaId=" + booking.VillaId + "&checkInDate=" + booking.CheckInDate + "&nights=" + booking.Nights
            };

            options.LineItems.Add(new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    UnitAmount = (long)(booking.TotalCost * 100),
                    Currency = "usd",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = villa.Name,
                        //Images= new List<string> { domain+villa.ImageUrl }
                    }
                },
                Quantity = 1
            });
           
            var service = new SessionService();
            Session session = service.Create(options);
            _unitOfWork.Bookings.UpdateStripePaymentId(booking.Id, session.Id,session.PaymentIntentId);
            _unitOfWork.Save();
            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);




            
        }
       
        [Authorize]
        public IActionResult BookingConfirmation(int bookingId)
        {
            Booking bookingfromDb = _unitOfWork.Bookings.Get(u => u.Id == bookingId,
                includeProperties:"User,Villa");
            if(bookingfromDb.Status == SD.StatusPending)
            {
                //PENDING ORDER
                var service = new SessionService();
                Session session = service.Get(bookingfromDb.StripeSessionId);
                if (session.PaymentStatus=="paid")
                {
                    _unitOfWork.Bookings.UpdateStatus(bookingfromDb.Id, SD.StatusApproved);
                    _unitOfWork.Bookings.UpdateStripePaymentId(bookingfromDb.Id,session.Id, session.PaymentIntentId);
                    _unitOfWork.Save();
                }
            }


            return View(bookingId);
           
        }
        [Authorize]
            public IActionResult BookingDetails(int bookingId)
        {
            Booking bookingFromDb= _unitOfWork.Bookings
                .Get(u => u.Id == bookingId,
                includeProperties: "User,Villa");
            return View(bookingFromDb);

        }
        #region API CALLS
        [Authorize]
        [HttpGet]
        public IActionResult GetAll(string status)
        {
            IEnumerable<Booking> bookings = new List<Booking>();
            if (User.IsInRole(SD.Role_Admin))
            {
                bookings = _unitOfWork.Bookings.GetAll(includeProperties: "User,Villa");
            }
            else
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
                bookings = _unitOfWork.Bookings.GetAll(u => u.UserId == userId, includeProperties: "User,Villa");
            }
            if(!string.IsNullOrEmpty(status))
            {
                bookings = bookings.Where(u => u.Status.ToLower().Equals(status.ToLower()));
            }
            return Json(new { data = bookings });

        }
        #endregion
    }

}
