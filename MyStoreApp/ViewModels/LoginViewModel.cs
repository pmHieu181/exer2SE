using System.ComponentModel.DataAnnotations; // Thư viện này cần thiết cho Data Annotations

namespace MyStoreApp.ViewModels // << Quan trọng: Thay "MyStoreApp" bằng tên namespace thực tế của dự án bạn
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập.")]
        [Display(Name = "Tên đăng nhập")] // Tên sẽ hiển thị trên label của form
        public string Username { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu.")]
        [DataType(DataType.Password)] // Giúp input hiển thị dưới dạng mật khẩu (các dấu chấm tròn)
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; }

        [Display(Name = "Ghi nhớ tôi?")] // Tên hiển thị cho checkbox "Remember Me"
        public bool RememberMe { get; set; }
    }
}