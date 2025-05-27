using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using WHLAdmin.Common.Models;
using WHLAdmin.Common.Repositories;
using WHLAdmin.Services;
using WHLAdmin.ViewModels;

namespace WHLAdmin.Tests.Services;

public class MarketingAgentsServiceTests()
{
    private readonly Mock<ILogger<MarketingAgentsService>> _logger = new();
    private readonly Mock<IMarketingAgentRepository> _agentRepository = new();
    private readonly Mock<IUsersService> _usersService = new();

    [Fact]
    public void ConstructorTests()
    {
        // ArgumentNullExceptions
        Assert.Throws<ArgumentNullException>(() => new MarketingAgentsService(null, null, null));
        Assert.Throws<ArgumentNullException>(() => new MarketingAgentsService(_logger.Object, null, null));
        Assert.Throws<ArgumentNullException>(() => new MarketingAgentsService(_logger.Object, _agentRepository.Object, null));

        // Not Null
        var actual = new MarketingAgentsService(_logger.Object, _agentRepository.Object, _usersService.Object);
        Assert.NotNull(actual);
    }

    [Fact]
    public async Task GetDataTests()
    {
        // Setup
        var agentRepositoryEmpty = new Mock<IMarketingAgentRepository>();
        agentRepositoryEmpty.Setup(s => s.GetAll()).ReturnsAsync([]);

        var agentRepositoryNonEmpty = new Mock<IMarketingAgentRepository>();
        agentRepositoryNonEmpty.Setup(s => s.GetAll()).ReturnsAsync(
        [
            new()
            {
                AgentId = 1, Name = "NAME", Active = true
            }
        ]);

        // Empty
        var service = new MarketingAgentsService(_logger.Object, _agentRepository.Object, _usersService.Object);
        var actual = await service.GetData(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
        Assert.NotNull(actual);
        Assert.Empty(actual.Agents);

        // Not Empty
        service = new MarketingAgentsService(_logger.Object, agentRepositoryNonEmpty.Object, _usersService.Object);
        actual = await service.GetData(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
        Assert.NotNull(actual);
        Assert.NotEmpty(actual.Agents);
    }

    [Fact]
    public async Task GetAllTests()
    {
        // Setup
        var agentRepositoryEmpty = new Mock<IMarketingAgentRepository>();
        agentRepositoryEmpty.Setup(s => s.GetAll()).ReturnsAsync([]);

        var agentRepositoryNonEmpty = new Mock<IMarketingAgentRepository>();
        agentRepositoryNonEmpty.Setup(s => s.GetAll()).ReturnsAsync(
        [
            new()
            {
                AgentId = 1, Name = "NAME", Active = true
            }
        ]);

        // Empty
        var service = new MarketingAgentsService(_logger.Object, agentRepositoryEmpty.Object, _usersService.Object);
        var actual = await service.GetAll(It.IsAny<string>(), It.IsAny<string>());
        Assert.Empty(actual);

        // Not Empty
        service = new MarketingAgentsService(_logger.Object, agentRepositoryNonEmpty.Object, _usersService.Object);
        actual = await service.GetAll(It.IsAny<string>(), It.IsAny<string>());
        Assert.NotEmpty(actual);
    }

    [Fact]
    public async Task GetOneTests()
    {
        // Setup
        var agentRepositoryNull = new Mock<IMarketingAgentRepository>();
        agentRepositoryNull.Setup(s => s.GetOne(It.IsAny<MarketingAgent>())).ReturnsAsync((MarketingAgent)null);

        var agentRepositoryNotNull = new Mock<IMarketingAgentRepository>();
        agentRepositoryNotNull.Setup(s => s.GetOne(It.IsAny<MarketingAgent>())).ReturnsAsync(new MarketingAgent()
        {
            AgentId = 1, Name = "NAME", Active = true
        });

        // Null
        var service = new MarketingAgentsService(_logger.Object, agentRepositoryNull.Object, _usersService.Object);
        var actual = await service.GetOne(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>());
        Assert.Null(actual);

        // Not Empty
        service = new MarketingAgentsService(_logger.Object, agentRepositoryNotNull.Object, _usersService.Object);
        actual = await service.GetOne(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>());
        Assert.NotNull(actual);
        Assert.Equal(1, actual.AgentId);
        Assert.Equal("NAME", actual.Name);
    }

    [Fact]
    public void GetOneForAddTests()
    {
        var service = new MarketingAgentsService(_logger.Object, _agentRepository.Object, _usersService.Object);
        var actual = service.GetOneForAdd(It.IsAny<string>(), It.IsAny<string>());
        Assert.NotNull(actual);
        Assert.Equal(0, actual.AgentId);
        Assert.Empty(actual.AgentName);
        Assert.Empty(actual.ContactName);
        Assert.Empty(actual.PhoneNumber);
        Assert.Empty(actual.EmailAddress);
        Assert.True(actual.Active);
    }

    [Fact]
    public async Task GetOneForEditTests()
    {
        // Setup
        var agentRepositoryNull = new Mock<IMarketingAgentRepository>();
        agentRepositoryNull.Setup(s => s.GetOne(It.IsAny<MarketingAgent>())).ReturnsAsync((MarketingAgent)null);

        var agentRepositoryNotNull = new Mock<IMarketingAgentRepository>();
        agentRepositoryNotNull.Setup(s => s.GetOne(It.IsAny<MarketingAgent>())).ReturnsAsync(new MarketingAgent()
        {
            AgentId = 1, Name = "NAME", Active = true
        });

        // Null
        var service = new MarketingAgentsService(_logger.Object, agentRepositoryNull.Object, _usersService.Object);
        var actual = await service.GetOneForEdit(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>());
        Assert.Null(actual);

        // Not Empty
        service = new MarketingAgentsService(_logger.Object, agentRepositoryNotNull.Object, _usersService.Object);
        actual = await service.GetOneForEdit(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>());
        Assert.NotNull(actual);
        Assert.Equal(1, actual.AgentId);
        Assert.Equal("NAME", actual.AgentName);
    }

    [Fact]
    public void SanitizeNullTest()
    {
        MarketingAgent agent = null;
        var service = new MarketingAgentsService(_logger.Object, _agentRepository.Object, _usersService.Object);
        service.Sanitize(agent);
        Assert.Null(agent);
    }

    [Theory]
    [InlineData(null, null, "", null)]
    [InlineData("", null, "", null)]
    [InlineData(" ", null, "", null)]
    [InlineData("NAME", null, "NAME", null)]
    [InlineData("NAME ", null, "NAME", null)]
    [InlineData(" NAME", null, "NAME", null)]
    [InlineData(" NAME ", null, "NAME", null)]
    [InlineData("NAME", "", "NAME", null)]
    [InlineData("NAME", " ", "NAME", null)]
    [InlineData("NAME", "CONTACT", "NAME", "CONTACT")]
    [InlineData("NAME", "CONTACT ", "NAME", "CONTACT")]
    [InlineData("NAME", " CONTACT", "NAME", "CONTACT")]
    [InlineData("NAME", " CONTACT ", "NAME", "CONTACT")]
    public void SanitizeObjectTests(string name, string contactName, string expectedName, string expectedDescription)
    {
        var agent = new MarketingAgent() { Name = name, ContactName = contactName };
        var service = new MarketingAgentsService(_logger.Object, _agentRepository.Object, _usersService.Object);
        service.Sanitize(agent);
        Assert.NotNull(agent);
        Assert.Equal(expectedName, agent.Name);
        Assert.Equal(expectedDescription, agent.ContactName);
    }

    [Fact]
    public void ValidateTests()
    {
        // Null Test
        var service = new MarketingAgentsService(_logger.Object, _agentRepository.Object, _usersService.Object);
        var actual = service.Validate(It.IsAny<string>(), It.IsAny<string>(), null, null);
        Assert.Equal("MA000", actual);

        // Null Name Test
        service = new MarketingAgentsService(_logger.Object, _agentRepository.Object, _usersService.Object);
        actual = service.Validate(It.IsAny<string>(), It.IsAny<string>(), new MarketingAgentViewModel(), null);
        Assert.Equal("MA101", actual);

        // Empty Name Test
        service = new MarketingAgentsService(_logger.Object, _agentRepository.Object, _usersService.Object);
        actual = service.Validate(It.IsAny<string>(), It.IsAny<string>(), new MarketingAgentViewModel() { Name = "" }, null);
        Assert.Equal("MA101", actual);

        // Spaces Name Test
        service = new MarketingAgentsService(_logger.Object, _agentRepository.Object, _usersService.Object);
        actual = service.Validate(It.IsAny<string>(), It.IsAny<string>(), new MarketingAgentViewModel() { Name = "  " }, null);
        Assert.Equal("MA101", actual);

        // Valid Name, Null existing amenities Test
        service = new MarketingAgentsService(_logger.Object, _agentRepository.Object, _usersService.Object);
        actual = service.Validate(It.IsAny<string>(), It.IsAny<string>(), new MarketingAgentViewModel() { Name = "NAME" }, null);
        Assert.Empty(actual);

        // Valid Name, Empty existing amenities Test
        service = new MarketingAgentsService(_logger.Object, _agentRepository.Object, _usersService.Object);
        actual = service.Validate(It.IsAny<string>(), It.IsAny<string>(), new MarketingAgentViewModel() { Name = "NAME" }, []);
        Assert.Empty(actual);

        // Duplicate Check FAIL for ADD Test
        service = new MarketingAgentsService(_logger.Object, _agentRepository.Object, _usersService.Object);
        actual = service.Validate(It.IsAny<string>(), It.IsAny<string>(), new MarketingAgentViewModel() { AgentId = 0, Name = "NAME" },
        [
            new()
            {
                AgentId = 1, Name = "NAME", Active = true
            }
        ]);
        Assert.Equal("MA002", actual);

        // Duplicate Check SUCCESS for ADD Test
        service = new MarketingAgentsService(_logger.Object, _agentRepository.Object, _usersService.Object);
        actual = service.Validate(It.IsAny<string>(), It.IsAny<string>(), new MarketingAgentViewModel() { AgentId = 0, Name = "NAME" },
        [
            new()
            {
                AgentId = 1, Name = "ANOTHERNAME", Active = true
            }
        ]);
        Assert.Empty(actual);

        // Existence Check FAIL for UPDATE Test
        service = new MarketingAgentsService(_logger.Object, _agentRepository.Object, _usersService.Object);
        actual = service.Validate(It.IsAny<string>(), It.IsAny<string>(), new MarketingAgentViewModel() { AgentId = 1, Name = "NAME" },
        [
            new()
            {
                AgentId = 2, Name = "NAME", Active = true
            }
        ]);
        Assert.Equal("MA001", actual);

        // Existence Check SUCCESS for UPDATE Test
        service = new MarketingAgentsService(_logger.Object, _agentRepository.Object, _usersService.Object);
        actual = service.Validate(It.IsAny<string>(), It.IsAny<string>(), new MarketingAgentViewModel() { AgentId = 1, Name = "NAME" },
        [
            new()
            {
                AgentId = 1, Name = "NAME", Active = true
            }
        ]);
        Assert.Empty(actual);

        // Duplicate Check FAIL for UPDATE Test
        service = new MarketingAgentsService(_logger.Object, _agentRepository.Object, _usersService.Object);
        actual = service.Validate(It.IsAny<string>(), It.IsAny<string>(), new MarketingAgentViewModel() { AgentId = 1, Name = "NEWNAME" },
        [
            new()
            {
                AgentId = 1, Name = "NAME", Active = true
            },
            new()
            {
                AgentId = 2, Name = "NEWNAME", Active = true
            }
        ]);
        Assert.Equal("MA002", actual);

        // Duplicate Check SUCCESS for UPDATE Test
        service = new MarketingAgentsService(_logger.Object, _agentRepository.Object, _usersService.Object);
        actual = service.Validate(It.IsAny<string>(), It.IsAny<string>(), new MarketingAgentViewModel() { AgentId = 1, Name = "NEWNAME" },
        [
            new()
            {
                AgentId = 1, Name = "NAME", Active = true
            },
            new()
            {
                AgentId = 2, Name = "ANOTHERNAME", Active = true
            }
        ]);
        Assert.Empty(actual);
    }

    [Fact]
    public async Task AddTests()
    {
        // Setup
        var agentToAdd = new EditableMarketingAgentViewModel()
        {
            Active = true, AgentId = 0, AgentName = "NAME"
        };

        var agentRepositoryException = new Mock<IMarketingAgentRepository>();
        agentRepositoryException.Setup(s => s.GetAll()).ThrowsAsync(new Exception() {});

        var agentRepositoryFailure = new Mock<IMarketingAgentRepository>();
        agentRepositoryFailure.Setup(s => s.GetAll()).ReturnsAsync([]);
        agentRepositoryFailure.Setup(s => s.Add(It.IsAny<string>(), It.IsAny<MarketingAgent>())).ReturnsAsync(false);

        var agentRepositorySuccess = new Mock<IMarketingAgentRepository>();
        agentRepositorySuccess.Setup(s => s.GetAll()).ReturnsAsync([]);
        agentRepositorySuccess.Setup(s => s.Add(It.IsAny<string>(), It.IsAny<MarketingAgent>())).ReturnsAsync(true);

        // Null Test
        var service = new MarketingAgentsService(_logger.Object, _agentRepository.Object, _usersService.Object);
        var actual = await service.Add(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), null);
        Assert.Equal("MA000", actual);

        // Validation FAIL Tests
        actual = await service.Add(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), new EditableMarketingAgentViewModel());
        Assert.Equal("MA101", actual);
        actual = await service.Add(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), new EditableMarketingAgentViewModel() { AgentName = "" });
        Assert.Equal("MA101", actual);
        actual = await service.Add(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), new EditableMarketingAgentViewModel() { AgentName = "   " });
        Assert.Equal("MA101", actual);

        // Add Exception
        service = new MarketingAgentsService(_logger.Object, agentRepositoryException.Object, _usersService.Object);
        await Assert.ThrowsAnyAsync<Exception>(() => service.Add(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), agentToAdd));

        // Add Failure
        service = new MarketingAgentsService(_logger.Object, agentRepositoryFailure.Object, _usersService.Object);
        actual = await service.Add(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), agentToAdd);
        Assert.Equal("MA003", actual);

        // Add Success
        service = new MarketingAgentsService(_logger.Object, agentRepositorySuccess.Object, _usersService.Object);
        actual = await service.Add(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), agentToAdd);
        Assert.Empty(actual);
    }

    [Fact]
    public async Task UpdateTests()
    {
        // Setup
        var agentToUpdate = new EditableMarketingAgentViewModel()
        {
            Active = true, AgentId = 1, AgentName = "NAME"
        };
        var existingMarketingAgent = new MarketingAgent()
        {
            Active = true, AgentId = 1, Name = "NAME"
        };

        var agentRepositoryException = new Mock<IMarketingAgentRepository>();
        agentRepositoryException.Setup(s => s.GetAll()).ThrowsAsync(new Exception() {});

        var agentRepositoryFailure = new Mock<IMarketingAgentRepository>();
        agentRepositoryFailure.Setup(s => s.GetAll()).ReturnsAsync([ existingMarketingAgent ]);
        agentRepositoryFailure.Setup(s => s.Update(It.IsAny<string>(), It.IsAny<MarketingAgent>())).ReturnsAsync(false);

        var agentRepositorySuccess = new Mock<IMarketingAgentRepository>();
        agentRepositorySuccess.Setup(s => s.GetAll()).ReturnsAsync([ existingMarketingAgent ]);
        agentRepositorySuccess.Setup(s => s.Update(It.IsAny<string>(), It.IsAny<MarketingAgent>())).ReturnsAsync(true);

        // Null Test
        var service = new MarketingAgentsService(_logger.Object, _agentRepository.Object, _usersService.Object);
        var actual = await service.Update(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), null);
        Assert.Equal("MA000", actual);

        // Validation FAIL Tests
        actual = await service.Update(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), new EditableMarketingAgentViewModel());
        Assert.Equal("MA101", actual);
        actual = await service.Update(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), new EditableMarketingAgentViewModel() { AgentName = "" });
        Assert.Equal("MA101", actual);
        actual = await service.Update(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), new EditableMarketingAgentViewModel() { AgentName = "   " });
        Assert.Equal("MA101", actual);

        // Update Exception
        service = new MarketingAgentsService(_logger.Object, agentRepositoryException.Object, _usersService.Object);
        await Assert.ThrowsAnyAsync<Exception>(() => service.Update(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), agentToUpdate));

        // Update Failure
        service = new MarketingAgentsService(_logger.Object, agentRepositoryFailure.Object, _usersService.Object);
        actual = await service.Update(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), agentToUpdate);
        Assert.Equal("MA004", actual);

        // Update Success
        service = new MarketingAgentsService(_logger.Object, agentRepositorySuccess.Object, _usersService.Object);
        actual = await service.Update(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), agentToUpdate);
        Assert.Empty(actual);
    }

    [Fact]
    public async Task DeleteTests()
    {
        // Invalid Input Tests
        var service = new MarketingAgentsService(_logger.Object, _agentRepository.Object, _usersService.Object);
        var actual = await service.Delete(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), -1);
        Assert.Equal("MA000", actual);
        actual = await service.Delete(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), 0);
        Assert.Equal("MA000", actual);

        // Setup
        var agentRepositoryNull = new Mock<IMarketingAgentRepository>();
        agentRepositoryNull.Setup(s => s.GetOne(It.IsAny<MarketingAgent>())).ReturnsAsync((MarketingAgent)null);

        var agentRepositoryException = new Mock<IMarketingAgentRepository>();
        agentRepositoryException.Setup(s => s.GetOne(It.IsAny<MarketingAgent>())).ThrowsAsync(new Exception() {});

        var agentRepositoryFailure = new Mock<IMarketingAgentRepository>();
        agentRepositoryFailure.Setup(s => s.GetOne(It.IsAny<MarketingAgent>())).ReturnsAsync(new MarketingAgent()
        {
            AgentId = 1, Name = "NAME", Active = true
        });
        agentRepositoryFailure.Setup(s => s.Delete(It.IsAny<string>(), It.IsAny<MarketingAgent>())).ReturnsAsync(false);

        var agentRepositorySuccess = new Mock<IMarketingAgentRepository>();
        agentRepositorySuccess.Setup(s => s.GetOne(It.IsAny<MarketingAgent>())).ReturnsAsync(new MarketingAgent()
        {
            AgentId = 1, Name = "NAME", Active = true
        });
        agentRepositorySuccess.Setup(s => s.Delete(It.IsAny<string>(), It.IsAny<MarketingAgent>())).ReturnsAsync(true);

        // Not Found Test
        service = new MarketingAgentsService(_logger.Object, agentRepositoryNull.Object, _usersService.Object);
        actual = await service.Delete(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), 1);
        Assert.Equal("MA001", actual);

        // Delete Exception
        service = new MarketingAgentsService(_logger.Object, agentRepositoryException.Object, _usersService.Object);
        await Assert.ThrowsAnyAsync<Exception>(() => service.Delete(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), 1));

        // Delete Failure
        service = new MarketingAgentsService(_logger.Object, agentRepositoryFailure.Object, _usersService.Object);
        actual = await service.Delete(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), 1);
        Assert.Equal("MA005", actual);

        // Delete Success
        service = new MarketingAgentsService(_logger.Object, agentRepositorySuccess.Object, _usersService.Object);
        actual = await service.Delete(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), 1);
        Assert.Empty(actual);
    }
}