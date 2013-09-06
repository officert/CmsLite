using System.Collections.Generic;

namespace CmsLite.Domains.Entities
{
    public interface IHavePageNodes
    {
        ICollection<PageNode> PageNodes { get; set; } 
    }
}
