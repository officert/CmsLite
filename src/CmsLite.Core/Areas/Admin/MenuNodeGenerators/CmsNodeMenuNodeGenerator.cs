using System.Collections.Generic;
using CmsLite.Interfaces.Services;
using MenuGen;
using MenuGen.MenuNodeGenerators;
using MenuGen.Models;

namespace CmsLite.Core.Areas.Admin.MenuNodeGenerators
{
    public class CmsNodeMenuNodeGenerator : MenuNodeGeneratorBase
    {
        private readonly ISectionNodeService _sectionNodeService;

        public CmsNodeMenuNodeGenerator(IMenuNodeTreeBuilder menuNodeTreeBuilder) : base(menuNodeTreeBuilder)
        {
        }

        public override IEnumerable<MenuNodeModel> GenerateMenuNodes()
        {
            return null;
            //return base.GenerateMenuNodes();
        }
    }
}