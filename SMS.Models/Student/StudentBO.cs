using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.Models.Student
{
    public class StudentBO
    {
        public long StudentID { get; set; }
        [Required, DisplayName("Registration No")]
        public string StudentRegNo { get; set; }
        [Required, DisplayName("First Name")]
        public string FirstName { get; set; }
        [DisplayName("Middle Name")]
        public string MiddleName { get; set; }
        [Required(ErrorMessage = "Display Name is required")]
        [DisplayName("Last Name")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Display Name is required")]
        [DisplayName("Display Name")]
        public string DisplayName { get; set; }
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Gender is required")]
        public string Gender { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "Date of Birth is required")]
        [DisplayName("DOB")]
        public DateTime DOB { get; set; }
        [Required(ErrorMessage = "Address is required")]
        public string Address { get; set; }
        [Required(ErrorMessage = "Contact no is required"), DisplayName("Contact No")]
        public string ContactNo { get; set; }
        [DisplayName("Status")]
        public bool IsEnable { get; set; }
        public string DisplayIsEnable
        {
            get
            {
                return (IsEnable) ? "yes" : "no";
            }
        }
    }
}
