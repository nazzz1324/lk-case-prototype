using Account.Domain.Enum;
using System.Collections.Generic;

namespace Account.Domain.Dictionaries
{
    public static class ErrorDictionary
    {
        private static readonly Dictionary<ErrorCodes, int> _httpStatusMap = new()
        {
            // Аутентификация
            [ErrorCodes.UserAlreadyExists] = 409,       
            [ErrorCodes.UserNotFound] = 404,            
            [ErrorCodes.UnauthorizedAccess] = 401, 
            [ErrorCodes.PasswordNotEqualsPasswordConfirm] = 400, 
            [ErrorCodes.PasswordIsWrong] = 401,
            
            // Группы
            [ErrorCodes.GroupAlreadyExists] = 409,
            [ErrorCodes.GroupNotFound] = 404,
            [ErrorCodes.GroupDoesNotHaveThisDiscipline] = 401,
            [ErrorCodes.GroupDoesNotHaveProle] = 401,



            // Преподаватели и студенты
            [ErrorCodes.TeacherAlreadyExists] = 409,
            [ErrorCodes.TeacherNotFound] = 404,
            [ErrorCodes.TeacherNoAccess] = 401,
            [ErrorCodes.StudentNotFound] = 404,
            [ErrorCodes.StudentDoesNotHaveGroup] = 401,

            // Профессиональные роли
            [ErrorCodes.ProleAlreadyExists] = 409,
            [ErrorCodes.ProleNotFound] = 404,

            // Дисциплины
            [ErrorCodes.DisciplineAlreadyExists] = 409,
            [ErrorCodes.DisciplineNotFound] = 404,

            // Компетенции
            [ErrorCodes.CompetenceAlreadyExists] = 409,
            [ErrorCodes.CompetenceNotFound] = 404,

            // Индикаторы
            [ErrorCodes.IndicatorAlreadyExists] = 409,
            [ErrorCodes.IndicatorNotFound] = 404,
            [ErrorCodes.InvalidScore] = 400,

            // Ролевая политика
            [ErrorCodes.RoleAlreadyExists] = 409,
            [ErrorCodes.RoleNotFound] = 404,
            [ErrorCodes.UserAlreadyExistsThisRole] = 409,

            // Системные
            [ErrorCodes.InternalServerError] = 500      
        };

        public static int GetHttpStatus(ErrorCodes errorCode)
        {
            if (_httpStatusMap.TryGetValue(errorCode, out int httpStatus))
                return httpStatus;

            return 400; 
        }
    }
}