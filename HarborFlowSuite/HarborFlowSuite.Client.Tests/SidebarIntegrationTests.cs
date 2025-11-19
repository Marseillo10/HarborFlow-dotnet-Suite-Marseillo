using Bunit;
using Bunit.TestDoubles;
using HarborFlowSuite.Client.Layout;
using HarborFlowSuite.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Microsoft.AspNetCore.Components.Authorization;
using Xunit;

namespace HarborFlowSuite.Client.Tests
{
    public class SidebarIntegrationTests : TestContext
    {
        public SidebarIntegrationTests()
        {
            Services.AddSingleton(new Mock<IAuthService>().Object);
            Services.AddSingleton(new Mock<NavigationManager>().Object);
        }

        [Fact]
        public void Sidebar_Should_Toggle_Class_When_Button_Clicked()
        {
            // Arrange
            var cut = Render<MainLayout>();

            // Act
            // Find the toggle button in NavMenu and click it
            // Note: MainLayout renders NavMenu. We need to find the toggle button inside NavMenu.
            // The toggle button has a class 'sidebar-toggler-wrapper'
            var toggleButton = cut.Find(".sidebar-toggler-wrapper");
            toggleButton.Click();

            // Assert
            // The sidebar div in MainLayout should have the 'collapsed' class
            var sidebar = cut.Find(".sidebar");
            Assert.Contains("collapsed", sidebar.ClassName);
        }

        [Fact]
        public void Sidebar_Should_Show_Logout_Button()
        {
            // Arrange
            var authServiceMock = new Mock<IAuthService>();

            // Manually mock AuthenticationStateProvider
            var identity = new System.Security.Claims.ClaimsIdentity(new[]
            {
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, "TestUser")
            }, "TestAuthentication");
            var principal = new System.Security.Claims.ClaimsPrincipal(identity);
            var authState = new AuthenticationState(principal);

            var authStateProviderMock = new Mock<AuthenticationStateProvider>();
            authStateProviderMock.Setup(p => p.GetAuthenticationStateAsync())
                .ReturnsAsync(authState);

            Services.AddSingleton(authStateProviderMock.Object);
            Services.AddAuthorizationCore(); // Required for AuthorizeView

            var cut = Render<NavMenu>();

            // Act
            var logoutBtn = cut.Find("button.nav-link");

            // Assert
            Assert.NotNull(logoutBtn);
            Assert.Contains("Log out", logoutBtn.TextContent);
        }
    }
}
