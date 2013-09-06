using CmsLite.Services.Helpers;
using NUnit.Framework;
using SharpTestsEx;

namespace CmsLite.Unit.Helpers
{
    [TestFixture]
    public class CmsUrlHelperFixture
    {
        [Test]
        public void FormatUrlName_RemovesAllSpaces()
        {
            //arrange
            const string urlName = "Foo Bar";

            //act
            var formattedUrlName = CmsUrlHelper.FormatUrlName(urlName);

            //assert
            formattedUrlName.Should().Be.EqualTo(urlName.Replace(" ", "").ToLower());
        }

        [Test]
        public void FormatUrlName_ConvertsToLowercase()
        {
            //arrange
            const string urlName = "FooBar";

            //act
            var formattedUrlName = CmsUrlHelper.FormatUrlName(urlName);

            //assert
            formattedUrlName.Should().Be.EqualTo(urlName.ToLower());
        }
    }
}
