using System.Diagnostics;
using System.Text.Json;
using System.Text;
using Xunit;
using System.Net.Http;
using DLL.Entities;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using Moq;
using UsersAPI.Services.EntityServices.DI;
using System.Net.Mime;
using Microsoft.Net.Http.Headers;
using static System.Net.WebRequestMethods;
using System.Reflection;
using Microsoft.ApplicationInsights.Extensibility.Implementation;
using System.Security.Principal;
using UsersAPI.Services.EntityServices;
using Microsoft.Extensions.DependencyInjection;
using DLL.Abstractions;
using DLL.Repositories;
using DLL;
using UsersAPI.Services;
using Microsoft.EntityFrameworkCore;
using System.Buffers.Text;
using Microsoft.AspNetCore.Http;

namespace ApiTests
{
    public class UsersTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private const string _testWord = "tests";
        private const string _testAdminWord = "admin_tests";

        public UsersTest(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        /// <summary>
        /// trying to delete user by admin and common user
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GET_AuthorizedDelete()
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var userService = scopedServices.GetRequiredService<IUserService>();
                var hashService = scopedServices.GetRequiredService<HashService>();
                var stateSerice = scopedServices.GetRequiredService<IUserStateService>();
                var groupService = scopedServices.GetRequiredService<IUserGroupService>();
                var userToRemove = new User(_testWord, hashService.Hash(_testWord), new UserGroup(Role.User, _testWord), new UserState(State.Active, _testWord));
                await userService.CreateAsync(userToRemove);
                var admin = new User(_testAdminWord, hashService.Hash(_testWord), new UserGroup(Role.Admin, _testWord), new UserState(State.Active, _testWord));
                await userService.CreateAsync(admin);
                // sending request from common user
                using (var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost:5289/api/users/remove_by_id?id=" + userToRemove.Id))
                {
                    var httpClient = _factory.CreateClient();
                    var headers = httpClient.DefaultRequestHeaders;
                    byte[] loginPasswordBytes = Encoding.UTF8.GetBytes(_testWord + ":" + _testWord);
                    string base64 = Convert.ToBase64String(loginPasswordBytes);
                    headers.Authorization = new AuthenticationHeaderValue("Basic", base64);
                    var response = await httpClient.SendAsync(request);
                    var success = HttpStatusCode.Forbidden == response.StatusCode;
                    Assert.True(success);
                    if (success is false) return;
                }
                // sending request from admin
                using (var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost:5289/api/users/remove_by_id?id=" + userToRemove.Id))
                {
                    var httpClient = _factory.CreateClient();
                    var headers = httpClient.DefaultRequestHeaders;
                    byte[] loginPasswordBytes = Encoding.UTF8.GetBytes(_testAdminWord + ":" + _testWord);
                    string base64 = Convert.ToBase64String(loginPasswordBytes);
                    headers.Authorization = new AuthenticationHeaderValue("Basic", base64);
                    var response = await httpClient.SendAsync(request);
                    response.EnsureSuccessStatusCode();
                    var usersToRemove = new List<User> { userToRemove, admin };
                    await userService.DeleteManyAsync(usersToRemove);
                    foreach (var user in usersToRemove)
                    {
                        await stateSerice.DeleteAsync(user.State);
                        await groupService.DeleteAsync(user.Group);
                    }
                }
            }
        }



        /// <summary>
        /// trying to create two users without waiting 5 seconds after creating the first one
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task POST_CreateUsersInOneGo()
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var userService = scopedServices.GetRequiredService<IUserService>();
                var users = await userService.GetManyAsync(u => u.Login == _testWord);
                var hashService = scopedServices.GetRequiredService<HashService>();
                var stateSerice = scopedServices.GetRequiredService<IUserStateService>();
                var groupService = scopedServices.GetRequiredService<IUserGroupService>();

                byte[] loginPasswordBytes = Encoding.UTF8.GetBytes(_testWord + ":" + _testWord);
                string base64 = Convert.ToBase64String(loginPasswordBytes);
                for (int i = 0; i < 2; i++)
                {
                    using (var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:5289/api/users/create"))
                    {
                        var httpClient = _factory.CreateClient();
                        var userToCreate = new User(_testWord, hashService.Hash(_testWord), new UserGroup(Role.User, _testWord), new UserState(State.Active, _testWord));
                        var json = System.Text.Json.JsonSerializer.Serialize(userToCreate);
                        var headers = httpClient.DefaultRequestHeaders;
                        request.Content = new StringContent(json, Encoding.UTF8, "application/json");
                        var response = await httpClient.SendAsync(request);
                        response.EnsureSuccessStatusCode();
                    }
                }
                var refreshedUsers = await userService.GetManyAsync(u => u.Login == _testWord && u.State.State == State.Active);
                Assert.True(refreshedUsers.Count() - users.Count() == 1);
                await userService.DeleteManyAsync(refreshedUsers);
                foreach (var user in refreshedUsers)
                {
                    await stateSerice.DeleteAsync(user.State);
                    await groupService.DeleteAsync(user.Group);
                }
            }
        }
    }
}
