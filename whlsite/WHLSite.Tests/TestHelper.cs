using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace WHLSite.Tests;

internal static class TestHelper
{
    internal static ControllerContext GetDefaultControllerContext(bool withServices = false)
    {
        if (withServices)
        {
            var authServiceMock = new Mock<IAuthenticationService>();
            authServiceMock
                .Setup(_ => _.SignInAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<ClaimsPrincipal>(), It.IsAny<AuthenticationProperties>()))
                .Returns(Task.FromResult((object)null));
            var services = new ServiceCollection();
            services.AddControllersWithViews();
            services.AddSingleton<IAuthenticationService>(authServiceMock.Object);
            return new ControllerContext()
            {
                HttpContext = new DefaultHttpContext
                {
                    User = ClaimsPrincipal.Current,
                    RequestServices = services.BuildServiceProvider()
                }
            };
        }

        return new ControllerContext()
        {
            HttpContext = new DefaultHttpContext
            {
                User = ClaimsPrincipal.Current
            }
        };
    }

    internal static ControllerContext GetAuthenticatedControllerContext(bool withServices = false)
    {
        if (withServices)
        {
            var authServiceMock = new Mock<IAuthenticationService>();
            authServiceMock
                .Setup(_ => _.SignInAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<ClaimsPrincipal>(), It.IsAny<AuthenticationProperties>()))
                .Returns(Task.FromResult((object)null));
            var services = new ServiceCollection();
            services.AddControllersWithViews();
            services.AddSingleton<IAuthenticationService>(authServiceMock.Object);
            return new ControllerContext()
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new(new GenericIdentity("AUTHUSER")),
                    RequestServices = services.BuildServiceProvider()
                }
            };
        }

        return new ControllerContext()
        {
            HttpContext = new DefaultHttpContext
            {
                User = new(new GenericIdentity("AUTHUSER"))
            }
        };
    }

    internal static ControllerContext GetAuthenticatedClaimsControllerContext(bool withServices = false)
    {
        if (withServices)
        {
            var authServiceMock = new Mock<IAuthenticationService>();
            authServiceMock
                .Setup(_ => _.SignInAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<ClaimsPrincipal>(), It.IsAny<AuthenticationProperties>()))
                .Returns(Task.FromResult((object)null));
            var services = new ServiceCollection();
            services.AddControllersWithViews();
            services.AddSingleton<IAuthenticationService>(authServiceMock.Object);
            return new ControllerContext()
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity([new Claim("Username", "USERNAME")], "Basic")),
                    RequestServices = services.BuildServiceProvider()
                }
            };
        }

        return new ControllerContext()
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity([new Claim("Username", "USERNAME")], "Basic")),
            }
        };
    }

    internal static ViewComponentContext GetAuthenticatedClaimsViewComponentContext(bool withServices = false)
    {
        if (withServices)
        {
            var authServiceMock = new Mock<IAuthenticationService>();
            authServiceMock
                .Setup(_ => _.SignInAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<ClaimsPrincipal>(), It.IsAny<AuthenticationProperties>()))
                .Returns(Task.FromResult((object)null));
            var services = new ServiceCollection();
            services.AddControllersWithViews();
            services.AddSingleton<IAuthenticationService>(authServiceMock.Object);
            return new ViewComponentContext()
            {
                ViewContext = new ViewContext
                {
                    HttpContext = new DefaultHttpContext
                    {
                        User = new ClaimsPrincipal(new ClaimsIdentity([new Claim("Username", "USERNAME")], "Basic")),
                        RequestServices = services.BuildServiceProvider()
                    }
                }
            };
        }

        return new ViewComponentContext()
        {
            ViewContext = new ViewContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity([new Claim("Username", "USERNAME")], "Basic"))
                }
            }
        };
    }
}