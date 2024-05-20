
using SMS.BL.Allocation;
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
        private readonly AllocationBL _allocationBL = new AllocationBL();
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
            var allAllocatedSubject = _allocationBL.GetAllSubjectAllocations();

            if (allAllocatedSubject.Any())
            {
                return Json(new { success = true, data = allAllocatedSubject }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = "No Data Found" }, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: TeacherSubjectAllocation/AddOrEdit
        public ActionResult AddOrEdit(long id = 0)
        {
            // Populate dropdown lists for teachers and subjects
            ViewBag.TeacherList = new SelectList(_allocationBL.Teachers.Where(t => t.IsEnable), "TeacherID", "DisplayName");
            ViewBag.SubjectList = new SelectList(_allocationBL.Subjects.Where(s => s.IsEnable), "SubjectID", "Name");

            if (id == 0) // Add new allocation
            {
                return PartialView("_AddTeacherSubjectAllocation", new Teacher_Subject_AllocationBO()); // Change to Teacher_Subject_AllocationBO
            }
            else // Edit existing allocation
            {
                var subjectAllocation = _allocationBL.GetSubjectAllocationById(id); // Fetch allocation using BL method
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
                        success = _allocationBL.SaveSubjectAllocation(subjectAllocation, out message); 
                    }
                    else // Edit existing allocation
                    {
                        // Update the existing allocation
                        success = _allocationBL.SaveSubjectAllocation(subjectAllocation, out message); 
                    }

                    
                    return Json(new { success, message });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "An error occurred while processing the request: " + ex.Message });
                }
            }

            // If model state is not valid, re-populate ViewBag and return to the view
            ViewBag.TeacherList = new SelectList(_allocationBL.Teachers, "TeacherID", "DisplayName", subjectAllocation.TeacherID);
            ViewBag.SubjectList = new SelectList(_allocationBL.Subjects, "SubjectID", "Name", subjectAllocation.SubjectID);
            return PartialView("_AddTeacherSubjectAllocation", subjectAllocation);
        }

        [HttpPost]
        public ActionResult DeleteSubjectAllocation(long id)
        {
            string msg;
            var result = _allocationBL.DeleteSubjectAllocation(id, out msg);
            return Json(new { success = result, message = msg });
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
        [HttpGet]
        public ActionResult GetAllStudentAllocation(string status = "all")
        {
            bool? isEnabled = null;
            if (status.ToLower() == "enabled")
            {
                isEnabled = true;
            }
            else if (status.ToLower() == "disabled")
            {
                isEnabled = false;
            }

            var allAllocatedStudent = _allocationBL.GetAllStudentAllocation(isEnabled);

            if (allAllocatedStudent != null && allAllocatedStudent.Any())
            {
                return Json(new { success = true, data = allAllocatedStudent }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = "No Data Found" }, JsonRequestBehavior.AllowGet);
            }
        }




        /// <summary>
        /// Add and edit the student allocation
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult AddOrEditStudent()
        {
            // Populate the student list for the dropdown
            ViewBag.StudentList = new SelectList(_allocationBL.Students.Where(s => s.IsEnable), "StudentID", "DisplayName");

            // Populate the subject list for the dropdown
            var subjectList = _allocationBL.Teacher_Subject_Allocation
                .Select(ts => ts.Subject)
                .Distinct()
                .ToList();

            ViewBag.SubjectList = new SelectList(subjectList, "SubjectID", "Name");

            // Return the partial view for adding a new allocation
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
                    success = _allocationBL.SaveStudentAllocation(allocation, out message);

                    return Json(new { success, message });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "An error occurred while processing the request: " + ex.Message });
                }
            }

            // Repopulate the dropdown lists if the model state is not valid
            ViewBag.StudentList = new SelectList(_allocationBL.Students.Where(s => s.IsEnable), "StudentID", "DisplayName", allocation.StudentID);

            var subjectList = _allocationBL.Teacher_Subject_Allocation
                .Select(ts => ts.Subject)
                .Distinct()
                .ToList();

            ViewBag.SubjectList = new SelectList(subjectList, "SubjectID", "Name", allocation.SubjectAllocationID);

            // Return the partial view with the current model state
            return PartialView("_AddStudentSubjectTeacherAllocation", allocation);
        }


        /// <summary>
        /// Get the allocated teacher details according to subject ID
        /// </summary>
        /// <param name="subjectID"></param>
        /// <returns></returns>
        public ActionResult GetTeachersBySubjectID(int subjectID)
        {
            try
            {
                var teachers = _allocationBL.Teacher_Subject_Allocation
                                    .Where(ts => ts.SubjectID == subjectID)
                                    .Select(ts => new
                                    {
                                        TeacherID = ts.TeacherID,
                                        DisplayName = ts.Teacher.DisplayName
                                    })
                                    .Distinct()
                                    .ToList();

                return Json(teachers, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Log the exception for debugging purposes
                Console.WriteLine("An error occurred while retrieving teachers: " + ex.Message);
                throw; // Rethrow the exception to indicate a server error
            }
        }

        /// <summary>
        /// retrieve the SubjectALlocationID based on the sujectid and the teacherid
        /// </summary>
        /// <param name="subjectID"></param>
        /// <param name="teacherID"></param>
        /// <returns></returns>
        public JsonResult GetAllocationID(long subjectID, long teacherID)
        {
            try
            {
                // Assuming Teacher_Subject_Allocation has a mapping between subjects and teachers
                var allocationID = _allocationBL.Teacher_Subject_Allocation
                    .Where(allocation => allocation.SubjectID == subjectID && allocation.TeacherID == teacherID)
                    .Select(allocation => allocation.SubjectAllocationID)
                    .FirstOrDefault();

                return Json(allocationID, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Log the exception for debugging purposes
                Console.WriteLine("An error occurred while retrieving allocation ID: " + ex.Message);
                return Json(null); // Return null or handle the error as needed
            }
        }

        [HttpPost]
        public ActionResult DeleteStudentAllocation(long id)
        {
            string msg;
            var result = _allocationBL.DeleteStudentAllocation(id, out msg);
            return Json(new { success = result, message = msg });
        }
    }
}