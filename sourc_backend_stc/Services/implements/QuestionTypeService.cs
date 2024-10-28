using sourc_backend_stc.Models;
using sourc_backend_stc.Utils;
using sourc_backend_stc.Data;

namespace sourc_backend_stc.Services
{
    public class QuestionTypeService : IQuestionTypeService
    {
        private readonly AppDbContext _context;

        public QuestionTypeService(AppDbContext context)
        {
            _context = context;
        }

        public QuestionTypeResponse GetQuestionTypeById(int id)
        {
            var validationResult = ErrorHandling.ValidateId(id);
            if (!validationResult.IsValid)
                throw new Exception(ErrorCodes.GetErrorMessage(ErrorCodes.InvalidId));

            var questionType = _context.QuestionTypes.FirstOrDefault(qt => qt.QuestionTypeID == id);
            if (questionType == null)
                throw new Exception(ErrorCodes.GetErrorMessage(ErrorCodes.ResourceNotFound));

            return new QuestionTypeResponse
            {
                QuestionTypeID = questionType.QuestionTypeID,
                QuestionTypeCode = questionType.QuestionTypeCode,
                QuestionTypeName = questionType.QuestionTypeName,
                CreateDate = questionType.CreateDate,
                UpdateDate = questionType.UpdateDate
            };
        }

        public void CreateQuestionType(QuestionType_CreateReq request)
        {
            var newQuestionType = new QuestionType
            {
                QuestionTypeCode = request.QuestionTypeCode,
                QuestionTypeName = request.QuestionTypeName
            };

            _context.QuestionTypes.Add(newQuestionType);
            _context.SaveChanges();
        }

        public void UpdateQuestionType(int id, QuestionType_CreateReq request)
        {
            var questionType = _context.QuestionTypes.FirstOrDefault(qt => qt.QuestionTypeID == id);
            if (questionType == null)
                throw new Exception(ErrorCodes.GetErrorMessage(ErrorCodes.ResourceNotFound));

            questionType.QuestionTypeCode = request.QuestionTypeCode;
            questionType.QuestionTypeName = request.QuestionTypeName;
            questionType.UpdateDate = DateTime.Now;

            _context.SaveChanges();
        }

        public void DeleteQuestionType(int id)
        {
            var questionType = _context.QuestionTypes.FirstOrDefault(qt => qt.QuestionTypeID == id);
            if (questionType == null)
                throw new Exception(ErrorCodes.GetErrorMessage(ErrorCodes.ResourceNotFound));

            questionType.IsDelete = true;
            _context.SaveChanges();
        }
    }
}
