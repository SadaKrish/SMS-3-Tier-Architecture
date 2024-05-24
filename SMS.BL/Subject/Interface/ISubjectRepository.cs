using SMS.Models.Subject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.BL.Subject.Interface
{
    public interface ISubjectRepository
    {
       /// <summary>
       /// Get the subjects list according to the status
       /// </summary>
       /// <param name="isEnable"></param>
       /// <returns></returns>
        IEnumerable<SubjectBO> GetSubjects(bool? isEnable = null);
        /// <summary>
        /// Get the subject by id
        /// </summary>
        /// <param name="subjectID"></param>
        /// <returns></returns>
        SubjectBO GetSubjectByID(long subjectID);
        /// <summary>
        /// Save the subject
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        bool SaveSubject(SubjectBO subject, out string msg);
        /// <summary>
        /// Delete the subject record
        /// </summary>
        /// <param name="id"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        bool DeleteSubject(long id, out string msg);
        /// <summary>
        /// Check the existence of subject code
        /// </summary>
        /// <param name="subjectId"></param>
        /// <param name="subjectCode"></param>
        /// <returns></returns>
        //void SaveChanges();
        bool SubjectCodeExists(long subjectId, string subjectCode);
        /// <summary>
        /// Chheck the subect name existence
        /// </summary>
        /// <param name="subjectId"></param>
        /// <param name="subjectName"></param>
        /// <returns></returns>
        bool SubjectNameExists(long subjectId, string subjectName);
        /// <summary>
        /// CHeck whether the subject allocated for any teacher
        /// </summary>
        /// <param name="subjectId"></param>
        /// <returns></returns>
        bool IsSubjectReferenced(int subjectId);
        /// <summary>
        /// Change the status of subject
        /// </summary>
        /// <param name="subjectId"></param>
        /// <param name="message"></param>
        /// <returns></returns>

        bool ToggleSubjectEnable(int subjectId, out string message);
        IEnumerable<SubjectBO> SearchSubjects(string searchText, string searchCategory);

    }
}
