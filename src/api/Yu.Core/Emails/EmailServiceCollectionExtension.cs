using Microsoft.Extensions.DependencyInjection;

namespace Yu.Core.Emails
{
    public static class EmailServiceCollectionExtension
    {
        public static void AddEmailSender(this IServiceCollection services)
        {
            //services.AddSingleton<IEmailSender>();
        }
    }
}
