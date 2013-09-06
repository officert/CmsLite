using CmsLite.Domains.Entities;

namespace CmsLite.Interfaces.Services
{
    public interface IMediaService : IServiceBase<File>
    {
        void Create(byte[] fileData, string mimeType, string name);
    }
}
