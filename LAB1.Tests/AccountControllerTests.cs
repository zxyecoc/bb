using Xunit;
using Moq;
using LAB1.Controllers;  
using LAB1.Models;
using Microsoft.AspNetCore.Mvc;
using LAB1.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace LAB1.Tests.ControllersTests
{
    public class AccountControllerTests
    {

        [Fact]
        public async Task Register_ValidModel_ReturnsRedirectToActionResult()
        {
            var userManagerMock = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
            var signInManagerMock = new Mock<SignInManager<User>>(userManagerMock.Object, Mock.Of<IHttpContextAccessor>(), Mock.Of<IUserClaimsPrincipalFactory<User>>(), null, null, null, null);

            var options = new DbContextOptionsBuilder<NewsBlogContext>()
               .UseInMemoryDatabase("TestDatabase")
               .Options;

            var context = new NewsBlogContext(options);  

            var controller = new UserController(context, userManagerMock.Object, signInManagerMock.Object);

            var validModel = new RegisterViewModel
            {
                UserName = "john.doe",
                Email = "john.doe@example.com",
                Password = "Password123!",
                ConfirmPassword = "Password123!"
            };

            userManagerMock.Setup(x => x.CreateAsync(It.IsAny<User>(), validModel.Password)).ReturnsAsync(IdentityResult.Success);

            var result = await controller.Register(validModel) as RedirectToActionResult;

            Assert.NotNull(result);
            Assert.Equal("Index", result.ActionName);  
            Assert.Equal("Home", result.ControllerName);  

            userManagerMock.Verify(x => x.CreateAsync(It.IsAny<User>(), validModel.Password), Times.Once);  
        }

        [Fact]
        public async Task Register_InvalidModel_ReturnsViewWithErrors()
        {
            var userManagerMock = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
            var signInManagerMock = new Mock<SignInManager<User>>(userManagerMock.Object, Mock.Of<IHttpContextAccessor>(), Mock.Of<IUserClaimsPrincipalFactory<User>>(), null, null, null, null);

            var options = new DbContextOptionsBuilder<NewsBlogContext>()
               .UseInMemoryDatabase("TestDatabase")
               .Options;

            var context = new NewsBlogContext(options);  // Використовуємо реальну in-memory базу даних

            var controller = new UserController(context, userManagerMock.Object, signInManagerMock.Object);

            var invalidModel = new RegisterViewModel
            {
                UserName = "john.doe",
                Email = "john.doe@example.com",
                Password = "",  
                ConfirmPassword = ""
            };

            controller.ModelState.AddModelError("Password", "Password is required");

            var result = await controller.Register(invalidModel) as ViewResult;

            Assert.NotNull(result);
            Assert.Equal("Register", result.ViewName);  
            Assert.False(controller.ModelState.IsValid);  

            var model = Assert.IsType<RegisterViewModel>(result.Model);
            Assert.Equal(invalidModel.UserName, model.UserName); 
            Assert.Equal(invalidModel.Email, model.Email);       

            Assert.True(controller.ModelState["Password"].Errors.Count > 0);
            Assert.Equal("Password is required", controller.ModelState["Password"].Errors[0].ErrorMessage);

            userManagerMock.Verify(x => x.CreateAsync(It.IsAny<User>(), invalidModel.Password), Times.Never);  
        }

        [Fact]
        public async Task Register_NullModel_ReturnsViewWithErrors()
        {
            var userManagerMock = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
            var signInManagerMock = new Mock<SignInManager<User>>(userManagerMock.Object, Mock.Of<IHttpContextAccessor>(), Mock.Of<IUserClaimsPrincipalFactory<User>>(), null, null, null, null);

            var options = new DbContextOptionsBuilder<NewsBlogContext>()
               .UseInMemoryDatabase("TestDatabase")
               .Options;

            var context = new NewsBlogContext(options);  

            var controller = new UserController(context, userManagerMock.Object, signInManagerMock.Object);

            var result = await controller.Register(null) as ViewResult;

            Assert.NotNull(result);
            Assert.Equal("Register", result.ViewName);  
            Assert.Null(result.Model);  
        }
    }
}
