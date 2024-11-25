namespace sourc_backend_stc.Utils
{
    public static class ErrorCodes
    {
        // Các mã trạng thái (status codes)
        public const int Success = 2000;                // Mã lỗi cho ID không hợp lệ
        public const int InvalidId = 4001;                // Mã lỗi cho ID không hợp lệ
        public const int ResourceNotFound = 4004;         // Mã lỗi cho tài nguyên không tìm thấy
        public const int InternalServerError = 5000;      // Mã lỗi cho lỗi nội bộ hệ thống
        public const int BadRequest = 4000;                // Mã lỗi cho yêu cầu không hợp lệ
        public const int Unauthorized = 4010;              // Mã lỗi cho yêu cầu không được ủy quyền
        public const int Forbidden = 4030;                 // Mã lỗi cho yêu cầu bị từ chối
        public const int NotImplemented = 5010;            // Mã lỗi cho chức năng chưa được thực hiện

        // Thông báo lỗi tương ứng
        public static readonly Dictionary<int, string> ErrorMessages = new Dictionary<int, string>
        {
            { InvalidId, "ID không hợp lệ." },
            { ResourceNotFound, "Không tìm thấy ID đã nhập." },
            { InternalServerError, "Có lỗi xảy ra trong hệ thống." },
            { BadRequest, "Yêu cầu không hợp lệ." },
            { Unauthorized, "Bạn không có quyền truy cập vào tài nguyên này." },
            { Forbidden, "Yêu cầu bị từ chối." },
            { NotImplemented, "Chức năng chưa được thực hiện." }
        };

        // Phương thức lấy thông báo lỗi theo mã
        public static string GetErrorMessage(int errorCode)
        {
            return ErrorMessages.TryGetValue(errorCode, out var message) ? message : "Lỗi không xác định.";
        }
    }
}
