using BlackLagoon.Application.Common.Interfaces;
using BlackLagoon.Application.Common.Utility;
using BlackLagoon.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using System.Collections.Generic;
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
        public IActionResult FinalizeBooking(int villaId, DateOnly checkInDate, int nights)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ApplicationUser user = _unitOfWork.Users.Get(u => u.Id == userId);

            Booking booking = new Booking()
            {
                VillaId = villaId,
                CheckInDate = checkInDate,
                Nights = nights,
                Villa = _unitOfWork.Villas.Get(u => u.Id == villaId, includeProperties: "VillaAmenity"),
                CheckOutDate = checkInDate.AddDays(nights),
                UserId = userId,
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
            var villaNumbers = _unitOfWork.VillaNumbers.GetAll().ToList();
            var bookedVillas = _unitOfWork.Bookings.GetAll(u => u.Status == SD.StatusApproved || u.Status == SD.StatusCheckIn).ToList();
            
                int roomsAvaible = SD.VillaRoomsAvaibleCount(
                    villa.Id, villaNumbers, booking.CheckInDate, booking.Nights, bookedVillas);

            if (roomsAvaible == 0)
            {
                TempData["error"] = "No Rooms Avaible for the selected dates";
                return RedirectToAction(nameof(FinalizeBooking),
                    new
                    {
                        villaId=booking.Id,
                        checkInDate = booking.CheckInDate,
                        nights = booking.Nights
                    });
            }
            

            booking.TotalCost = villa.Price * booking.Nights;
            booking.Status = SD.StatusPending;
            booking.BookingDate = DateTime.Now;

            _unitOfWork.Bookings.Add(booking);
            _unitOfWork.Save();


            var domain = Request.Scheme + "://" + Request.Host.Value + "/";
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
            _unitOfWork.Bookings.UpdateStripePaymentId(booking.Id, session.Id, session.PaymentIntentId);
            _unitOfWork.Save();
            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);





        }

        [Authorize]
        public IActionResult BookingConfirmation(int bookingId)
        {
            Booking bookingfromDb = _unitOfWork.Bookings.Get(u => u.Id == bookingId,
                includeProperties: "User,Villa");
            if (bookingfromDb.Status == SD.StatusPending)
            {
                //PENDING ORDER
                var service = new SessionService();
                Session session = service.Get(bookingfromDb.StripeSessionId);
                if (session.PaymentStatus == "paid")
                {
                    _unitOfWork.Bookings.UpdateStatus(bookingfromDb.Id, SD.StatusApproved, 0);
                    _unitOfWork.Bookings.UpdateStripePaymentId(bookingfromDb.Id, session.Id, session.PaymentIntentId);
                    _unitOfWork.Save();
                }
            }


            return View(bookingId);

        }
        [Authorize]
        public IActionResult BookingDetails(int bookingId)
        {
            Booking bookingFromDb = _unitOfWork.Bookings
                .Get(u => u.Id == bookingId,
                includeProperties: "User,Villa");
            if (bookingFromDb.VillaNumber == 0 && bookingFromDb.Status == SD.StatusApproved)
            {
                List<int> avaibleVillaNumbers = AssignAvailebleVillaNumberByVilla(bookingFromDb.VillaId);
                bookingFromDb.VillaNumbers = _unitOfWork.VillaNumbers.GetAll(u => u.VillaId
                == bookingFromDb.VillaId && avaibleVillaNumbers.Any(x => x == u.Villa_Number)).ToList();
            }
            return View(bookingFromDb);

        }
        [HttpPost]
        [Authorize(Roles = SD.Role_Admin)]
        public IActionResult CheckIn(Booking booking)
        {
            _unitOfWork.Bookings.UpdateStatus(booking.Id, SD.StatusCheckIn, booking.VillaNumber);
            _unitOfWork.Save();
            TempData["success"] = "Check In Successful";
            return RedirectToAction(nameof(BookingDetails), new { bookingId = booking.Id });
        }
        [HttpPost]
        [Authorize(Roles = SD.Role_Admin)]
        public IActionResult CheckOut(Booking booking)
        {
            _unitOfWork.Bookings.UpdateStatus(booking.Id, SD.StatusCompletted, booking.VillaNumber);
            _unitOfWork.Save();
            TempData["success"] = "Check In Successful";
            return RedirectToAction(nameof(BookingDetails), new { bookingId = booking.Id });
        }
        [HttpPost]
        [Authorize(Roles = SD.Role_Admin)]
        public IActionResult CancelBooking(Booking booking)
        {
            _unitOfWork.Bookings.UpdateStatus(booking.Id, SD.StatusCancelled, 0);
            _unitOfWork.Save();
            TempData["success"] = "Cancell Successful";
            return RedirectToAction(nameof(BookingDetails), new { bookingId = booking.Id });
        }
        private List<int> AssignAvailebleVillaNumberByVilla(int villaId)
        {
            List<int> avaibleVillaNumbers = new();
            var villaNumbers = _unitOfWork.VillaNumbers.GetAll(u => u.VillaId == villaId);
            var checkedInVilla = _unitOfWork.Bookings
            .GetAll(u => u.VillaId == villaId
            && u.Status == SD.StatusCheckIn)
            .Select(u => u.VillaNumber);
            foreach (var villaNumber in villaNumbers)
            {
                if (!checkedInVilla.Contains(villaNumber.Villa_Number))
                {
                    avaibleVillaNumbers.Add(villaNumber.Villa_Number);
                }
            }
            return avaibleVillaNumbers; ;
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
            if (!string.IsNullOrEmpty(status))
            {
                bookings = bookings.Where(u => u.Status.ToLower().Equals(status.ToLower()));
            }
            return Json(new { data = bookings });

        }


        #endregion
    }

}
