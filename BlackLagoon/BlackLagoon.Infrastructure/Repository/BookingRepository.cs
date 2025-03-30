using BlackLagoon.Application.Common.Utility;
using BlackLagoon.Common.Interfaces;
using BlackLagoon.Domain.Entities;
using BlackLagoon.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackLagoon.Infrastructure.Repository
{
    public class BookingRepository : Repository<Booking>, IBookingRepository
    {

        private readonly ApplicationDbContext _db;

        public BookingRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }


        

        public void Update(Booking entity)
        {
            _db.Update(entity);
        }

        public void UpdateStatus(int bookingId, string bookingStatus)
        {
            var bookingFromDb = _db.Bookings.FirstOrDefault(m=>m.Id == bookingId);
            if (bookingFromDb != null)
            {
                bookingFromDb.Status = bookingStatus;
                if(bookingStatus==SD.StatusCheckIn)
                {
                    bookingFromDb.ActualCheckInDate = DateTime.Now;
                }
                if(bookingStatus == SD.StatusCompletted)
                {
                    bookingFromDb.ActualCheckOutDate = DateTime.Now;
                }
                
                
            }

        }

        public void UpdateStripePaymentId(int bookingId, string sessionId, string paymentId)
        {
            var bookingFromDb = _db.Bookings.FirstOrDefault(m => m.Id == bookingId);
            if (bookingFromDb != null)
            {
                if(!string.IsNullOrEmpty(sessionId))
                {
                    bookingFromDb.StripeSessionId = sessionId;
                }
                if(!string.IsNullOrEmpty(paymentId))
                {
                    bookingFromDb.StripePaymentIntentId = paymentId;
                    bookingFromDb.PaymentDue = DateTime.Now;
                    bookingFromDb.isPaymentSuccessfull = true;
                }

            }
        }
    }
}

