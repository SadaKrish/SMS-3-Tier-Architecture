/// <summary>
/// 
/// </summary>
/// <author>Sadakshini</author>
using SMS.BL.Allocation;
using SMS.BL.Allocation.Interface;
using SMS.Data;
using SMS.Models.Student_Allocation;
using SMS.Models.Teacher_Subject_Allocation;
using SMS.ViewModel.Allocation;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SMS.Controllers
{
    public class AllocationController : Controller
    {
        //private readonly AllocationBL _allocationBL = new AllocationBL();
        private readonly IAllocationRepository _allocationRepository;
        public AllocationController()
        {
            _allocationRepository = new AllocationRepository();
        }
        // GET: Allocation
        /// <summary>
        /// index page for subject allocaton
        /// </summary>
        /// <returns></returns>
        public ActionResult SubjectAllocation()
        { 
            var result = new AllocationViewModel();
           // result.Teacher_Subject_AllocationList = _allocationBL.GetAllSubjectAllocation();
            return View(result);
        }

        /// <summary>
        /// retrieve the details
        /// </summary>
        /// <returns></returns>
        public ActionResult GetAllSubjectAllocation()
        {
            var allAllocatedSubject = _allocationRepository.GetAllSubjectAllocations();

            if (allAllocatedSubject.Any())
            {
                return Json(new { success = true, data = allAllocatedSubject }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = "No Data Found" }, JsonRequestBehavior.AllowGet);
            }
        }
        /// <summary>
        /// Add method for subject allocation
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: TeacherSubjectAllocation/AddOrEdit
        public ActionResult AddOrEdit(long id = 0)
        {
            ViewBag.TeacherList = new SelectList(_allocationRepository.GetEnabledTeachersList(), "Value", "Text");
            ViewBag.SubjectList = new SelectList(_allocationRepository.GetEnabledSubjectList(), "Value", "Text");

            if (id == 0)
            {
                return PartialView("_AddTeacherSubjectAllocation", new Teacher_Subject_AllocationBO());
            }
            else
            {
                var subjectAllocation = _allocationRepository.GetSubjectAllocationById(id);
                if (subjectAllocation == null)
                {
                    return HttpNotFound();
                }
                return PartialView("_AddTeacherSubjectAllocation", subjectAllocation);
            }
        }

        // POST: TeacherSubjectAllocation/AddOrEdit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddOrEdit(Teacher_Subject_AllocationBO subjectAllocation)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string message;
                    bool success;
                    if (subjectAllocation.SubjectAllocationID == 0) // Add new allocation
                    {
                        success = _allocationRepository.SaveSubjectAllocation(subjectAllocation, out message); 
                    }
                    else // Edit existing allocation
                    {
                        // Update the existing allocation
                        success = _allocationRepository.SaveSubjectAllocation(subjectAllocation, out message); 
                    }

                    
                    return Json(new { success, message });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "An error occurred while processing the request: " + ex.Message });
                }
            }

            // If model state is not valid, re-populate ViewBag and return to the view
            ViewBag.TeacherList = new SelectList(_allocationRepository.GetEnabledTeachersList(), "TeacherID", "DisplayName", subjectAllocation.TeacherID);
            ViewBag.SubjectList = new SelectList(_allocationRepository.GetEnabledSubjectList(), "SubjectID", "Name", subjectAllocation.SubjectID);
            return PartialView("_AddTeacherSubjectAllocation", subjectAllocation);
        }

        /// <summary>
        ///Delete the subject allocation 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteSubjectAllocation(long id)
        {
            string msg;
            var result = _allocationRepository.DeleteSubjectAllocation(id, out msg);
            return Json(new { success = result, message = msg });
        }
        [HttpPost]
        public ActionResult SearchSubjectAllocations(string searchText, string searchCategory)
        {
            try
            {
                var searchResults = _allocationRepository.SearchSubjectAllocations(searchText, searchCategory);
                return Json(new { success = true, data = searchResults });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Student ALlocation page
        /// </summary>
        /// <returns></returns>
        public ActionResult StudentAllocation()
        {
            var result = new AllocationViewModel();
            // result.Teacher_Subject_AllocationList = _allocationBL.GetAllSubjectAllocation();
            return View(result);
        }
        /// <summary>
        /// retrieve the studnet details along with student's status
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetAllStudentAllocation(string status = "all")
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

            var allAllocatedStudent = _allocationRepository.GetAllStudentAllocation(isEnabled);
            
            if (allAllocatedStudent != null && allAllocatedStudent.Any())
            {
                return Json(new { success = true, data = allAllocatedStudent }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success =false, message = "No Data Found" }, JsonRequestBehavior.AllowGet);
            }
        }





        /// <summary>
        /// Add and edit the student allocation
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult AddOrEditStudent()
        {
            ViewBag.StudentList = _allocationRepository.GetStudentList();
            ViewBag.TeacherList = _allocationRepository.GetTeacherList();
            ViewBag.SubjectList = _allocationRepository.GetAllSubjectsFromAllocation();

            return PartialView("_AddStudentSubjectTeacherAllocation", new Student_Subject_Teacher_AllocationBO());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddOrEditStudent(Student_Subject_Teacher_AllocationBO allocation)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string message;
                    bool success;

                    // Save the new allocation
                    success = _allocationRepository.SaveStudentAllocation(allocation, out message);

                    return Json(new { success, message });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "An error occurred while processing the request: " + ex.Message });
                }
            }

            // Repopulate the dropdown lists if the model state is not valid
            ViewBag.StudentList = _allocationRepository.GetStudentList();
            ViewBag.TeacherList = _allocationRepository.GetTeacherList();
            ViewBag.SubjectList = _allocationRepository.GetAllSubjectsFromAllocation();

            // Return the partial view with the current model state
            return PartialView("_AddStudentSubjectTeacherAllocation", allocation);
        }




        /// <summary>
        /// Get the allocated teacher details according to subject ID
        /// </summary>
        /// <param name="subjectID"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetTeachersBySubjectID(int subjectID)
        {
            var teachers = _allocationRepository.GetTeachersBySubjectID(subjectID);
            return Json(teachers, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// retrieve the SubjectALlocationID based on the sujectid and the teacherid
        /// </summary>
        /// <param name="subjectID"></param>
        /// <param name="teacherID"></param>
        /// <returns></returns>
        //public JsonResult GetAllocationID(long subjectID, long teacherID)
        //{
        //    try
        //    {
        //        // Assuming Teacher_Subject_Allocation has a mapping between subjects and teachers
        //        var allocationID = _allocationRepository.Teacher_Subject_Allocation
        //            .Where(allocation => allocation.SubjectID == subjectID && allocation.TeacherID == teacherID)
        //            .Select(allocation => allocation.SubjectAllocationID)
        //            .FirstOrDefault();

        //        return Json(allocationID, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log the exception for debugging purposes
        //        Console.WriteLine("An error occurred while retrieving allocation ID: " + ex.Message);
        //        return Json(null); // Return null or handle the error as needed
        //    }
        //}

        [HttpPost]
        public ActionResult DeleteStudentAllocation(long id)
        {
            string msg;
            var result = _allocationRepository.DeleteStudentAllocation(id, out msg);
            return Json(new { success = result, message = msg });
        }

        /// <summary>
        /// Delete a complete allocation of a student
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //[HttpPost]
        //public ActionResult DeleteWholeStudentAllocation(string id)
        //{
        //    string msg;
        //    var result = _allocationRepository.DeleteWholeStudentAllocation(id, out msg);
        //    return Json(new { success = result, message = msg });
        //}

        /// <summary>
        /// Search in Student Allocation page
        /// </summary>
        /// <param name="searchText"></param>
        /// <param name="searchCategory"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SearchStudentAllocations(string searchText, string searchCategory)
        {
            try
            {
                var searchResults = _allocationRepository.SearchStudentAllocations(searchText, searchCategory);
                return Json(new { success = true, data = searchResults });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


    }
}