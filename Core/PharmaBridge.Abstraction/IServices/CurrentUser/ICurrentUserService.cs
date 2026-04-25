using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Abstraction.IServices.CurrentUser
{
    public interface ICurrentUserService
    {
        string UserId { get; }
        bool IsAdmin { get; }
        bool IsPharmacyOwner { get; }

        Task<int> GetPharmacyIdAsync();
    }
}
