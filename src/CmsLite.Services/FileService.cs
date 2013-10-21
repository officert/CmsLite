using System;
using System.Collections.Generic;
using System.Linq;
using CmsLite.Domains.Entities;
using CmsLite.Interfaces.Data;
using CmsLite.Interfaces.Services;

namespace CmsLite.Services
{
    public class FileService : IFileService
    {
        private readonly IUnitOfWork _unitOfWork;

        public FileService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public File GetById(int id)
        {
            return _unitOfWork.Context.GetDbSet<File>().FirstOrDefault(x => x.Id == id);
        }

        public IEnumerable<File> GetAllFiles()
        {
            return _unitOfWork.Context.GetDbSet<File>().OrderBy(x => x.Id);
        }

        public void Create(byte[] fileData, string mimeType, string name)
        {
            if(fileData == null || mimeType == null)
                throw new ArgumentException("FileData and Mime Type must be provided.");

            var fileDbSet = _unitOfWork.Context.GetDbSet<File>();

            var newFile = fileDbSet.Create();
            newFile.FileData = fileData;
            newFile.MimeType = mimeType;
            newFile.Name = name;

            fileDbSet.Add(newFile);

            _unitOfWork.Context.SaveChanges();
        }
    }
}
