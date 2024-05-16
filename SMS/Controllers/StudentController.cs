/// <summary>
/// 
/// </summary>
/// <author>Sadakshini</author>
using SMS.BL.Student;
using SMS.Data;
using SMS.Models.Student;
using SMS.ViewModel.Student;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SMS.Controllers
{
    public class StudentController : Controller
    {
        // GET: Student
        private readonly StudentBL _studentBL = new StudentBL();
        // GET: Teacher
        public ActionResult Index()
        {
            var result = new StudentViewModel();
            result.StudentList = _studentBL.GetAllStudent();
            return View(result);
        }
        /// <summary>
        /// Get Students accroding to the filter value
        /// </summary>
        /// <param name="isEnable"></param>
        /// <returns></returns>
        public ActionResult GetStudent(bool? isEnable = null)
        {
            _studentBL.Configuration.ProxyCreationEnabled = false;

            // Get all subjects or filter based on the isEnable parameter
            IQueryable<Student> query = _studentBL.Students;
            if (isEnable.HasValue)
            {
                query = query.Where(s => s.IsEnable == isEnable.Value);
            }

            var studentlist = query.ToList();
            return Json(new { data = studentlist }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Add and edit
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AddOrEdit(int id = 0)
        {
            if (id == 0)
            {
                return PartialView("_AddOrEdit", new StudentBO());
            }
            else
            {
                var student = _studentBL.GetStudentByID(id);
                if (student == null)
                {
                    return HttpNotFound();
                }
                return PartialView("_AddOrEdit", student);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddOrEdit(StudentBO student)
        {
            if (ModelState.IsValid)
            {
                string message;

                // Check if the registration no already exists
                var existingStudentRegNo = _studentBL.Students.Any(s => s.StudentID != student.StudentID && s.StudentRegNo == student.StudentRegNo);
                if (existingStudentRegNo)
                {
                    return Json(new { success = false, message = "Registration No already exists" });
                }

                // Check if the display name already exists
                var existingStudentDisplayName = _studentBL.Students.Any(s => s.StudentID != student.StudentID && s.DisplayName == student.DisplayName);
                if (existingStudentDisplayName)
                {
                    return Json(new { success = false, message = "Display name already exists" });
                }

                // Save or update the teacher
                if (student.StudentID == 0)
                {
                    if (_studentBL.SaveStudent(student, out message))
                    {
                        return Json(new { success = true, message = message });
                    }
                    else
                    {
                        return Json(new { success = false, message = message });
                    }
                }
                else
                {
                    if (_studentBL.SaveStudent(student, out message))
                    {
                        return Json(new { success = true, message = message });
                    }
                    else
                    {
                        return Json(new { success = false, message = message });
                    }
                }
            }
            else
            {
                // If model state is not valid, return validation errors
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return Json(new { success = false, message = string.Join(",", errors) });
            }
        }


        /// <summary>
        /// Delete Student
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Delete(int id)
        {
            Student student = _studentBL.Students.Include("Student_Subject_Teacher_Allocation").FirstOrDefault(x => x.StudentID == id);

            if (student == null)
            {
                return HttpNotFound();
            }

            // Check if the student is referenced in any related entities
            if (student.Student_Subject_Teacher_Allocation.Any())
            {
                // Student is following a subject, return a response indicating that confirmation is required
                return Json(new { success = false, requiresConfirmation = true, message = student.DisplayName+" is following a course." }, JsonRequestBehavior.AllowGet);
            }

            // If the student is not following any subjects, proceed with deletion
            _studentBL.Students.Remove(student);
            _studentBL.SaveChanges();

            return Json(new { success = true, message = "Deleted Successfully" }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Is Enable button toggle
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ToggleEnable(int id)
        {
            Student student = _studentBL.Students.Include("Student_Subject_Teacher_Allocation").FirstOrDefault(x => x.StudentID == id);

            if (student == null)
            {
                return HttpNotFound(); // or return some appropriate error response
            }

            bool currentStatus = student.IsEnable;

            // If the current status is enabled, check if the teacher is referenced in any related entities
            if (currentStatus && student.Student_Subject_Teacher_Allocation.Any())
            {
                return Json(new { success = false, message = "Cannot change status because " + student.DisplayName + " is referenced in other entities" });
            }

            // Toggle the enable status
            student.IsEnable = !currentStatus;
            _studentBL.SaveChanges();

            string message = currentStatus ? "Disabled Successfully" : "Enabled Successfully";

            return Json(new { success = true, message = message }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetStudentDisplayName(long studentID)
        {
            var teacher = _studentBL.GetStudentByID(studentID);
            if (teacher != null)
            {
                return Json(new { success = true, displayName = teacher.DisplayName }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = "Teacher not found" }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}