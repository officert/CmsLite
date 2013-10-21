using System.Collections.Generic;
using CmsLite.Domains.Entities;

namespace CmsLite.Interfaces.Services
{
    public interface IFileService
    {
        File GetById(int id);

        IEnumerable<File> GetAllFiles();

        void Create(byte[] fileData, string mimeType, string name);
    }
}
