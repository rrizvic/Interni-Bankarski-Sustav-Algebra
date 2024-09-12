using IbsaAppTeam1Pra.Controllers;
using IbsaAppTeam1Pra.Models;
using IbsaAppTeam1Pra.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Xunit;

namespace Testovi
{
    public class UserControllerTests
    {

        [Fact]
        public void CreateUser_ShouldReturnCreateProfileView()
        {
            // Arrange
            var loginVM = new LoginVM
            {
                Username = "testuser@algebra.hr",
                Password = "testpass"

            };

            var controller = new UserController();
            var users = new List<User>();

            // Act
            var result = controller.Login(loginVM) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("CreateProfile", result.ViewName);
            Assert.Empty(users);
            Assert.Equal(loginVM.Username, users[0].Username);
            Assert.Equal(loginVM.Password, users[0].Password);
        }

        [Fact]
        public void Login_ShouldReturnError_WhenUsernameDoesNotContainAlgebraDomain()
        {
            // Arrange
            var controller = new UserController();
            var loginVM = new LoginVM
            {
                Username = "testuser", // Invalid: doesn't contain "@algebra.hr"
                Password = "validPassword123"
            };

            // Act
            var result = controller.Login(loginVM) as ViewResult;

            // Assert
            Assert.False(controller.ModelState.IsValid);
            Assert.True(controller.ModelState.ContainsKey(""));
            Assert.Contains(controller.ModelState[""].Errors, e => e.ErrorMessage == "Invalid username. It must contain '@algebra.hr'.");
        }

        [Fact]
        public void Login_ShouldReturnError_WhenPasswordIsLessThanEightCharacters()
        {
            // Arrange
            var controller = new UserController();
            var loginVM = new LoginVM
            {
                Username = "user@algebra.hr",
                Password = "short" // Invalid: less than 8 characters
            };

            // Act
            var result = controller.Login(loginVM) as ViewResult;

            // Assert
            Assert.False(controller.ModelState.IsValid);
            Assert.True(controller.ModelState.ContainsKey(""));
            Assert.Contains(controller.ModelState[""].Errors, e => e.ErrorMessage == "Invalid password. It must be at least 8 characters long.");
        }



    }
}
