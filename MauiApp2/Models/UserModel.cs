using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp2.Models
{
    public class UserModel
    {
        public string UserId { get; set; }
        [Required(ErrorMessage = "First name is required")]
        public string UserFirstName { get; set; }
        [Required(ErrorMessage = "Last name is required")]
        public string UserLastName { get; set; }
        [Required, EmailAddress]
        public string UserEmail { get; set; }
        public string UserName { get; set; }
        public string UserMobile { get; set; }
        public string UserAddress { get; set; }

        public string UserPhone { get; set; } 
        public string UserRoleId { get; set; }
        public string UserCountryCode { get; set; } 
        public int UserStatus { get; set; }
        public string UserRoleName { get; set; } = string.Empty;
        
        public int UserCompanyId { get; set; }
        public int? UserBranchId { get; set; }
        public string? UserGender { get; set; }
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
