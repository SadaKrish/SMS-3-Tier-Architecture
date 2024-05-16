﻿using SMS.BL.Subject;
using SMS.Data;
using SMS.Models.Student_Allocation;
using SMS.Models.Subject;
using SMS.Models.Teacher;
using SMS.Models.Teacher_Subject_Allocation;
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
        public IEnumerable<object> GetAllSubjectAllocation()
        {
            var allSubjectAllocations = Teacher_Subject_Allocation.Include("Subject").Include("Teacher").ToList();


            if (allSubjectAllocations.Count > 0)
            {
                var data = allSubjectAllocations.Select(item => new
                {
                    SubjectAllocationID = item.SubjectAllocationID,
                    SubjectCode = item.Subject.SubjectCode,
                    SubjectName = item.Subject.Name,
                    TeacherRegNo = item.Teacher.TeacherRegNo,
                    TeacherName = item.Teacher.DisplayName
                });

                return data;
            }
            return null;
        }


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

        public bool SaveSubjectAllocation(Teacher_Subject_AllocationBO allocation, out string message)
        {
            message = "";

            bool allocationExists = Teacher_Subject_Allocation
                                        .Any(a => a.TeacherID == allocation.TeacherID && a.SubjectID == allocation.SubjectID);

            try
            {
                if (allocationExists)
                {
                    var editAllocation = Teacher_Subject_Allocation.SingleOrDefault(s => s.SubjectAllocationID == allocation.SubjectAllocationID);
                    if (editAllocation == null)
                    {
                        message = "Unable to find the allocation for edit";
                        return false;
                    }
                    else 
                    {
                        editAllocation.TeacherID = allocation.TeacherID;
                        editAllocation.SubjectID = allocation.SubjectID;
                        SaveChanges();
                        message = "Allocation Updated successfully";
                        return true;
                    }
                }
                else
                {
                    var newAllocation = new SMS.Data.Teacher_Subject_Allocation();
                    newAllocation.TeacherID = allocation.TeacherID;
                    newAllocation.SubjectID = allocation.SubjectID;
                    Teacher_Subject_Allocation.Add(newAllocation);
                    SaveChanges();
                    message = "Allocation added successfully";
                    return true;
                }
            }
            catch (Exception ex)
            {
                message = "An error occurred while processing the request: " + ex.Message;
                return false;
            }
        }

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

        public IEnumerable<object> GetAllStudentAllocation()
        {
            var allStudentAllocations = Student_Subject_Teacher_Allocation
                                            .Include("Student")
                                            .Include("Teacher_Subject_Allocation.Teacher")
                                            .Include("Teacher_Subject_Allocation.Subject")
                                            .ToList();

            var data = allStudentAllocations.Select(item => new
            {
                StudentAllocationID = item.StudentAllocationID,
                StudentRegNo = item.Student.StudentRegNo,
                StudentName = item.Student.DisplayName,
                SubjectCode = item.Teacher_Subject_Allocation.Subject.SubjectCode,
                SubjectName = item.Teacher_Subject_Allocation.Subject.Name,
                TeacherRegNo = item.Teacher_Subject_Allocation.Teacher.TeacherRegNo,
                TeacherName = item.Teacher_Subject_Allocation.Teacher.DisplayName
            });

            return data;
        }

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
        //public IEnumerable<TeacherBO> GetEnabledTeachers(long id)
        //{
        //    var enabledTeacher=Teachers.Where(t=>t.TeacherID==id)
        //}
        public bool SaveStudentAllocation(Student_Subject_Teacher_AllocationBO allocation, out string message)
        {
            message = "";

            bool allocationExists = Student_Subject_Teacher_Allocation
                                        .Any(a => a.StudentID == allocation.StudentID && a.SubjectAllocationID==a.SubjectAllocationID);

            try
            {
                if (allocationExists)
                {
                    var editAllocation = Student_Subject_Teacher_Allocation.SingleOrDefault(s => s.StudentAllocationID == allocation.StudentAllocationID);
                    if (editAllocation == null)
                    {
                        message = "Unable to find the allocation for edit";
                        return false;
                    }
                    
                        editAllocation.StudentID = allocation.StudentID;
                        editAllocation.SubjectAllocationID = allocation.SubjectAllocationID;
                        SaveChanges();
                        message = "Allocation Updated successfully";
                        return true;
                    
                }
                else
                {
                    // Check if the student is already allocated to the same subject under another teacher
                    bool studentFollowsSubject = Student_Subject_Teacher_Allocation
                                .Any(a => a.StudentID == allocation.StudentID &&
                                          a.Teacher_Subject_Allocation.SubjectID == allocation.SubjectID);

                    if (studentFollowsSubject)
                    {
                        // If student already follows the subject under another teacher, return false with a message
                        message = "The student is already following the subject under another teacher";
                        return false;
                    }

                    // If no existing allocation, create a new one
                    var newAllocation = new SMS.Data.Student_Subject_Teacher_Allocation();
                    newAllocation.StudentID = allocation.StudentID;
                    newAllocation.SubjectAllocationID = allocation.SubjectAllocationID;
                    Student_Subject_Teacher_Allocation.Add(newAllocation);
                    SaveChanges();
                    message = "Allocation added successfully";
                    return true;
                }
            }
            catch (Exception ex)
            {
                message = "An error occurred while processing the request: " + ex.Message;
                return false;
            }
        }

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