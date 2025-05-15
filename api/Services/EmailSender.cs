using IotSmartHome.Data.Entities;
using Microsoft.AspNetCore.Identity;

namespace IotSmartHome.Services;

public class EmailSender : IEmailSender<UserEntity>
{
    public Task SendConfirmationLinkAsync(UserEntity user, string email, string confirmationLink)
    {
        return Task.CompletedTask;
    }

    public Task SendPasswordResetLinkAsync(UserEntity user, string email, string resetLink)
    {
        return Task.CompletedTask;
    }

    public Task SendPasswordResetCodeAsync(UserEntity user, string email, string resetCode)
    {
        return Task.CompletedTask;
    }
}