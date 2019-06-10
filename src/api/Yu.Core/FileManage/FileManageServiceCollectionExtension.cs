using Microsoft.Extensions.DependencyInjection;

namespace Yu.Core.FileManage
{
    public static class FileManageServiceCollectionExtension
    {
        public static void AddFileManage(this IServiceCollection services)
        {
            services.AddSingleton<IFileEntity, FileManageEntity>();
            services.AddSingleton<IFileStore, FileManageStore>();
        }
    }
}
