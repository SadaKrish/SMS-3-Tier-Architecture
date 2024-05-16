/// <summary>
/// 
/// </summary>
/// <author>Sadakshini</author>
using SMS.BL.Teacher;
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
        private readonly TeacherBL _teacherBL = new TeacherBL();
        // GET: Teacher
        public ActionResult Index()
        {
            var result = new TeacherViewModel();
            result.TeacherList = _teacherBL.GetAllTeacher();
            return View(result);
        }

        public ActionResult GetTeacher(bool? isEnable = null)
        {
            _teacherBL.Configuration.ProxyCreationEnabled = false;

            // Get all subjects or filter based on the isEnable parameter
            IQueryable<Teacher> query = _teacherBL.Teachers;
            if (isEnable.HasValue)
            {
                query = query.Where(s => s.IsEnable == isEnable.Value);
            }

            var teacherlist = query.ToList();
            return Json(new { data = teacherlist }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult AddOrEdit(int id = 0)
        {
            if (id == 0)
            {
                return PartialView("_AddOrEdit", new TeacherBO());
            }
            else
            {
                var teacher = _teacherBL.GetTeacherByID(id);
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

                // Check if the registration no already exists
                var existingTeacherRegNo = _teacherBL.Teachers.Any(s => s.TeacherID != teacher.TeacherID && s.TeacherRegNo == teacher.TeacherRegNo);
                if (existingTeacherRegNo)
                {
                    return Json(new { success = false, message = "Registration No already exists" });
                }

                // Check if the display name already exists
                var existingTeacherDisplayName = _teacherBL.Teachers.Any(s => s.TeacherID != teacher.TeacherID && s.DisplayName == teacher.DisplayName);
                if (existingTeacherDisplayName)
                {
                    return Json(new { success = false, message = "Display name already exists" });
                }

                // Save or update the teacher
                if (teacher.TeacherID == 0)
                {
                    if (_teacherBL.SaveTeacher(teacher, out message))
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
                    if (_teacherBL.SaveTeacher(teacher, out message))
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



        [HttpPost]
        public ActionResult Delete(int id)
        {
            Teacher teacher = _teacherBL.Teachers.Include("Teacher_Subject_Allocation").FirstOrDefault(x => x.TeacherID == id);

            if (teacher == null)
            {
                return HttpNotFound();
            }

            // Check if the student is referenced in any related entities
            if (teacher.Teacher_Subject_Allocation.Any())
            {
                return Json(new { success = false, message = "Cannot delete "+teacher.DisplayName+" because it is referenced in other entities" }, JsonRequestBehavior.AllowGet);
            }

            _teacherBL.Teachers.Remove(teacher);
            _teacherBL.SaveChanges();

            return Json(new { success = true, message = "Deleted Successfully" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ToggleEnable(int id)
        {
            Teacher teacher = _teacherBL.Teachers.Include("Teacher_Subject_Allocation").FirstOrDefault(x => x.TeacherID == id);

            if (teacher == null)
            {
                return HttpNotFound(); // or return some appropriate error response
            }

            bool currentStatus = teacher.IsEnable;


            if (currentStatus && teacher.Teacher_Subject_Allocation.Any())
            {
                return Json(new { success = false, message = "Cannot change status because " + teacher.DisplayName + " is referenced in other entities" });
            }

            // Toggle the enable status
            teacher.IsEnable = !currentStatus;
            _teacherBL.SaveChanges();

            string message = currentStatus ? "Disabled Successfully" : "Enabled Successfully";

            return Json(new { success = true, message = message }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetTeacherDisplayName(long teacherID)
        {
            var teacher = _teacherBL.GetTeacherByID(teacherID);
            if (teacher != null)
            {
                return Json(new { success = true, displayName = teacher.DisplayName }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false,message="Teacher not found"}, JsonRequestBehavior.AllowGet);
            }
        }
    }
}