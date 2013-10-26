using System;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using CmsLite.Core;
using CmsLite.Domains.Entities;
using CmsLite.Interfaces.Services;
using CmsLite.Resources;
using Moq;
using NUnit.Framework;
using SharpTestsEx;

namespace CmsLite.Unit
{
    [TestFixture]
    public class CmsActionInvokerFixture
    {
        private CmsActionInvoker _actionInvoker;
        private Mock<ISectionNodeService> _sectionNodeServiceMock;
        private Mock<IPageNodeService> _pageNodeServiceMock;
        private Mock<ControllerContext> _controllerContextMock;
        private RouteData _routeData;

        [TestFixtureSetUp]
        public void SetupFixture()
        {
            _sectionNodeServiceMock = new Mock<ISectionNodeService>();
            _pageNodeServiceMock = new Mock<IPageNodeService>();
            _actionInvoker = new CmsActionInvoker(_sectionNodeServiceMock.Object, _pageNodeServiceMock.Object);

            _controllerContextMock = new Mock<ControllerContext>();
            _routeData = new RouteData();

            _controllerContextMock.Setup(x => x.RouteData).Returns(_routeData);
        }

        [Test]
        public void InvokeAction_ControllerContextIsNull_ReturnsFalse()
        {
            //arrange
            const string actionName = "Foobar";

            //act
            var result = _actionInvoker.InvokeAction(null, actionName);

            //assert

            result.Should().Be.False();
        }

        [Test]
        public void InvokeAction_ControllerContextRouteDataIsNull_ReturnsFalse()
        {
            //arrange
            const string actionName = "Foobar";
            _controllerContextMock.Setup(x => x.RouteData).Returns((RouteData) null);

            //act
            var result = _actionInvoker.InvokeAction(_controllerContextMock.Object, actionName);

            //assert

            result.Should().Be.False();
        }

        [Test]
        [Ignore]
        public void InvokeAction_NoSectionNodeExistsForRouteDataControllerName_ReturnsFalse()
        {
            //arrange
            const string actionName = "Foobar";
            const string controllerName = "foobar";

            _sectionNodeServiceMock.Setup(x => x.GetByUrlName(It.IsAny<string>())).Returns((SectionNode)null);

            //act + assert
            Assert.That(() => _actionInvoker.InvokeAction(_controllerContextMock.Object, actionName),
                Throws.ArgumentException.With.Message.EqualTo(string.Format(Messages.SectionNodeNotFoundForUrlName,
                    controllerName)));
        }
    }
}
