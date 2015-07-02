namespace SIM.Commands
{
  using System;
  using NSubstitute;
  using SIM.Abstract.Connection;
  using SIM.Abstract.Services;
  using SIM.Extensions;
  using SIM.Services;
  using Xunit;

  public class ServiceAddCommandTests
  {
    [Fact]
    public void ServiceAddCommand_Execute_NoArguments()
    {                                                 
      // arrange
      var connectionFactory = Substitute.For<IConnectionStringFactory>().IsNotNull();
      var serviceStore = Substitute.For<IServiceStore>().IsNotNull();
      
      var sut = new ServiceAddCommand(connectionFactory, serviceStore);

      // act
      var result = sut.Execute();

      // assert
      Assert.Equal(false, result.Success);
      Assert.NotEqual(null, result.Error);
      Assert.Equal(typeof(ArgumentNullException).FullName, result.Error.ClassName);

      // ReSharper disable once PossibleNullReferenceException
      // ReSharper disable once AssignNullToNotNullAttribute
      serviceStore.DidNotReceiveWithAnyArgs().Add(null);
    }

    [Fact]
    public void ServiceAddCommand_Execute_WithArguments_ServiceType()
    {
      // arrange
      var connectionFactory = Substitute.For<IConnectionStringFactory>().IsNotNull();
      var serviceStore = Substitute.For<IServiceStore>().IsNotNull();

      var sut = new ServiceAddCommand(connectionFactory, serviceStore)
      {
        ServiceType = ServiceType.SqlServer
      };

      // act
      var result = sut.Execute();

      // assert
      Assert.Equal(false, result.Success);
      Assert.NotEqual(null, result.Error);
      Assert.Equal(typeof(ArgumentNullException).FullName, result.Error.ClassName);

      // ReSharper disable once PossibleNullReferenceException
      // ReSharper disable once AssignNullToNotNullAttribute
      serviceStore.DidNotReceiveWithAnyArgs().Add(null);
    }

    [Fact]
    public void ServiceAddCommand_Execute_WithArguments_ServiceName()
    {
      // arrange
      var connectionFactory = Substitute.For<IConnectionStringFactory>().IsNotNull();
      var serviceStore = Substitute.For<IServiceStore>().IsNotNull();

      var sut = new ServiceAddCommand(connectionFactory, serviceStore)
      {
        ServiceName = "Test"
      };

      // act
      var result = sut.Execute();

      // assert
      Assert.Equal(false, result.Success);
      Assert.NotEqual(null, result.Error);
      Assert.Equal(typeof(ArgumentNullException).FullName, result.Error.ClassName);

      // ReSharper disable once PossibleNullReferenceException
      // ReSharper disable once AssignNullToNotNullAttribute
      serviceStore.DidNotReceiveWithAnyArgs().Add(null);
    }

    [Fact]
    public void ServiceAddCommand_Execute_WithArguments_ServiceType_ServiceName()
    {
      // arrange
      var connectionFactory = Substitute.For<IConnectionStringFactory>().IsNotNull();
      var serviceStore = Substitute.For<IServiceStore>().IsNotNull();

      var sut = new ServiceAddCommand(connectionFactory, serviceStore)
      {
        ServiceType = ServiceType.SqlServer,
        ServiceName = "Test"        
      };

      // act
      var result = sut.Execute();

      // assert
      Assert.Equal(false, result.Success);
      Assert.NotEqual(null, result.Error);
      Assert.Equal(typeof(ArgumentNullException).FullName, result.Error.ClassName);

      serviceStore.DidNotReceiveWithAnyArgs().Add(null);
    }

    [Fact]
    public void ServiceAddCommand_Execute_WithArguments_All_InvalidSqlServerConnectionString()
    {
      // arrange
      var connectionFactory = Substitute.For<IConnectionStringFactory>().IsNotNull();
      var serviceStore = Substitute.For<IServiceStore>().IsNotNull();

      var sut = new ServiceAddCommand(connectionFactory, serviceStore)
      {
        ServiceType = ServiceType.SqlServer,
        ServiceName = "Test"                  ,
        ConnectionString = "Invalid Connection String 123"
      };

      // act
      var result = sut.Execute();

      // assert
      Assert.Equal(false, result.Success);
      Assert.NotEqual(null, result.Error);
      Assert.Equal(typeof(ArgumentException).FullName, result.Error.ClassName);

      serviceStore.DidNotReceiveWithAnyArgs().Add(null);
    }

    [Fact]
    public void ServiceAddCommand_Execute_WithArguments_All_Valid()
    {
      // arrange
      var connectionFactory = Substitute.For<IConnectionStringFactory>().IsNotNull();
      var serviceStore = Substitute.For<IServiceStore>().IsNotNull();      

      var sut = new ServiceAddCommand(connectionFactory, serviceStore)
      {
        ServiceType = ServiceType.SqlServer,
        ServiceName = "Test",
        ConnectionString = "Data Source=."
      };

      // act
      var result = sut.Execute();

      // assert
      Assert.Equal(true, result.Success);
      Assert.Equal(null, result.Error);

      serviceStore.Received().Exists("Test");
      serviceStore.ReceivedWithAnyArgs(1).Add(null);
    }

    [Fact]
    public void ServiceAddCommand_Execute_WithArguments_All_AlreadyExists()
    {
      // arrange
      var connectionFactory = Substitute.For<IConnectionStringFactory>().IsNotNull();
      var serviceStore = Substitute.For<IServiceStore>().IsNotNull();

      serviceStore.Exists("Test").Returns(true);

      var sut = new ServiceAddCommand(connectionFactory, serviceStore)
      {
        ServiceType = ServiceType.SqlServer,
        ServiceName = "Test",
        ConnectionString = "Data Source=."
      };

      // act
      var result = sut.Execute();

      // assert
      Assert.Equal(false, result.Success);
      Assert.Equal(null, result.Error);

      serviceStore.Received().Exists("Test");
      serviceStore.DidNotReceiveWithAnyArgs().Add(null);
    }
  }
}