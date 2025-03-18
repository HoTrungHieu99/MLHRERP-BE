using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.IService
{
    public interface IEmailService
    {
        Task<bool> SendEmailRegisterAccountAsync(string emailRequest, string subjectEmail, string fullName);
    }
}