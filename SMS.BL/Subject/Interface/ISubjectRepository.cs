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
        //IQueryable<Data.Subject> Subjects { get; }
        IEnumerable<SubjectBO> GetAllSubject();
        IEnumerable<SubjectBO> GetSubjects(bool? isEnable = null);
        
        SubjectBO GetSubjectByID(long subjectID);
        bool SaveSubject(SubjectBO subject, out string msg);
        bool DeleteSubject(long id, out string msg);
        //void SaveChanges();
        bool SubjectCodeExists(long subjectId, string subjectCode);
        bool SubjectNameExists(long subjectId, string subjectName);

        bool IsSubjectReferenced(int subjectId);

        bool ToggleSubjectEnable(int subjectId, out string message);


    }
}
