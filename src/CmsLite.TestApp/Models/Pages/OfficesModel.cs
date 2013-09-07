﻿using CmsLite.Core.Cms.Attributes;
using CmsLite.Utilities.Cms;
using CmsLite.Core;

namespace CmsLite.TestApp.Models.Pages
{
    [CmsModelTemplate]
    public class OfficesModel
    {
        [CmsModelProperty(
            DisplayName = "Intro Text",
            PropertyType = CmsPropertyType.RichTextEditor,
            TabName = "Office Info",
            TabOrder = 1
            )]
        public string Intro { get; set; }
    }
}