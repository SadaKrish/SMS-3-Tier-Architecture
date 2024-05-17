
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
        public ActionResult SubjectAllocation()
        { 
            var result = new AllocationViewModel();
           // result.Teacher_Subject_AllocationList = _allocationBL.GetAllSubjectAllocation();
            return View(result);
        }

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
        public ActionResult AddOrEdit(long id=0)
        {
            // Populate dropdown lists for teachers and subjects
            ViewBag.TeacherList = new SelectList(_allocationBL.Teachers.Where(t => t.IsEnable), "TeacherID", "DisplayName");
            ViewBag.SubjectList = new SelectList(_allocationBL.Subjects.Where(s => s.IsEnable), "SubjectID", "Name");

            if (id ==0) // Add new allocation
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
                        success = _allocationBL.SaveSubjectAllocation(subjectAllocation, out message); // Call BL method
                    }
                    else // Edit existing allocation
                    {
                        // Update the existing allocation
                        success = _allocationBL.SaveSubjectAllocation(subjectAllocation, out message); // Call BL method
                    }

                    // Return JSON response based on success or failure
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

        public ActionResult GetAllStudentAllocation()
        {
            var allAllocatedStudent = _allocationBL.GetAllStudentAllocation();

            if (allAllocatedStudent != null)
            {
                return Json(new { success = true, data = allAllocatedStudent }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, message = "No Data Found" }, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult AddOrEditStudent(long id = 0)
        {
            ViewBag.StudentList = new SelectList(_allocationBL.Students.Where(s => s.IsEnable), "StudentID", "DisplayName");

            var subjectList = _allocationBL.Teacher_Subject_Allocation
                .Select(ts => ts.Subject)
                .Distinct()
                .ToList();

            ViewBag.SubjectList = new SelectList(subjectList, "SubjectID", "Name");

            if (id == 0) // Add new allocation
            {
                return PartialView("_AddStudentSubjectTeacherAllocation", new Student_Subject_Teacher_AllocationBO());
            }
            else // Edit existing allocation
            {
                var studentAllocation = _allocationBL.GetStudentAllocationById(id);
                if (studentAllocation == null)
                {
                    return HttpNotFound();
                }

                // Populate the ViewBag with the subject and teacher details for the dropdowns
                ViewBag.SubjectID = studentAllocation.SubjectID;
                ViewBag.TeacherID = studentAllocation.TeacherID;

                return PartialView("_AddStudentSubjectTeacherAllocation", studentAllocation);
            }
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

                    success = _allocationBL.SaveStudentAllocation(allocation, out message);

                    return Json(new { success, message });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "An error occurred while processing the request: " + ex.Message });
                }
            }

            ViewBag.StudentList = new SelectList(_allocationBL.Students, "StudentID", "DisplayName", allocation.StudentID);
            ViewBag.SubjectList = new SelectList(_allocationBL.Subjects, "SubjectID", "Name", allocation.SubjectAllocationID);
            ViewBag.TeacherList = new SelectList(_allocationBL.Teachers, "TeacherID", "DisplayName", allocation.TeacherID);

            return PartialView("_AddStudentSubjectTeacherAllocation", allocation);
        }


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
            try
            {
                string message;
                bool success = _allocationBL.DeleteStudentAllocation(id, out message); // Call BL method

                // Return JSON response based on success or failure
                return Json(new { success, message });
            }
            catch
            {
                return Json(new { success = false, message = "Error On delete" });
            }
        }
    }
}