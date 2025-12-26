using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Account.Domain.Enum
{
    public enum ErrorCodes
    {

        UserAlreadyExists = 12,
        UserNotFound = 13,
        UnauthorizedAccess = 14,
        UserAlreadyExistsThisRole = 15,
        
        GroupAlreadyExists = 81,
        GroupNotFound = 82,
        GroupDoesNotHaveThisDiscipline = 83,




        TeacherAlreadyExists = 91,
        TeacherNotFound = 92,
        TeacherNoAccess = 93,
        StudentNotFound = 94,
        StudentDoesNotHaveGroup = 95,


        ProleAlreadyExists = 71,
        ProleNotFound = 72,

        DisciplineAlreadyExists = 61,
        DisciplineNotFound = 62,


        CompetenceAlreadyExists = 51,
        CompetenceNotFound = 52,

        IndicatorAlreadyExists = 41,
        IndicatorNotFound = 42,
        InvalidScore = 43,

        RoleAlreadyExists = 31,
        RoleNotFound = 32,

        PasswordNotEqualsPasswordConfirm = 21,
        PasswordIsWrong = 22,

        InternalServerError = 10,

    }
}
