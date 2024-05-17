﻿using SMS.BL.Subject;
using SMS.Data;
using SMS.Models.Student_Allocation;
using SMS.Models.Subject;
using SMS.Models.Teacher;
using SMS.Models.Teacher_Subject_Allocation;
using SMS.ViewModel.Allocation;
using SMS.ViewModel.Subject;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations.Sql;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace SMS.BL.Allocation
{
    public class AllocationBL: sms_dbEntities
    {

       /// <summary>
       /// get the list for subjectallocation
       /// </summary>
       /// <returns></returns>

        public IEnumerable<SubjectAllocationDetailViewModel> GetAllSubjectAllocations()
        {
            var allSubjectAllocations = Teacher_Subject_Allocation
                .Include("Subject")
                .Include("Teacher")
                .ToList();

            if (allSubjectAllocations.Count > 0)
            {
                var data = allSubjectAllocations
                    .GroupBy(item => new { item.Teacher.TeacherRegNo, item.Teacher.DisplayName })
                    .Select(g => new SubjectAllocationDetailViewModel
                    {
                        TeacherRegNo = g.Key.TeacherRegNo,
                        DisplayName = g.Key.DisplayName,
                        Subjects = g.Select(item => new SubjectAllocationViewModel
                        {
                            SubjectAllocationID = item.SubjectAllocationID,
                            SubjectCode = item.Subject.SubjectCode,
                            Name = item.Subject.Name
                        }).ToList()
                    }).ToList();

                return data;
            }
            return null;
        }





        /// <summary>
        /// get subject allocation by id
        /// </summary>
        /// <param name="subjectAllocationId"></param>
        /// <returns></returns>


        public Teacher_Subject_AllocationBO GetSubjectAllocationById(long subjectAllocationId)
        {
            var results = Teacher_Subject_Allocation.Select(s => new Teacher_Subject_AllocationBO()
            {
                SubjectAllocationID = s.SubjectAllocationID,
                TeacherID = s.SubjectID,
                SubjectID = s.SubjectID
            }).Where(s => s.SubjectAllocationID == subjectAllocationId).FirstOrDefault();

             return results;
          
        }
        /// <summary>
        /// add and edit subject allocation
        /// </summary>
        /// <param name="allocation"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool SaveSubjectAllocation(Teacher_Subject_AllocationBO allocation, out string message)
        {
            message = "";

            try
            {
                // Check if the allocation already exists
                bool allocationExists = Teacher_Subject_Allocation
                    .Any(a => a.TeacherID == allocation.TeacherID && a.SubjectID == allocation.SubjectID);

                if (allocationExists)
                {
                    message = "Allocation for the selected teacher and subject already exists";
                    return false;
                }

                // Add new allocation
                var newAllocation = new SMS.Data.Teacher_Subject_Allocation();
                newAllocation.TeacherID = allocation.TeacherID;
                newAllocation.SubjectID = allocation.SubjectID;
                Teacher_Subject_Allocation.Add(newAllocation);
                SaveChanges();
                message = "Allocation added successfully";

                return true;
            }
            catch (Exception ex)
            {
                // Log the inner exception
                if (ex.InnerException != null)
                {
                    message = "An error occurred while processing the request: " + ex.InnerException.Message;
                }
                else
                {
                    message = "An error occurred while processing the request: " + ex.Message;
                }
                return false;
            }
        }


        /// <summary>
        /// delete subject allocation
        /// </summary>
        /// <param name="id"></param>
        /// <param name="msg"></param>
        /// <returns></returns>

        public bool DeleteSubjectAllocation(long id, out string msg)
        {
            msg = "";
            var allocation = Teacher_Subject_Allocation.SingleOrDefault(t => t.SubjectAllocationID == id);
            try
            {
                if (allocation != null)
                {
                    if (allocation.Student_Subject_Teacher_Allocation.Any())
                    {
                        msg = "Cannot delete the allocation as it is assigned to a student";
                        return false;
                    }
                    Teacher_Subject_Allocation.Remove(allocation);
                    SaveChanges();
                    msg = "Allocation has been deleted successfully!";
                    return true;
                }
                msg = "Allocation removed already!";
                return false;
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                return false;
            }
        }

        //**********************************Student ALlocation******************************>
        /// <summary>
        /// get the students and alocated subject details in grouping
        /// </summary>
        /// <returns></returns>

        public IEnumerable<StudentAllocationGroupedViewModel> GetAllStudentAllocation()
        {
            var allStudentAllocations = Student_Subject_Teacher_Allocation
                                            .Include("Student")
                                            .Include("Teacher_Subject_Allocation.Teacher")
                                            .Include("Teacher_Subject_Allocation.Subject")
                                            .ToList();

            var groupedData = allStudentAllocations
                .GroupBy(item => new { item.Student.StudentRegNo, item.Student.DisplayName })
                .Select(group => new StudentAllocationGroupedViewModel
                {
                    StudentRegNo = group.Key.StudentRegNo,
                    DisplayName = group.Key.DisplayName,
                    StudentAllocations = group.Select(item => new StudentAllocationViewModel
                    {
                        StudentAllocationID = item.StudentAllocationID,
                        SubjectCode = item.Teacher_Subject_Allocation.Subject.SubjectCode,
                        Name = item.Teacher_Subject_Allocation.Subject.Name,
                        TeacherRegNo = item.Teacher_Subject_Allocation.Teacher.TeacherRegNo,
                        TeacherName = item.Teacher_Subject_Allocation.Teacher.DisplayName
                    }).ToList()
                });

            return groupedData;
        }

        //public IEnumerable<StudentAllocationGroupedViewModel> GetAllStudentAllocation()
        //{
        //    var allStudentAllocations = Student_Subject_Teacher_Allocation
        //                                    .Include("Student")
        //                                    .Include("Teacher_Subject_Allocation.Teacher")
        //                                    .Include("Teacher_Subject_Allocation.Subject")
        //                                    .ToList();

        //    var groupedData = allStudentAllocations
        //        .GroupBy(item => new { item.Student.StudentRegNo, item.Student.DisplayName })
        //        .Select(group => new StudentAllocationGroupedViewModel
        //        {
        //            StudentRegNo = group.Key.StudentRegNo,
        //            DisplayName = group.Key.DisplayName,
        //            StudentAllocations = group.Select(item => new StudentAllocationViewModel
        //            {
        //                StudentAllocationID = item.StudentAllocationID,
        //                SubjectCode = item.Teacher_Subject_Allocation.Subject.SubjectCode,
        //                Name = item.Teacher_Subject_Allocation.Subject.Name,
        //                TeacherRegNo = item.Teacher_Subject_Allocation.Teacher.TeacherRegNo,
        //                TeacherName = item.Teacher_Subject_Allocation.Teacher.DisplayName
        //            }).ToList()
        //        });

        //    return groupedData;
        //}



        /// <summary>
        /// /Get the studentallocation by ID
        /// </summary>
        /// <param name="studentAllocationId"></param>
        /// <returns></returns>

        public Student_Subject_Teacher_AllocationBO GetStudentAllocationById(long studentAllocationId)
        {
            var results = Student_Subject_Teacher_Allocation.Select(s => new Student_Subject_Teacher_AllocationBO()
            {
                StudentAllocationID=s.StudentAllocationID,
                StudentID=s.StudentID,
                SubjectAllocationID = s.SubjectAllocationID
                
            }).Where(s => s.StudentAllocationID == studentAllocationId).FirstOrDefault();

            return results;

        }
        /// <summary>
        /// add and edit the allocation
        /// </summary>
        /// <param name="allocationBO"></param>
        /// <param name="message"></param>
        /// <returns></returns>

        public bool SaveStudentAllocation(Student_Subject_Teacher_AllocationBO studentAllocation, out string msg)
        {
            msg = "";

            bool isExistingStudentAllocation = Student_Subject_Teacher_Allocation.Any(s => s.StudentAllocationID == studentAllocation.StudentAllocationID);

            var editStudenttAllocation = Student_Subject_Teacher_Allocation.SingleOrDefault(s => s.StudentAllocationID == studentAllocation.StudentAllocationID);

            bool isStudentAllocated = Student_Subject_Teacher_Allocation.Any(s => s.SubjectAllocationID == studentAllocation.SubjectAllocationID && s.StudentID == studentAllocation.StudentID);



            try
            {
                if (studentAllocation.SubjectAllocationID == 0)
                {
                    msg = "Please Fill All Details";
                    return false;
                }

                if (isStudentAllocated)
                {
                    msg = "This Allocation Already Exists.";
                    return false;
                }

                if (isExistingStudentAllocation)
                {


                    if (editStudenttAllocation == null)
                    {
                        msg = "Unable to find the subject allocation for edit";
                        return false;
                    }

                    editStudenttAllocation.SubjectAllocationID = studentAllocation.SubjectAllocationID;

                    SaveChanges();
                    msg = "Allocation Details Updated Successfully!";
                    return true;

                }


                var newStudentAllocation = new SMS.Data.Student_Subject_Teacher_Allocation();
                newStudentAllocation.StudentID = studentAllocation.StudentID;
                newStudentAllocation.SubjectAllocationID = studentAllocation.SubjectAllocationID;



                Student_Subject_Teacher_Allocation.Add(newStudentAllocation);
                SaveChanges();
                msg = "Student Allocation Added Successfully!";
                return true;
            }
            catch (Exception error)
            {
                msg = error.Message;
                return false;
            }

        }





        /// <summary>
        /// Delete allocation independently
        /// </summary>
        /// <param name="id"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool DeleteStudentAllocation(long id, out string msg)
        {
            msg = "";
            var studentAllocation = Student_Subject_Teacher_Allocation.SingleOrDefault(t => t.StudentAllocationID == id);
            try
            {
                if (studentAllocation != null)
                {

                    Student_Subject_Teacher_Allocation.Remove(studentAllocation);
                    SaveChanges();
                    msg = "Allocation has been deleted successfully!";
                    return true;

                }
                msg = "Allocation removed already!";
                return false;
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                return false;
            }
        }
    }
}
