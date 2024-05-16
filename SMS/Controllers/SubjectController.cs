/// <summary>
/// 
/// </summary>
/// <author>Sadakshini</author>
using SMS.BL.Subject;
using SMS.Data;
using SMS.ViewModel.Subject;
using SMS.Models.Subject;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SMS.Controllers
{
    public class SubjectController : Controller
    {
        private readonly SubjectBL _subjectBL = new SubjectBL();
        // GET: Subject
        public ActionResult Index()
        {
            var result = new SubjectViewModel();
            result.SubjectList = _subjectBL.GetAllSubject();
            return View(result);
        }
        /// <summary>
        /// Get subject according to the filter value
        /// </summary>
        /// <param name="isEnable"></param>
        /// <returns></returns>
        public ActionResult GetSubject(bool? isEnable = null)
        {
            _subjectBL.Configuration.ProxyCreationEnabled = false;

            // Get all subjects or filter based on the isEnable parameter
            IQueryable<Subject> query = _subjectBL.Subjects;
            if (isEnable.HasValue)
            {
                query = query.Where(s => s.IsEnable == isEnable.Value);
            }

            var sublist = query.ToList();
            return Json(new { data = sublist }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Add Subject and edit existing subject
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        [HttpGet]
        public ActionResult AddOrEdit(int id = 0)
        {
            if (id == 0)
            {
                return PartialView("_AddOrEdit", new SubjectBO());
            }
            else
            {
                var subject = _subjectBL.GetSubjectByID(id);
                if (subject == null)
                {
                    return HttpNotFound();
                }
                return PartialView("_AddOrEdit", subject);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddOrEdit(SubjectBO subject)
        {
            if (ModelState.IsValid)
            {
                string message;

                // Check if the subject code already exists
                var existingSubjectCode = _subjectBL.Subjects.Any(s => s.SubjectID != subject.SubjectID && s.SubjectCode == subject.SubjectCode);
                if (existingSubjectCode)
                {
                    return Json(new { success = false, message = "SubjectCode already exists" });
                }

                // Check if the subject name already exists
                var existingSubjectName = _subjectBL.Subjects.Any(s => s.SubjectID != subject.SubjectID && s.Name == subject.Name);
                if (existingSubjectName)
                {
                    return Json(new { success = false, message = "SubjectName already exists" });
                }

                // Save or update the subject
                if (subject.SubjectID == 0)
                {
                    if (_subjectBL.SaveSubject(subject, out message))
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
                    if (_subjectBL.SaveSubject(subject, out message))
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
                return Json(new { success = false, message = string.Join("<br>", errors) });
            }
        }

        /// <summary>
        /// Delete method
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        [HttpPost]
        public ActionResult Delete(int id)
        {
            Subject subject = _subjectBL.Subjects.Include("Teacher_Subject_Allocation").FirstOrDefault(x => x.SubjectID == id);

            if (subject == null)
            {
                return HttpNotFound();
            }

            // Check if the student is referenced in any related entities
            if (subject.Teacher_Subject_Allocation.Any())
            {
                return Json(new { success = false, message = "Cannot delete "+subject.Name+" because it is referenced in other entities" }, JsonRequestBehavior.AllowGet);
            }

            _subjectBL.Subjects.Remove(subject);
            _subjectBL.SaveChanges();

            return Json(new { success = true, message = "Deleted Successfully" }, JsonRequestBehavior.AllowGet);
        }

        
        /// <summary>
        /// STatus button enable disable
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ToggleEnable(int id)
        {
            Subject subject = _subjectBL.Subjects.Include("Teacher_Subject_Allocation").FirstOrDefault(x => x.SubjectID == id);

            if (subject == null)
            {
                return HttpNotFound();
            }

            bool currentStatus = subject.IsEnable;

            // If the current status is enabled, check if the teacher is referenced in any related entities
            if (currentStatus && subject.Teacher_Subject_Allocation.Any())
            {
                return Json(new { success = false, message = "Cannot change status because " + subject.Name + " is referenced in other entities" });
            }

            // Toggle the enable status
            subject.IsEnable = !currentStatus;
            _subjectBL.SaveChanges();

            string message = currentStatus ? "Disabled Successfully" : "Enabled Successfully";

            return Json(new { success = true, message = message }, JsonRequestBehavior.AllowGet);
        }
    }
}