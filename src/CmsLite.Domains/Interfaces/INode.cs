using System;
using System.Collections.Generic;
using CmsLite.Domains.Entities;

namespace CmsLite.Domains.Interfaces
{
    public interface INode
    {
        int Id { get; set; }

        int Order { get; set; }

        string DisplayName { get; set; }

        string UrlName { get; set; }

        bool IsPublished { get; set; }

        DateTime? CreatedOn { get; set; }

        DateTime? ModifiedOn { get; set; }

        ICollection<PageNode> PageNodes { get; set; }

        bool InTrash { get; set; }
    }
}
