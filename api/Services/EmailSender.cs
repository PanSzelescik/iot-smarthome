using IotSmartHome.Database.Entities;
using Microsoft.AspNetCore.Identity;

namespace IotSmartHome.Services;

public class EmailSender : IEmailSender<UserEntity>
{
    public Task SendConfirmationLinkAsync(UserEntity user, string email, string confirmationLink)
    {
        throw new NotImplementedException();
    }

    public Task SendPasswordResetLinkAsync(UserEntity user, string email, string resetLink)
    {
        throw new NotImplementedException();
    }

    public Task SendPasswordResetCodeAsync(UserEntity user, string email, string resetCode)
    {
        throw new NotImplementedException();
    }
}