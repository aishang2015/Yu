using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Yu.Core.FileManage
{
    public interface IFileStore
    {
        /// <summary>
        /// 取得文件信息
        /// </summary>
        /// <param name="relativePath">相对路径</param>
        /// <param name="rootPhysicalPath">物理根路径</param>
        /// <returns>文件信息</returns>
        IFileEntity GetFileInfo(string relativePath, string rootPhysicalPath);

        /// <summary>
        /// 取得目录信息
        /// </summary>
        /// <param name="relativePath">相对路径</param>
        /// <param name="rootPhysicalPath">物理根路径</param>
        /// <returns>目录信息</returns>
        IFileEntity GetDirectoryInfo(string relativePath, string rootPhysicalPath);

        /// <summary>
        /// 取得目录内容
        /// </summary>
        /// <param name="relativePath">相对路径</param>
        /// <param name="rootPhysicalPath">物理根路径</param>
        /// <returns>目录内容</returns>
        IEnumerable<IFileEntity> GetDirectoryContents(string relativePath, string rootPhysicalPath);

        /// <summary>
        /// 创建目录
        /// </summary>
        /// <param name="relativePath">相对路径</param>
        /// <param name="rootPhysicalPath">物理根路径</param>
        /// <returns>创建结果</returns>
        bool CreateDirectory(string relativePath, string rootPhysicalPath);

        /// <summary>
        /// 创建文件
        /// </summary>
        /// <param name="relativePath">相对路径</param>
        /// <param name="rootPhysicalPath">物理根路径</param>
        /// <param name="fileStream">文件流</param>
        Task<IFileEntity> CreateFile(string relativePath, string rootPhysicalPath, Stream fileStream);

        /// <summary>
        /// 删除目录
        /// </summary>
        /// <param name="relativePath">相对路径</param>
        /// <param name="rootPhysicalPath">物理根路径</param>
        /// <returns>删除结果</returns>
        bool DeleteDirectory(string relativePath, string rootPhysicalPath);

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="relativePath">相对路径</param>
        /// <param name="rootPhysicalPath">物理根路径</param>
        /// <returns>删除结果</returns>
        bool DeleteFile(string relativePath, string rootPhysicalPath);
    }
}
