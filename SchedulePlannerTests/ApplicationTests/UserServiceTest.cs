using EntityFramework.Exceptions.Common;
using SchedulePlannerBack.Domain.Entity;
using SchedulePlannerBack.Exceptions;
using SchedulePlannerBack.Interfaces;
using SchedulePlannerBack.Service;

namespace SchedulePlannerTests.ApplicationTests;

using System;
using Moq;
using NUnit.Framework;
using Microsoft.Extensions.Logging;

[TestFixture]
public class UserServiceTest
{
    private Mock<ILogger> _loggerMock;
    private Mock<IUserRepository> _userRepositoryMock;
    private UserService _userService;

    [SetUp]
    public void Setup()
    {
        _loggerMock = new Mock<ILogger>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _userService = new UserService(
            _userRepositoryMock.Object,
            _loggerMock.Object);
    }

    [Test]
    public void GetUserById_CallsRepositoryGetById()
    {
        var user = CreateUser(1);

        _userRepositoryMock
            .Setup(x => x.GetById(user.Id))
            .Returns(user);

        var result = _userService.GetUserById(user.Id);

        _userRepositoryMock.Verify(
            x => x.GetById(user.Id),
            Times.Once);

        Assert.That(Equals(user, result));
    }

    [Test]
    public void GetUserById_ThrowsElementNotFoundException_WhenRepositoryReturnsNull()
    {
        long userId = 1;

        _userRepositoryMock
            .Setup(x => x.GetById(userId))
            .Returns((User?)null);

        Assert.Throws<ElementNotFoundException>(() =>
            _userService.GetUserById(userId));
    }

    [Test]
    public void GetUserById_ThrowsStorageException_WhenRepositoryThrowsStorageException()
    {
        long userId = 1;

        _userRepositoryMock
            .Setup(x => x.GetById(It.IsAny<long>()))
            .Throws(new StorageException(new Exception("db error")));

        Assert.Throws<StorageException>(() =>
            _userService.GetUserById(userId));
    }
    [Test]
    public void GetUserById_ThrowsStorageException_WhenRepositoryThrowsUniqueConstraintException()
    {
        long userId = 1;

        _userRepositoryMock
            .Setup(x => x.GetById(It.IsAny<long>()))
            .Throws(new UniqueConstraintException());

        Assert.Throws<UniqueConstraintException>(() =>
            _userService.GetUserById(userId));
    }

    private bool Equals(User user, User userToCheck)
    {
        return user.Id == userToCheck.Id 
            && user.Login == userToCheck.Login
            && user.Password == userToCheck.Password;
    }

    private User CreateUser(long id)
    {
        return new User
        {
            Id = id,
            Login = "testuser",
            Password = "password",
        };
    }
}
