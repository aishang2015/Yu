using Microsoft.Extensions.FileProviders.Physical;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Yu.Core.FileManage
{
    public class FileManageStore : IFileStore
    {

        // 文件访问的服务器路径
        private readonly string _requestPath;

        /// <summary>
        /// 取得文件信息
        /// </summary>
        /// <param name="relativePath">相对路径</param>
        /// <param name="rootPhysicalPath">物理根路径</param>
        /// <returns>文件信息</returns>
        public IFileEntity GetFileInfo(string relativePath, string rootPhysicalPath)
        {
            var path = GetFilePhysicalPath(relativePath, rootPhysicalPath);
            var fileInfo = new PhysicalFileInfo(new FileInfo(path));
            return fileInfo.Exists ? new FileManageEntity
            {
                FilePhysicalPath = path,
                IsDirectory = fileInfo.IsDirectory,
                LastModified = fileInfo.LastModified.UtcDateTime,
                Length = fileInfo.Length,
                RelativePath = relativePath,
                Name = fileInfo.Name
            } : null;
        }

        /// <summary>
        /// 取得目录信息
        /// </summary>
        /// <param name="relativePath">相对路径</param>
        /// <param name="rootPhysicalPath">物理根路径</param>
        /// <returns>目录信息</returns>
        public IFileEntity GetDirectoryInfo(string relativePath, string rootPhysicalPath)
        {
            var path = GetFilePhysicalPath(relativePath, rootPhysicalPath);
            var directoryInfo = new PhysicalDirectoryInfo(new DirectoryInfo(path));
            return directoryInfo.Exists ? new FileManageEntity
            {
                FilePhysicalPath = path,
                IsDirectory = directoryInfo.IsDirectory,
                LastModified = directoryInfo.LastModified.UtcDateTime,
                Length = directoryInfo.Length,
                RelativePath = relativePath,
                Name = directoryInfo.Name
            } : null;
        }

        /// <summary>
        /// 取得目录内容包括目录和文件
        /// </summary>
        /// <param name="relativePath">相对路径</param>
        /// <param name="rootPhysicalPath">物理根路径</param>
        /// <returns>目录内容</returns>
        public IEnumerable<IFileEntity> GetDirectoryContents(string relativePath, string rootPhysicalPath)
        {
            var results = new List<IFileEntity>();
            var path = GetFilePhysicalPath(relativePath, rootPhysicalPath);
            var directoryInfo = new PhysicalDirectoryInfo(new DirectoryInfo(path));
            if (!directoryInfo.Exists)
            {
                return results;
            }

            // 全部目录
            Directory.GetDirectories(path).ToList().ForEach(dir =>
                results.Add(GetDirectoryInfo(dir.Substring(rootPhysicalPath.Length), rootPhysicalPath)));

            // 全部文件
            Directory.GetFiles(path).ToList().ForEach(file =>
                results.Add(GetFileInfo(file.Substring(rootPhysicalPath.Length), rootPhysicalPath)));

            return results;
        }

        /// <summary>
        /// 创建目录
        /// </summary>
        /// <param name="relativePath">相对路径</param>
        /// <param name="rootPhysicalPath">物理根路径</param>
        /// <returns>创建结果</returns>
        public bool CreateDirectory(string relativePath, string rootPhysicalPath)
        {
            var path = GetFilePhysicalPath(relativePath, rootPhysicalPath);

            // 目录或文件存在无法创建
            if (File.Exists(path) || Directory.Exists(path))
            {
                return false;
            }

            // 创建目录
            Directory.CreateDirectory(path);
            return true;
        }

        /// <summary>
        /// 创建文件
        /// </summary>
        /// <param name="relativePath">相对路径</param>
        /// <param name="rootPhysicalPath">物理根路径</param>
        /// <param name="fileStream">文件流</param>
        public async Task<IFileEntity> CreateFile(string relativePath, string rootPhysicalPath, Stream fileStream)
        {
            var path = GetFilePhysicalPath(relativePath, rootPhysicalPath);
            using (var stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write))
            {
                await fileStream.CopyToAsync(stream);
            }
            var fileInfo = new PhysicalFileInfo(new FileInfo(path));
            return new FileManageEntity
            {
                FilePhysicalPath = path,
                IsDirectory = fileInfo.IsDirectory,
                LastModified = fileInfo.LastModified.UtcDateTime,
                Length = fileInfo.Length,
                RelativePath = relativePath,
                Name = fileInfo.Name
            };
        }

        /// <summary>
        /// 删除目录
        /// </summary>
        /// <param name="relativePath">相对路径</param>
        /// <param name="rootPhysicalPath">物理根路径</param>
        /// <returns>删除结果</returns>
        public bool DeleteDirectory(string relativePath, string rootPhysicalPath)
        {
            var path = GetFilePhysicalPath(relativePath, rootPhysicalPath);

            // 目录不存在
            if (!Directory.Exists(path))
            {
                return false;
            }

            // 递归删除
            Directory.Delete(path, true);
            return true;
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="relativePath">相对路径</param>
        /// <param name="rootPhysicalPath">物理根路径</param>
        /// <returns>删除结果</returns>
        public bool DeleteFile(string relativePath, string rootPhysicalPath)
        {
            var path = GetFilePhysicalPath(relativePath, rootPhysicalPath);

            // 文件不存在
            if (!File.Exists(path))
            {
                return false;
            }

            File.Delete(path);
            return true;
        }

        /// <summary>
        /// 取得路径对应的绝对路径
        /// </summary>
        /// <param name="relativePath">相对路径</param>
        /// <param name="rootPhysicalPath">物理根路径</param>
        /// <returns>服务器物理路径</returns>
        private string GetFilePhysicalPath(string relativePath, string rootPhysicalPath)
        {
            relativePath = relativePath?.Replace('\\', '/').Trim('/');
            var resultPath = string.IsNullOrEmpty(relativePath) ? rootPhysicalPath : Path.Combine(rootPhysicalPath, relativePath);
            return resultPath;
        }
    }
}
