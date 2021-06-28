# Moq.AutoMock  ![Continuous](https://github.com/moq/Moq.AutoMocker/workflows/Continuous/badge.svg) [![NuGet Status](https://img.shields.io/nuget/v/Moq.AutoMock.svg?style=flat)](https://www.nuget.org/packages/Moq.AutoMock)

An automocking container for Moq. Use this if you're invested in your IoC
container and want to decouple your unit tests from changes to their 
constructor arguments.

Usage
======

Simplest usage is to build an instance that you can unit test.

```csharp
var mocker = new AutoMocker();
var car = mocker.CreateInstance<Car>();

car.DriveTrain.ShouldNotBeNull();
car.DriveTrain.ShouldImplement<IDriveTrain>();
Mock<IDriveTrain> mock = Mock.Get(car.DriveTrain);
```

If you have a special instance that you need to use, you can register it
with `.Use(...)`. This is very similar to registrations in a regular IoC
container (i.e. `For<IService>().Use(x)` in StructureMap).

```csharp
var mocker = new AutoMocker();

mocker.Use<IDriveTrain>(new DriveTrain());
// OR, setup a Mock
mocker.Use<IDriveTrain>(x => x.Shaft.Length == 5);

var car = mocker.CreateInstance<Car>();
```

Extracting Mocks
----------------

At some point you might need to get to a mock that was auto-generated. For
this, use the `.Get<>()` or `.GetMock<>()` methods.

```csharp
var mocker = new AutoMocker();

// Let's say you have a setup that needs verifying
mocker.Use<IDriveTrain>(x => x.Accelerate(42) == true);

var car = mocker.CreateInstance<Car>();
car.Accelerate(42);

// Then extract & verify
var driveTrainMock = mocker.GetMock<IDriveTrain>();
driveTrainMock.VerifyAll();
```

Alternately, there's an even faster way to verify all mocks in the container:

```csharp
var mocker = new AutoMocker();
mocker.Use<IDriveTrain>(x => x.Accelerate(42) == true);

var car = mocker.CreateInstance<Car>();
car.Accelerate(42);

// This method verifies all mocks in the container
mocker.VerifyAll();
```

## Rider plugin

[JetBtains Rider](https://www.jetbrains.com/rider/) plugin [MoqComplete](https://plugins.jetbrains.com/plugin/12659-moqcomplete) implement autocomplete for your Setup, Callback and Return methods

### `Callback` autocompletion
![image](https://user-images.githubusercontent.com/1781005/78583584-b13c1900-783f-11ea-9ef3-4a1cd6c51cae.png)

### `Setup` method `It.` parameters autocompletion
![image](https://user-images.githubusercontent.com/1781005/78583593-b4cfa000-783f-11ea-8d08-8dac334468e2.png)

### `Return` autocompletion
![image](https://user-images.githubusercontent.com/1781005/78583626-c0bb6200-783f-11ea-98b8-f8d78c1ced64.png)

