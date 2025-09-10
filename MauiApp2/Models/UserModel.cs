using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp2.Models
{
    public class UserModel
    {
        public string UserId { get; set; }
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
        public string UserEmail { get; set; }
        public string UserName { get; set; }
        public string UserMobile { get; set; }
        public string UserAddress { get; set; }

        public string UserPhone { get; set; } = "1234567890";
        public string UserRoleId { get; set; }
        public string UserCountryCode { get; set; } = "1";
        public int UserStatus { get; set; }
        public string UserRoleName { get; set; } = string.Empty;
    }
    public class UserResponse
    {
        public int page { get; set; }
        public int pageSize { get; set; }
        public int totalRecords { get; set; }
        public int totalPages { get; set; }
        public List<UserModel> data { get; set; } = new();
    }
}
