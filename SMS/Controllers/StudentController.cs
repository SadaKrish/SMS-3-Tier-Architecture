/// <summary>
/// 
/// </summary>
/// <author>Sadakshini</author>
using SMS.BL.Student;
using SMS.BL.Student.Interface;
using SMS.BL.Subject;
using SMS.BL.Subject.Interface;
using SMS.Data;
using SMS.Models.Student;
using SMS.Models.Subject;
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
        //private readonly StudentBL _studentBL = new StudentBL();
        private readonly IStudentRepository _studentRepository;

        public StudentController()
        {
            _studentRepository = new StudentRepository();
        }
        public ActionResult Index()
        {
            var result = new StudentViewModel();
            result.StudentList = _studentRepository.GetAllStudent();
            return View(result);
        }
        /// <summary>
        /// Get Students accroding to the filter value
        /// </summary>
        /// <param name="isEnable"></param>
        /// <returns></returns>
        public ActionResult GetStudents(bool? isEnable = null)
        {
            var students = _studentRepository.GetStudents(isEnable);
            return Json(new { data = students }, JsonRequestBehavior.AllowGet);
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
                var student = _studentRepository.GetStudentByID(id);
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

                // Retrieve the existing student to compare values
                var existingStudent = _studentRepository.GetStudentByID(student.StudentID);

                // Check if the registration no already exists
                if (existingStudent == null || existingStudent.StudentRegNo != student.StudentRegNo)
                {
                    if (_studentRepository.StudentRegNoExists(student.StudentID,student.StudentRegNo))
                    {
                        return Json(new { success = false, message = "Registration No already exists" });
                    }
                }

                // Check if the display name already exists
                if (existingStudent == null || existingStudent.DisplayName != student.DisplayName)
                {
                    if (_studentRepository.StudentDisplayNameExists(student.StudentID, student.DisplayName))
                    {
                        return Json(new { success = false, message = "Display name already exists" });
                    }
                }

                // Save or update the student
                if (_studentRepository.SaveStudent(student, out message))
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
            string message;
            bool requiresConfirmation;

            var success = _studentRepository.DeleteStudent(id, out message, out requiresConfirmation);

            if (success)
            {
                return Json(new { success = true, message = message }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, requiresConfirmation = requiresConfirmation, message = message }, JsonRequestBehavior.AllowGet);
            }
        }
        /// <summary>
        /// Is Enable button toggle
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ToggleEnable(int id)
        {
            string message;
            var success = _studentRepository.ToggleStudentEnable(id, out message);

            if (success)
            {
                return Json(new { success = true, message = message }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return HttpNotFound();
            }
        }


    }
}