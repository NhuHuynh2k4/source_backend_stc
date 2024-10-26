using Microsoft.AspNetCore.Http; // Thêm thư viện để sử dụng StatusCodes

namespace sourc_backend_stc.Utils
{
    public static class ErrorHandling
    {
        // Phương thức để validate ID
        public static (bool IsValid, string ErrorMessage) ValidateId(int? id)
        {
            if (!id.HasValue || id <= 0)
            {
                return (false, GetStatusCodeMessage(StatusCodes.Status400BadRequest));
            }
            return (true, null);
        }

        // Phương thức kiểm tra dữ liệu đầu vào và xử lý lỗi nếu không có dữ liệu
        public static (bool IsSuccess, string Message) HandleIfEmpty(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return (false, string.Empty); // Trả về lỗi trống nếu dữ liệu đầu vào rỗng
            }
            return (true, "Dữ liệu hợp lệ"); // Trả về thành công nếu có dữ liệu
        }

        public static bool HandleError(int statusCode)
        {
            // Log hoặc xử lý chi tiết lỗi (tuỳ chọn) nếu cần thiết
            Console.WriteLine($"Lỗi xảy ra với mã trạng thái: {statusCode}");

            // Trả về false khi gặp lỗi
            return false;
        }

        // Phương thức để lấy thông báo theo StatusCodes
        public static string GetStatusCodeMessage(int statusCode)
        {
            return statusCode switch
            {
                StatusCodes.Status400BadRequest => "Yêu cầu không hợp lệ.",
                StatusCodes.Status404NotFound => "Không tìm thấy tài nguyên.",
                StatusCodes.Status500InternalServerError => "Lỗi máy chủ nội bộ.",
                _ => "Lỗi không xác định."
            };
        }
    }
}
