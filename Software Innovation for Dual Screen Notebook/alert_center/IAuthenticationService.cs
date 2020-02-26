using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using alert_center.models;

namespace alert_center
{
    public interface IAuthenticationService
    {
        Task<TokenInfo> Authenticate(String userName, String password, String appId, String secret);
    }
}
