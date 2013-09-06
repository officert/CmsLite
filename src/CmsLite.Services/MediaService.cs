using System;
using CmsLite.Domains.Entities;
using CmsLite.Interfaces.Data;
using CmsLite.Interfaces.Services;

namespace CmsLite.Services
{
    public class MediaService : ServiceBase<File>, IMediaService
    {
        public MediaService(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }

        public void Create(byte[] fileData, string mimeType, string name)
        {
            if(fileData == null || mimeType == null)
                throw new ArgumentException("FileData and Mime Type must be provided.");

            var fileDbSet = UnitOfWork.Context.GetDbSet<File>();

            var newFile = fileDbSet.Create();
            newFile.FileData = fileData;
            newFile.MimeType = mimeType;
            newFile.Name = name;

            fileDbSet.Add(newFile);

            UnitOfWork.Context.SaveChanges();
        }
    }
}
