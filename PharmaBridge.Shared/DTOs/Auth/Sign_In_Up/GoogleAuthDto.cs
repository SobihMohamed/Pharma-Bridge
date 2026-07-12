using PharmaBridge.Shared.EnumHelper.UserEnums;

namespace PharmaBridge.Shared.DTOs.Auth.Sign_In_Up
{
    public class GoogleAuthDto
    {
        public string IdToken { get; set; } = null!;

        public UserRole? Role { get; set; } =  UserRole.Patient;
    }
}