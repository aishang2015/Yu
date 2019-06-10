using System;

namespace Yu.Core.FileManage
{
    public class FileManageEntity : IFileEntity
    {
        // 相对服务器路径
        public string RelativePath { get; set; }

        // 绝对路径
        public string FilePhysicalPath { get; set; }

        // 文件名或文件夹名
        public string Name { get; set; }

        // 编辑时间
        public DateTime LastModified { get; set; }

        // 文件长度 文件夹情况下为-1
        public long Length { get; set; }

        // 是否为目录
        public bool IsDirectory { get; set; }

    }
}
