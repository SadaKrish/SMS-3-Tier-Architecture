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
using SMS.BL.Subject.Interface;
using SMS.BL.Allocation;

namespace SMS.Controllers
{
    public class SubjectController : Controller
    {
        //private readonly SubjectBL _subjectBL = new SubjectBL();
        private readonly ISubjectRepository _subjectRepository;

        public SubjectController() 
        {
            _subjectRepository = new SubjectRepository();
        }
        // GET: Subject
        public ActionResult Index()
        {
            var result = new SubjectViewModel();
            // result.SubjectList = _subjectBL.GetAllSubject();
            result.SubjectList = _subjectRepository.GetSubjects();
            return View(result);
        }
        /// <summary>
        /// Get subject according to the filter value
        /// </summary>
        /// <param name="isEnable"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetSubjects(string status = "all")
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

            var subjects = _subjectRepository.GetSubjects(isEnabled);

            if (subjects != null && subjects.Any())
            {
                return Json(new { success = true, data = subjects }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = "No Data Found" }, JsonRequestBehavior.AllowGet);
            }
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
                var subject = _subjectRepository.GetSubjectByID(id); // Assume this method exists in your BL to fetch a subject by ID
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
                if (_subjectRepository.SubjectCodeExists(subject.SubjectID, subject.SubjectCode))
                {
                    return Json(new { success = false, message = "SubjectCode already exists" });
                }

                // Check if the subject name already exists
                if (_subjectRepository.SubjectNameExists(subject.SubjectID, subject.Name))
                {
                    return Json(new { success = false, message = "SubjectName already exists" });
                }

                if (_subjectRepository.SaveSubject(subject, out message))
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
                return Json(new { success = false, message = string.Join("<br>", errors) });
            }
        }


        /// <summary>
        /// Delete method
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        [HttpPost]
        public ActionResult Delete(long id)
        {
            string msg;
            var result = _subjectRepository.DeleteSubject(id, out msg);
            return Json(new { success = result, message = msg });
        }


        /// <summary>
        /// STatus button enable disable
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SearchSubjects(string searchText, string searchCategory)
        {
            try
            {
                var searchResults = _subjectRepository.SearchSubjects(searchText, searchCategory);
                return Json(new { success = true, data = searchResults });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

    }
}