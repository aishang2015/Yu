using System;

namespace Yu.Core.FileManage
{
    public interface IFileEntity
    {
        string RelativePath { get; set; }

        string FilePhysicalPath { get; set; }

        string Name { get; set; }

        DateTime LastModified { get; set; }

        long Length { get; set; }

        bool IsDirectory { get; set; }
    }
}
