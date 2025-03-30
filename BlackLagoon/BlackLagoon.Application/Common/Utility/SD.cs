using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackLagoon.Application.Common.Utility
{
   public  static class SD
    {
        // Roles
        public const string Role_Customer = "User";
        public const string Role_Admin = "Admin";

        // Booking Status
        public const string StatusPending = "Pending";
        public const string StatusApproved = "Approved";
        public const string StatusCheckIn= "CheckedIn";
        public const string StatusCompletted = "Completed";
        public const string StatusCancelled = "Cancelled";
        public const string StatusRefuned = "Refunded";

    }
}
