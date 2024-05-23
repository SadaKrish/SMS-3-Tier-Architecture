/// <summary>
/// 
/// </summary>
/// <author>Sadakshini</author>
using SMS.BL.Allocation.Interface;
using SMS.Data;
using SMS.Models.Student_Allocation;
using SMS.Models.Teacher_Subject_Allocation;
using SMS.ViewModel.Allocation;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SMS.BL.Allocation
{
    public class AllocationRepository: IAllocationRepository
    {
        private readonly SMS_DBEntities _dbEntities;

        public AllocationRepository()
        {
            _dbEntities = new SMS_DBEntities();
        }
        public AllocationRepository(SMS_DBEntities dbEntities) 
        {
            _dbEntities = dbEntities;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SubjectAllocationDetailViewModel> GetAllSubjectAllocations()
        {
            var allSubjectAllocations = _dbEntities.Teacher_Subject_Allocation
                .Include("Subject")
                .Include("Teacher")
                .ToList();

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


        public Teacher_Subject_AllocationBO GetSubjectAllocationById(long subjectAllocationId)
        {
            var results = _dbEntities.Teacher_Subject_Allocation.Select(s => new Teacher_Subject_AllocationBO()
            {
                SubjectAllocationID = s.SubjectAllocationID,
                TeacherID = s.SubjectID,
                SubjectID = s.SubjectID
            }).Where(s => s.SubjectAllocationID == subjectAllocationId).FirstOrDefault();

            return results;

        }
        public IEnumerable<SelectListItem> GetEnabledTeachersList()
        {
            return _dbEntities.Teachers
                .Where(s => s.IsEnable)
                .Select(s => new SelectListItem
                {
                    Value = s.TeacherID.ToString(),
                    Text = s.TeacherRegNo + " - " + s.DisplayName
                });
        }

        public IEnumerable<SelectListItem> GetEnabledSubjectList()
        {
            return _dbEntities.Subjects
                .Where(s => s.IsEnable)
                .Select(s => new SelectListItem
                {
                    Value = s.SubjectID.ToString(),
                    Text = s.SubjectCode + " - " + s.Name
                });
        }

        public bool SaveSubjectAllocation(Teacher_Subject_AllocationBO allocation, out string message)
        {
            message = "";

            try
            {
                // Check if the allocation already exists
                bool allocationExists = _dbEntities.Teacher_Subject_Allocation
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
                _dbEntities.Teacher_Subject_Allocation.Add(newAllocation);
                _dbEntities.SaveChanges();
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

        public bool DeleteSubjectAllocation(long id, out string msg)
        {
            msg = "";
            var allocation = _dbEntities.Teacher_Subject_Allocation.SingleOrDefault(t => t.SubjectAllocationID == id);
            try
            {
                if (allocation != null)
                {
                    if (allocation.Student_Subject_Teacher_Allocation.Any())
                    {
                        msg = "Cannot delete the allocation as it is assigned to a student";
                        return false;
                    }
                    _dbEntities.Teacher_Subject_Allocation.Remove(allocation);
                    _dbEntities.SaveChanges();
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

        public IEnumerable<SubjectAllocationDetailViewModel> SearchSubjectAllocations(string searchText, string searchCategory)
        {
            var students = GetAllSubjectAllocations();


            if (searchCategory == "TeacherRegNo")
            {
                students = students.Where(a => a.TeacherRegNo.ToUpper().Contains(searchText.ToUpper())).ToList();
            }
            else if (searchCategory == "TeacherName")
            {
                students = students.Where(a => a.DisplayName.ToUpper().Contains(searchText.ToUpper())).ToList();
            }

            else if (searchCategory == "SubjectCode")
            {
                students = students.Where(a => a.Subjects.Any(t => t.SubjectCode.ToUpper().Contains(searchText.ToUpper()))).ToList();
            }
            else if (searchCategory == "SubjectName")
            {
                students = students.Where(a => a.Subjects.Any(t => t.Name.ToUpper().Contains(searchText.ToUpper()))).ToList();
            }

            return students;
        }

        public IEnumerable<StudentAllocationGroupedViewModel> GetAllStudentAllocation(bool? status = null)
        {
            IQueryable<Student_Subject_Teacher_Allocation> query = _dbEntities.Student_Subject_Teacher_Allocation
                .Include("Student")
                .Include("Teacher_Subject_Allocation.Teacher")
                .Include("Teacher_Subject_Allocation.Subject");

            if (status.HasValue)
            {
                query = query.Where(s => s.Student.IsEnable == status.Value);
            }

            var allStudentAllocations = query.ToList();

            var groupedData = allStudentAllocations
                .GroupBy(item => new { item.Student.StudentRegNo, item.Student.DisplayName, item.Student.IsEnable })
                .Select(group => new StudentAllocationGroupedViewModel
                {
                    StudentRegNo = group.Key.StudentRegNo,
                    DisplayName = group.Key.DisplayName,
                    IsEnable = group.Key.IsEnable, // Set the IsEnable property
                    TeacherAllocations = group.GroupBy(g => new { g.Teacher_Subject_Allocation.Teacher.TeacherRegNo, g.Teacher_Subject_Allocation.Teacher.DisplayName })
                        .Select(subGroup => new TeacherAllocationViewModel
                        {
                            TeacherRegNo = subGroup.Key.TeacherRegNo,
                            TeacherName = subGroup.Key.DisplayName,
                            Subjects = subGroup.Select(item => new SubjectAllocationViewModel
                            {
                                StudentAllocationID = item.StudentAllocationID,
                                SubjectCode = item.Teacher_Subject_Allocation.Subject.SubjectCode,
                                Name = item.Teacher_Subject_Allocation.Subject.Name
                            }).ToList()
                        }).ToList()
                });

            return groupedData;
        }

        public Student_Subject_Teacher_AllocationBO GetStudentAllocationById(long studentAllocationId)
        {
            var results = _dbEntities.Student_Subject_Teacher_Allocation.Select(s => new Student_Subject_Teacher_AllocationBO()
            {
                StudentAllocationID = s.StudentAllocationID,
                StudentID = s.StudentID,
                SubjectAllocationID = s.SubjectAllocationID

            }).Where(s => s.StudentAllocationID == studentAllocationId).FirstOrDefault();

            return results;

        }

        public IEnumerable<object> GetTeachersBySubjectID(int subjectID)
        {
            return _dbEntities.Teacher_Subject_Allocation
                .Where(tsa => tsa.SubjectID == subjectID)
                .Select(tsa => new
                {
                    TeacherRegNo = tsa.Teacher.TeacherRegNo,
                    DisplayName = tsa.Teacher.DisplayName,
                    SubjectAllocationID = tsa.SubjectAllocationID
                })
                .ToList();
        }
        public IEnumerable<SelectListItem> GetStudentList()
        {
            return _dbEntities.Students
                .Where(s => s.IsEnable)
                .Select(s => new SelectListItem
                {
                    Value = s.StudentID.ToString(),
                    Text = s.StudentRegNo + " - " + s.DisplayName
                })
                .ToList();
        }

        public IEnumerable<SelectListItem> GetTeacherList()
        {
            return _dbEntities.Teacher_Subject_Allocation
                .Select(ts => new SelectListItem
                {
                    Value = ts.TeacherID.ToString(),
                    Text = ts.Teacher.TeacherRegNo + " - " + ts.Teacher.DisplayName
                })
                .ToList();
        }

        public IEnumerable<SelectListItem> GetSubjectList()
        {
            return _dbEntities.Teacher_Subject_Allocation
                .Select(ts => new SelectListItem
                {
                    Value = ts.SubjectID.ToString(),
                    Text = ts.Subject.SubjectCode + " - " + ts.Subject.Name
                })
                .Distinct()
                .ToList();
        }
        public IEnumerable<SelectListItem> GetAllSubjectsFromAllocation()
        {
            return _dbEntities.Teacher_Subject_Allocation
                .Select(ts => new SelectListItem
                {
                    Value = ts.SubjectID.ToString(),
                    Text = ts.Subject.SubjectCode + " - " + ts.Subject.Name
                })
                .Distinct()
                .ToList();
        }


        public bool SaveStudentAllocation(Student_Subject_Teacher_AllocationBO studentAllocation, out string msg)
        {
            msg = "";

            bool isExistingStudentAllocation = _dbEntities.Student_Subject_Teacher_Allocation.Any(s => s.StudentAllocationID == studentAllocation.StudentAllocationID);

            bool isStudentAllocated = _dbEntities.Student_Subject_Teacher_Allocation.Any(s => s.SubjectAllocationID == studentAllocation.SubjectAllocationID && s.StudentID == studentAllocation.StudentID);
            // bool isSubjectAllocated=Student.



            try
            {

                if (isStudentAllocated)
                {
                    msg = "This Allocation Already Exists.";
                    return false;
                }
                var newStudentAllocation = new SMS.Data.Student_Subject_Teacher_Allocation();
                newStudentAllocation.StudentID = studentAllocation.StudentID;
                newStudentAllocation.SubjectAllocationID = studentAllocation.SubjectAllocationID;



                _dbEntities.Student_Subject_Teacher_Allocation.Add(newStudentAllocation);
                _dbEntities.SaveChanges();
                msg = "Student Allocation Added Successfully!";
                return true;
            }
            catch (Exception error)
            {
                msg = error.Message;
                return false;
            }

        }

        public bool DeleteStudentAllocation(long id, out string msg)
        {
            msg = "";
            var studentAllocation = _dbEntities.Student_Subject_Teacher_Allocation.SingleOrDefault(t => t.StudentAllocationID == id);
            try
            {
                if (studentAllocation != null)
                {

                    _dbEntities.Student_Subject_Teacher_Allocation.Remove(studentAllocation);
                    _dbEntities.SaveChanges();
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

        public IEnumerable<StudentAllocationGroupedViewModel> SearchStudentAllocations(string searchText, string searchCategory)
        {
            var students = GetAllStudentAllocation();

            // Perform the search logic based on the selected category
            if (searchCategory == "StudentRegNo")
            {
                students = students.Where(a => a.StudentRegNo.ToUpper().Contains(searchText.ToUpper())).ToList();
            }
            else if (searchCategory == "TeacherRegNo")
            {
                students = students.Where(a => a.TeacherAllocations.Any(t => t.TeacherRegNo.ToUpper().Contains(searchText.ToUpper()))).ToList();
            }
            else if (searchCategory == "SubjectCode")
            {
                students = students.Where(a => a.TeacherAllocations.Any(t => t.Subjects.Any(s => s.SubjectCode.ToUpper().Contains(searchText.ToUpper())))).ToList();
            }
            else if (searchCategory == "StudentName")
            {
                students = students.Where(a => a.DisplayName.ToUpper().Contains(searchText.ToUpper())).ToList();
            }
            else if (searchCategory == "TeacherName")
            {
                students = students.Where(a => a.TeacherAllocations.Any(t => t.TeacherName.ToUpper().Contains(searchText.ToUpper()))).ToList();
            }
            else if (searchCategory == "SubjectName")
            {
                students = students.Where(a => a.TeacherAllocations.Any(t => t.Subjects.Any(s => s.Name.ToUpper().Contains(searchText.ToUpper())))).ToList();
            }

            return students;
        }
    }
}
