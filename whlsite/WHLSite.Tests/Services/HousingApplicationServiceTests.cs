// using System;
// using System.Linq;
// using Microsoft.Extensions.Logging;
// using Moq;
// using WHLSite.Common.Repositories;
// using WHLSite.Services;

// namespace WHLSite.Tests.Services;

// public class HousingApplicationServiceTests
// {
//     private readonly Mock<ILogger<HousingApplicationService>> _logger = new();
//     private readonly Mock<IHousingApplicationRepository> _applicationRepository = new();

//     [Fact]
//     public void ConstructorTests()
//     {
//         // ArgumentNullExceptions
//         Assert.Throws<ArgumentNullException>(() => new HousingApplicationService(null, null));
//         Assert.Throws<ArgumentNullException>(() => new HousingApplicationService(_logger.Object, null));

//         // Not Null
//         var actual = new HousingApplicationService(_logger.Object, _applicationRepository.Object);
//         Assert.NotNull(actual);
//     }

//     [Fact]
//     public async void GetDashboardTests()
//     {
//         // Exception
//         var applicationRepository = new Mock<IHousingApplicationRepository>();
//         applicationRepository.Setup(s => s.GetAll(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ThrowsAsync(new Exception() {});
//         var service = new HousingApplicationService(_logger.Object, applicationRepository.Object);
//         await Assert.ThrowsAnyAsync<Exception>(() => service.GetForDashboard(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));

//         // Null applications
//         service = new HousingApplicationService(_logger.Object, _applicationRepository.Object);
//         var actual = await service.GetForDashboard(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
//         Assert.NotNull(actual);
//         Assert.Empty(actual.Applications);

//         // No applications
//         applicationRepository.Setup(s => s.GetAll(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync([]);
//         service = new HousingApplicationService(_logger.Object, applicationRepository.Object);
//         actual = await service.GetForDashboard(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
//         Assert.NotNull(actual);
//         Assert.Empty(actual.Applications);

//         // With applications
//         applicationRepository.Setup(s => s.GetAll(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync([ new() { ApplicationId = 1 }]);
//         service = new HousingApplicationService(_logger.Object, applicationRepository.Object);
//         actual = await service.GetForDashboard(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
//         Assert.NotNull(actual);
//         Assert.NotEmpty(actual.Applications);
//         Assert.Equal(1, actual.Applications.First().ApplicationId);
//     }
// }