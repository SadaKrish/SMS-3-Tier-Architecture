/// <summary>
/// 
/// </summary>
/// <author>Sadakshini</author>
using SMS.BL.Student;
using SMS.BL.Student.Interface;
using SMS.BL.Teacher;
using SMS.BL.Teacher.Interface;
using SMS.Data;
using SMS.Models.Subject;
using SMS.Models.Teacher;
using SMS.ViewModel.Teacher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SMS.Controllers
{
    
    public class TeacherController : Controller
    {
        //private readonly TeacherBL _teacherBL = new TeacherBL();
        private readonly ITeacherRepository _teacherRepository;

        public TeacherController()
        {
            _teacherRepository = new TeacherRepository();
        }
        // GET: Teacher
        public ActionResult Index()
        {
            var result = new TeacherViewModel();
            result.TeacherList = _teacherRepository.GetTeachers();
            return View(result);
        }

        /// <summary>
        /// Get Teacher details
        /// </summary>
        /// <param name="isEnable"></param>
        /// <returns></returns>

        [HttpGet]
        public ActionResult GetTeachers(string status = "all")
        {
            bool? isEnabled = null;
            if (status.ToLower() == "active")
            {
                isEnabled = true;
            }
            else if (status.ToLower() == "inactive")
            {
                isEnabled = false;
            }

            var teachers = _teacherRepository.GetTeachers(isEnabled);

            if (teachers != null && teachers.Any())
            {
                return Json(new { success = true, data = teachers }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = "No Data Found" }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Add and edit existing teacher details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        [HttpGet]
        public ActionResult AddOrEdit(int id = 0)
        {
            if (id == 0)
            {
                return PartialView("_AddOrEdit", new TeacherBO());
            }
            else
            {
                var teacher = _teacherRepository.GetTeacherByID(id);
                if (teacher == null)
                {
                    return HttpNotFound();
                }
                return PartialView("_AddOrEdit", teacher);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddOrEdit(TeacherBO teacher)
        {
            if (ModelState.IsValid)
            {
                string message;

                // Check if the subject code already exists
                if (_teacherRepository.TeacherRegNoExists(teacher.TeacherID,teacher.TeacherRegNo))
                {
                    return Json(new { success = false, message = "Teacher Reg No already exists" });
                }

                // Check if the subject name already exists
                if (_teacherRepository.TeacherDisplayNameExists(teacher.TeacherID, teacher.DisplayName))
                {
                    return Json(new { success = false, message = "Display Name already exists" });
                }

                if (_teacherRepository.SaveTeacher(teacher, out message))
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
                var errors = ModelState.Where(ms => ms.Value.Errors.Any())
                               .Select(ms => new
                               {
                                   Key = ms.Key,
                                   Errors = ms.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                               });

                return Json(new { success = false, errors });
            }
        }


        /// <summary>
        /// Delete the teacher details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Delete(long id)
        {
            string msg;
            var result = _teacherRepository.DeleteTeacher(id, out msg);
            return Json(new { success = result, message = msg });
        }

        /// <summary>
        /// cahnging the ststau of the teacher
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ToggleEnable(int id)
        {
            string message;
            bool success = _teacherRepository.ToggleTeacherEnable(id, out message);

            if (!success)
            {
                return Json(new { success = false, message = message });
            }

            return Json(new { success = true, message = message }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Search studnets
        /// </summary>
        /// <param name="searchText"></param>
        /// <param name="searchCategory"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SearchTeachers(string searchText, string searchCategory)
        {
            try
            {
                var searchResults = _teacherRepository.SearchTeachers(searchText, searchCategory);
                return Json(new { success = true, data = searchResults });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

    }
}