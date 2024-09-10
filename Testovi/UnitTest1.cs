using IbsaAppTeam1Pra.Controllers;
using IbsaAppTeam1Pra.Models;
using IbsaAppTeam1Pra.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Xunit;

namespace Testovi
{
    public class UnitTest1
    {

            [Fact]
            public void CreateUser_ShouldReturnCreateProfileView()
            {
                // Arrange
                var loginVM = new LoginVM
                {
                    Username = "testuser",
                    Password = "testpassword"
                };

                var controller = new UserController();
                var users = new List<User>();

                // Act
                var result = controller.Login(loginVM) as ViewResult;

                // Assert
                Assert.NotNull(result);
                Assert.Equal("CreateProfile", result.ViewName);
                Assert.Single(users);
                Assert.Equal(loginVM.Username, users[0].Username);
                Assert.Equal(loginVM.Password, users[0].Password);
            }
        }
    }
