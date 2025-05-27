using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using WHLAdmin.Common.Models;
using WHLAdmin.Common.Repositories;
using WHLAdmin.Services;
using WHLAdmin.ViewModels;

namespace WHLAdmin.Tests.Services;

public class NotificationConfigsServiceTests()
{
    private readonly Mock<ILogger<NotificationConfigsService>> _logger = new();
    private readonly Mock<INotificationConfigRepository> _notificationConfigRepository = new();
    private readonly Mock<IMetadataService> _metadataService = new();
    private readonly Mock<IUsersService> _usersService = new();

    [Fact]
    public void ConstructorTests()
    {
        // ArgumentNullExceptions
        Assert.Throws<ArgumentNullException>(() => new NotificationConfigsService(null, null, null, null));
        Assert.Throws<ArgumentNullException>(() => new NotificationConfigsService(_logger.Object, null, null, null));
        Assert.Throws<ArgumentNullException>(() => new NotificationConfigsService(_logger.Object, _notificationConfigRepository.Object, null, null));
        Assert.Throws<ArgumentNullException>(() => new NotificationConfigsService(_logger.Object, _notificationConfigRepository.Object, _metadataService.Object, null));

        // Not Null
        var actual = new NotificationConfigsService(_logger.Object, _notificationConfigRepository.Object, _metadataService.Object, _usersService.Object);
        Assert.NotNull(actual);
    }

    [Fact]
    public async Task GetDataTests()
    {
        // Setup
        var notificationConfigRepositoryEmpty = new Mock<INotificationConfigRepository>();
        notificationConfigRepositoryEmpty.Setup(s => s.GetAll()).ReturnsAsync([]);

        var notificationConfigRepositoryNonEmpty = new Mock<INotificationConfigRepository>();
        notificationConfigRepositoryNonEmpty.Setup(s => s.GetAll()).ReturnsAsync(
        [
            new()
            {
                NotificationId = 1, CategoryCd = "CATCD",
                Title = "TITLE", Text = "TEXT",
                FrequencyCd = "FREQCD", FrequencyInterval = 0,
                NotificationList = "",
                Active = true
            }
        ]);

        // Empty
        var service = new NotificationConfigsService(_logger.Object, notificationConfigRepositoryEmpty.Object, _metadataService.Object, _usersService.Object);
        var actual = await service.GetData(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
        Assert.NotNull(actual);
        Assert.Empty(actual.Notifications);

        // Not Empty
        service = new NotificationConfigsService(_logger.Object, notificationConfigRepositoryNonEmpty.Object, _metadataService.Object, _usersService.Object);
        actual = await service.GetData(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
        Assert.NotNull(actual);
        Assert.NotEmpty(actual.Notifications);
    }

    [Fact]
    public async Task GetAllTests()
    {
        // Setup
        var notificationConfigRepositoryEmpty = new Mock<INotificationConfigRepository>();
        notificationConfigRepositoryEmpty.Setup(s => s.GetAll()).ReturnsAsync([]);

        var notificationConfigRepositoryNonEmpty = new Mock<INotificationConfigRepository>();
        notificationConfigRepositoryNonEmpty.Setup(s => s.GetAll()).ReturnsAsync(
        [
            new()
            {
                NotificationId = 1, CategoryCd = "CATCD",
                Title = "TITLE", Text = "TEXT",
                FrequencyCd = "FREQCD", FrequencyInterval = 0,
                NotificationList = "",
                Active = true
            }
        ]);

        // Empty
        var service = new NotificationConfigsService(_logger.Object, notificationConfigRepositoryEmpty.Object, _metadataService.Object, _usersService.Object);
        var actual = await service.GetAll();
        Assert.Empty(actual);

        // Not Empty
        service = new NotificationConfigsService(_logger.Object, notificationConfigRepositoryNonEmpty.Object, _metadataService.Object, _usersService.Object);
        actual = await service.GetAll();
        Assert.NotEmpty(actual);
    }

    [Fact]
    public async Task GetOneTests()
    {
        // Setup
        var notificationConfigRepositoryNull = new Mock<INotificationConfigRepository>();
        notificationConfigRepositoryNull.Setup(s => s.GetOne(It.IsAny<NotificationConfig>())).ReturnsAsync((NotificationConfig)null);

        var notificationConfigRepositoryNotNull = new Mock<INotificationConfigRepository>();
        notificationConfigRepositoryNotNull.Setup(s => s.GetOne(It.IsAny<NotificationConfig>())).ReturnsAsync(new NotificationConfig()
        {
            NotificationId = 1, CategoryCd = "CATCD",
            Title = "TITLE", Text = "TEXT",
            FrequencyCd = "FREQCD", FrequencyInterval = 0,
            NotificationList = "",
            Active = true
        });

        // Null
        var service = new NotificationConfigsService(_logger.Object, notificationConfigRepositoryNull.Object, _metadataService.Object, _usersService.Object);
        var actual = await service.GetOne(It.IsAny<int>());
        Assert.Null(actual);

        // Not Empty
        service = new NotificationConfigsService(_logger.Object, notificationConfigRepositoryNotNull.Object, _metadataService.Object, _usersService.Object);
        actual = await service.GetOne(It.IsAny<int>());
        Assert.NotNull(actual);
        Assert.Equal(1, actual.NotificationId);
        Assert.Equal("TEXT", actual.Text);
    }

    [Fact]
    public async void GetOneForAddTests()
    {
        var service = new NotificationConfigsService(_logger.Object, _notificationConfigRepository.Object, _metadataService.Object, _usersService.Object);
        var actual = await service.GetOneForAdd();
        Assert.NotNull(actual);
        Assert.Equal(0, actual.NotificationId);
        Assert.Empty(actual.Title);
        Assert.Empty(actual.Text);
        Assert.True(actual.Active);
    }

    [Fact]
    public async Task GetOneForEditTests()
    {
        // Setup
        var notificationConfigRepositoryNull = new Mock<INotificationConfigRepository>();
        notificationConfigRepositoryNull.Setup(s => s.GetOne(It.IsAny<NotificationConfig>())).ReturnsAsync((NotificationConfig)null);

        var notificationConfigRepositoryNotNull = new Mock<INotificationConfigRepository>();
        notificationConfigRepositoryNotNull.Setup(s => s.GetOne(It.IsAny<NotificationConfig>())).ReturnsAsync(new NotificationConfig()
        {
            NotificationId = 1, CategoryCd = "CATCD",
            Title = "TITLE", Text = "TEXT",
            FrequencyCd = "FREQCD", FrequencyInterval = 0,
            NotificationList = "",
            Active = true
        });

        // Null
        var service = new NotificationConfigsService(_logger.Object, notificationConfigRepositoryNull.Object, _metadataService.Object, _usersService.Object);
        var actual = await service.GetOneForEdit(It.IsAny<int>());
        Assert.Null(actual);

        // Not Empty
        service = new NotificationConfigsService(_logger.Object, notificationConfigRepositoryNotNull.Object, _metadataService.Object, _usersService.Object);
        actual = await service.GetOneForEdit(It.IsAny<int>());
        Assert.NotNull(actual);
        Assert.Equal(1, actual.NotificationId);
        Assert.Equal("TEXT", actual.Text);
    }

    [Fact]
    public void SanitizeNullTest()
    {
        NotificationConfig notificationConfig = null;
        var service = new NotificationConfigsService(_logger.Object, _notificationConfigRepository.Object, _metadataService.Object, _usersService.Object);
        service.Sanitize(notificationConfig);
        Assert.Null(notificationConfig);
    }

    [Theory]
    [InlineData(null, null, null, null, -1, null, "", "", "", "", 0, null)]
    [InlineData("", null, null, null, -1, null, "", "", "", "", 0, null)]
    [InlineData(" ", null, null, null, -1, null, "", "", "", "", 0, null)]
    [InlineData("CATCD", null, null, null, -1, null, "CATCD", "", "", "", 0, null)]
    [InlineData("CATCD ", null, null, null, -1, null, "CATCD", "", "", "", 0, null)]
    [InlineData(" CATCD", null, null, null, -1, null, "CATCD", "", "", "", 0, null)]
    [InlineData(" CATCD ", null, null, null, -1, null, "CATCD", "", "", "", 0, null)]
    [InlineData("CATCD", "", null, null, -1, null, "CATCD", "", "", "", 0, null)]
    [InlineData("CATCD", " ", null, null, -1, null, "CATCD", "", "", "", 0, null)]
    [InlineData("CATCD", "TITLE", null, null, -1, null, "CATCD", "TITLE", "", "", 0, null)]
    [InlineData("CATCD", "TITLE ", null, null, -1, null, "CATCD", "TITLE", "", "", 0, null)]
    [InlineData("CATCD", " TITLE", null, null, -1, null, "CATCD", "TITLE", "", "", 0, null)]
    [InlineData("CATCD", " TITLE ", null, null, -1, null, "CATCD", "TITLE", "", "", 0, null)]
    [InlineData("CATCD", "TITLE", "", null, -1, null, "CATCD", "TITLE", "", "", 0, null)]
    [InlineData("CATCD", "TITLE", " ", null, -1, null, "CATCD", "TITLE", "", "", 0, null)]
    [InlineData("CATCD", "TITLE", "TEXT", null, -1, null, "CATCD", "TITLE", "TEXT", "", 0, null)]
    [InlineData("CATCD", "TITLE", "TEXT ", null, -1, null, "CATCD", "TITLE", "TEXT", "", 0, null)]
    [InlineData("CATCD", "TITLE", " TEXT", null, -1, null, "CATCD", "TITLE", "TEXT", "", 0, null)]
    [InlineData("CATCD", "TITLE", " TEXT ", null, -1, null, "CATCD", "TITLE", "TEXT", "", 0, null)]
    [InlineData("CATCD", "TITLE", "TEXT", "", -1, null, "CATCD", "TITLE", "TEXT", "", 0, null)]
    [InlineData("CATCD", "TITLE", "TEXT", " ", -1, null, "CATCD", "TITLE", "TEXT", "", 0, null)]
    [InlineData("CATCD", "TITLE", "TEXT", "FREQCD", -1, null, "CATCD", "TITLE", "TEXT", "FREQCD", 0, null)]
    [InlineData("CATCD", "TITLE", "TEXT", "FREQCD ", -1, null, "CATCD", "TITLE", "TEXT", "FREQCD", 0, null)]
    [InlineData("CATCD", "TITLE", "TEXT", " FREQCD", -1, null, "CATCD", "TITLE", "TEXT", "FREQCD", 0, null)]
    [InlineData("CATCD", "TITLE", "TEXT", " FREQCD ", -1, null, "CATCD", "TITLE", "TEXT", "FREQCD", 0, null)]
    [InlineData("CATCD", "TITLE", "TEXT", "FREQCD", 0, null, "CATCD", "TITLE", "TEXT", "FREQCD", 0, null)]
    [InlineData("CATCD", "TITLE", "TEXT", "FREQCD", 1, null, "CATCD", "TITLE", "TEXT", "FREQCD", 1, null)]
    [InlineData("CATCD", "TITLE", "TEXT", "FREQCD", 1, "", "CATCD", "TITLE", "TEXT", "FREQCD", 1, null)]
    [InlineData("CATCD", "TITLE", "TEXT", "FREQCD", 1, " ", "CATCD", "TITLE", "TEXT", "FREQCD", 1, null)]
    [InlineData("CATCD", "TITLE", "TEXT", "FREQCD", 1, "LIST", "CATCD", "TITLE", "TEXT", "FREQCD", 1, "LIST")]
    [InlineData("CATCD", "TITLE", "TEXT", "FREQCD", 1, "LIST ", "CATCD", "TITLE", "TEXT", "FREQCD", 1, "LIST")]
    [InlineData("CATCD", "TITLE", "TEXT", "FREQCD", 1, " LIST", "CATCD", "TITLE", "TEXT", "FREQCD", 1, "LIST")]
    [InlineData("CATCD", "TITLE", "TEXT", "FREQCD", 1, " LIST ", "CATCD", "TITLE", "TEXT", "FREQCD", 1, "LIST")]
    public void SanitizeObjectTests(string categoryCd, string title, string text,
                                            string frequencyCd, int frequencyInterval, string notificationList,
                                        string expectedCategoryCd, string expectedTitle, string expectedText,
                                            string expectedFrequencyCd, int expectedFrequencyInterval, string expectedNotificationList)
    {
        var notificationConfig = new NotificationConfig()
        {
            CategoryCd = categoryCd,
            Title = title,
            Text = text,
            FrequencyCd = frequencyCd,
            FrequencyInterval = frequencyInterval,
            NotificationList = notificationList
        };
        var service = new NotificationConfigsService(_logger.Object, _notificationConfigRepository.Object, _metadataService.Object, _usersService.Object);
        service.Sanitize(notificationConfig);
        Assert.NotNull(notificationConfig);
        Assert.Equal(expectedCategoryCd, notificationConfig.CategoryCd);
        Assert.Equal(expectedTitle, notificationConfig.Title);
        Assert.Equal(expectedText, notificationConfig.Text);
        Assert.Equal(expectedFrequencyCd, notificationConfig.FrequencyCd);
        Assert.Equal(expectedFrequencyInterval, notificationConfig.FrequencyInterval);
        Assert.Equal(expectedNotificationList, notificationConfig.NotificationList);
    }

    [Fact]
    public async void ValidateNullTest()
    {
        var service = new NotificationConfigsService(_logger.Object, _notificationConfigRepository.Object, _metadataService.Object, _usersService.Object);
        var actual = await service.Validate(null, null);
        Assert.Equal("N000", actual);
    }

    [Theory]
    [InlineData(null, null, null, -1, null, null, "N101")]
    [InlineData("", null, null, -1, null, null, "N101")]
    [InlineData(" ", null, null, -1, null, null, "N101")]
    [InlineData("NOCATCD", null, null, 1, null, null, "N101")]
    [InlineData("CATCD", null, null, 1, null, null, "N102")]
    [InlineData("CATCD", "", null, -1, null, null, "N102")]
    [InlineData("CATCD", " ", null, -1, null, null, "N102")]
    [InlineData("CATCD", "TITLE", null, -1, null, null, "N103")]
    [InlineData("CATCD", "TITLE", "", -1, null, null, "N103")]
    [InlineData("CATCD", "TITLE", " ", -1, null, null, "N103")]
    [InlineData("CATCD", "TITLE", "TEXT", -1, null, null, "N105")]
    [InlineData("CATCD", "TITLE", "TEXT", 0, null, null, "N105")]
    [InlineData("CATCD", "TITLE", "TEXT", 0, "", null, "N105")]
    [InlineData("CATCD", "TITLE", "TEXT", 0, " ", null, "N105")]
    [InlineData("CATCD", "TITLE", "TEXT", 0, "FREQCD", null, "")]
    [InlineData("INTERNAL", "TITLE", "TEXT", 0, "FREQCD", null, "N106")]
    [InlineData("INTERNAL", "TITLE", "TEXT", 0, "FREQCD", "", "N106")]
    [InlineData("INTERNAL", "TITLE", "TEXT", 0, "FREQCD", " ", "N106")]
    [InlineData("INTERNAL", "TITLE", "TEXT", 0, "FREQCD", "LIST", "")]
    public async void ValidateBaseTests(string categoryCd, string title, string text,
                                        int frequencyInterval, string frequencyCd, string notificationList,
                                        string expectedCode)
    {
        var notificationToValidate = new NotificationConfig()
        {
            CategoryCd = categoryCd,
            Title = title,
            Text = text,
            FrequencyCd = frequencyCd,
            FrequencyInterval = frequencyInterval,
            NotificationList = notificationList
        };

        var categories = new Dictionary<string, string>
        {
            { "CATCD", "Category" },
            { "INTERNAL", "Internal" }
        };

        var frequencies = new Dictionary<string, string>
        {
            { "FREQCD", "Frequency" },
            { "ON", "On" }
        };

        var metadataService = new Mock<IMetadataService>();
        metadataService.Setup(s => s.GetCategories(It.IsAny<bool>())).ReturnsAsync(categories);
        metadataService.Setup(s => s.GetFrequencyTypes(It.IsAny<bool>())).ReturnsAsync(frequencies);

        var service = new NotificationConfigsService(_logger.Object, _notificationConfigRepository.Object, metadataService.Object, _usersService.Object);
        var actual = await service.Validate(notificationToValidate, null);
        Assert.Equal(expectedCode, actual);
    }

    [Fact]
    public async void ValidateExistenceTests()
    {
        // Setup
        var notificationToValidate = new NotificationConfig()
        { 
            Active = true, NotificationId = 1, CategoryCd = "CATCD", Title = "TITLE", Text = "TEXT", FrequencyCd = "FREQCD", FrequencyInterval = 0, NotificationList = "LIST"
        };

        var categories = new Dictionary<string, string>
        {
            { "CATCD", "Category" },
            { "INTERNAL", "Internal" }
        };

        var frequencies = new Dictionary<string, string>
        {
            { "FREQCD", "Frequency" },
            { "ON", "On" }
        };

        var metadataService = new Mock<IMetadataService>();
        metadataService.Setup(s => s.GetCategories(It.IsAny<bool>())).ReturnsAsync(categories);
        metadataService.Setup(s => s.GetFrequencyTypes(It.IsAny<bool>())).ReturnsAsync(frequencies);

        // Existence Check FAIL for UPDATE Test
        var existingNotification = new NotificationConfig() { NotificationId = 2, CategoryCd = "CATCD", Title = "TITLE" };
        var service = new NotificationConfigsService(_logger.Object, _notificationConfigRepository.Object, metadataService.Object, _usersService.Object);
        var actual = await service.Validate(notificationToValidate, [ existingNotification ]);
        Assert.Equal("N001", actual);

        // Existence Check SUCCESS for UPDATE Test
        existingNotification.NotificationId = 1;
        service = new NotificationConfigsService(_logger.Object, _notificationConfigRepository.Object, metadataService.Object, _usersService.Object);
        actual = await service.Validate(notificationToValidate, [ existingNotification ]);
        Assert.Empty(actual);
    }

    [Fact]
    public async void ValidateDuplicateTests()
    {
        // Setup
        var notificationToValidate = new NotificationConfig()
        { 
            Active = true, NotificationId = 0, CategoryCd = "CATCD", Title = "TITLE", Text = "TEXT", FrequencyCd = "FREQCD", FrequencyInterval = 0, NotificationList = "LIST"
        };

        var categories = new Dictionary<string, string>
        {
            { "CATCD", "Category" },
            { "INTERNAL", "Internal" }
        };

        var frequencies = new Dictionary<string, string>
        {
            { "FREQCD", "Frequency" },
            { "ON", "On" }
        };

        var metadataService = new Mock<IMetadataService>();
        metadataService.Setup(s => s.GetCategories(It.IsAny<bool>())).ReturnsAsync(categories);
        metadataService.Setup(s => s.GetFrequencyTypes(It.IsAny<bool>())).ReturnsAsync(frequencies);

        // Duplicate Check FAIL for ADD Test
        var existingNotification = new NotificationConfig() { NotificationId = 1, CategoryCd = "CATCD", Title = "TITLE", Text = "TEXT", FrequencyCd = "FREQCD", FrequencyInterval = 0, NotificationList = "LIST" };
        var service = new NotificationConfigsService(_logger.Object, _notificationConfigRepository.Object, metadataService.Object, _usersService.Object);
        var actual = await service.Validate(notificationToValidate, [ existingNotification ]);
        Assert.Equal("N002", actual);

        // Duplicate Check SUCCESS for ADD Test
        existingNotification.Title = "ANOTHERTITLE";
        service = new NotificationConfigsService(_logger.Object, _notificationConfigRepository.Object, metadataService.Object, _usersService.Object);
        actual = await service.Validate(notificationToValidate, [ existingNotification ]);
        Assert.Empty(actual);

        // Duplicate Check FAIL for UPDATE Test
        notificationToValidate.NotificationId = 1;
        existingNotification = new NotificationConfig() { NotificationId = 2, CategoryCd = "CATCD", Title = "TITLE", Text = "TEXT", FrequencyCd = "FREQCD", FrequencyInterval = 0, NotificationList = "LIST" };
        service = new NotificationConfigsService(_logger.Object, _notificationConfigRepository.Object, metadataService.Object, _usersService.Object);
        actual = await service.Validate(notificationToValidate, [ notificationToValidate, existingNotification ]);
        Assert.Equal("N002", actual);

        // Duplicate Check SUCCESS for UPDATE Test
        existingNotification = new NotificationConfig() { NotificationId = 2, CategoryCd = "CATCD", Title = "ANOTHERTITLE", Text = "TEXT", FrequencyCd = "FREQCD", FrequencyInterval = 0, NotificationList = "LIST" };
        service = new NotificationConfigsService(_logger.Object, _notificationConfigRepository.Object, metadataService.Object, _usersService.Object);
        actual = await service.Validate(notificationToValidate, [ notificationToValidate, existingNotification ]);
        Assert.Empty(actual);
    }

    [Fact]
    public async Task AddTests()
    {
        // Setup
        var notificationConfigToAdd = new EditableNotificationConfigViewModel()
        {
            Active = true, NotificationId = 0, CategoryCd = "CATCD", Title = "TITLE", Text = "TEXT", FrequencyCd = "ON", FrequencyInterval = 0, NotificationList = "LIST"
        };

        var categories = new Dictionary<string, string>
        {
            { "CATCD", "Category" },
            { "INTERNAL", "Internal" }
        };

        var frequencies = new Dictionary<string, string>
        {
            { "FREQCD", "Frequency" },
            { "ON", "On" }
        };

        var metadataService = new Mock<IMetadataService>();
        metadataService.Setup(s => s.GetCategories(It.IsAny<bool>())).ReturnsAsync(categories);
        metadataService.Setup(s => s.GetFrequencyTypes(It.IsAny<bool>())).ReturnsAsync(frequencies);

        var notificationConfigRepositoryException = new Mock<INotificationConfigRepository>();
        notificationConfigRepositoryException.Setup(s => s.GetAll()).ThrowsAsync(new Exception() {});

        var notificationConfigRepositoryFailure = new Mock<INotificationConfigRepository>();
        notificationConfigRepositoryFailure.Setup(s => s.GetAll()).ReturnsAsync([]);
        notificationConfigRepositoryFailure.Setup(s => s.Add(It.IsAny<string>(), It.IsAny<NotificationConfig>())).ReturnsAsync(false);

        var notificationConfigRepositorySuccess = new Mock<INotificationConfigRepository>();
        notificationConfigRepositorySuccess.Setup(s => s.GetAll()).ReturnsAsync([]);
        notificationConfigRepositorySuccess.Setup(s => s.Add(It.IsAny<string>(), It.IsAny<NotificationConfig>())).ReturnsAsync(true);

        // Null Test
        var service = new NotificationConfigsService(_logger.Object, _notificationConfigRepository.Object, _metadataService.Object, _usersService.Object);
        var actual = await service.Add(It.IsAny<string>(), It.IsAny<string>(), null);
        Assert.Equal("N000", actual);

        // Validation FAIL Tests
        actual = await service.Add(It.IsAny<string>(), It.IsAny<string>(), new EditableNotificationConfigViewModel());
        Assert.Equal("N101", actual);
        actual = await service.Add(It.IsAny<string>(), It.IsAny<string>(), new EditableNotificationConfigViewModel() { CategoryCd = "" });
        Assert.Equal("N101", actual);
        actual = await service.Add(It.IsAny<string>(), It.IsAny<string>(), new EditableNotificationConfigViewModel() { CategoryCd = "   " });
        Assert.Equal("N101", actual);

        // Add Exception
        service = new NotificationConfigsService(_logger.Object, notificationConfigRepositoryException.Object, metadataService.Object, _usersService.Object);
        await Assert.ThrowsAnyAsync<Exception>(() => service.Add(It.IsAny<string>(), It.IsAny<string>(), notificationConfigToAdd));

        // Add Failure
        service = new NotificationConfigsService(_logger.Object, notificationConfigRepositoryFailure.Object, metadataService.Object, _usersService.Object);
        actual = await service.Add(It.IsAny<string>(), It.IsAny<string>(), notificationConfigToAdd);
        Assert.Equal("N003", actual);

        // Add Success
        service = new NotificationConfigsService(_logger.Object, notificationConfigRepositorySuccess.Object, metadataService.Object, _usersService.Object);
        actual = await service.Add(It.IsAny<string>(), It.IsAny<string>(), notificationConfigToAdd);
        Assert.Empty(actual);
    }

    [Fact]
    public async Task UpdateTests()
    {
        // Setup
        var notificationConfigToUpdate = new EditableNotificationConfigViewModel()
        {
            Active = true, NotificationId = 1, CategoryCd = "CATCD", Title = "TITLE", Text = "TEXT", FrequencyCd = "ON", FrequencyInterval = 0, NotificationList = "LIST"
        };
        var existingNotificationConfig = new NotificationConfig()
        {
            Active = true, NotificationId = 1, CategoryCd = "CATCD", Title = "TITLE", Text = "TEXT", FrequencyCd = "ON", FrequencyInterval = 0, NotificationList = "LIST"
        };

        var categories = new Dictionary<string, string>
        {
            { "CATCD", "Category" },
            { "INTERNAL", "Internal" }
        };

        var frequencies = new Dictionary<string, string>
        {
            { "FREQCD", "Frequency" },
            { "ON", "On" }
        };

        var metadataService = new Mock<IMetadataService>();
        metadataService.Setup(s => s.GetCategories(It.IsAny<bool>())).ReturnsAsync(categories);
        metadataService.Setup(s => s.GetFrequencyTypes(It.IsAny<bool>())).ReturnsAsync(frequencies);

        var notificationConfigRepositoryException = new Mock<INotificationConfigRepository>();
        notificationConfigRepositoryException.Setup(s => s.GetAll()).ThrowsAsync(new Exception() {});

        var notificationConfigRepositoryFailure = new Mock<INotificationConfigRepository>();
        notificationConfigRepositoryFailure.Setup(s => s.GetAll()).ReturnsAsync([ existingNotificationConfig ]);
        notificationConfigRepositoryFailure.Setup(s => s.Update(It.IsAny<string>(), It.IsAny<NotificationConfig>())).ReturnsAsync(false);

        var notificationConfigRepositorySuccess = new Mock<INotificationConfigRepository>();
        notificationConfigRepositorySuccess.Setup(s => s.GetAll()).ReturnsAsync([ existingNotificationConfig ]);
        notificationConfigRepositorySuccess.Setup(s => s.Update(It.IsAny<string>(), It.IsAny<NotificationConfig>())).ReturnsAsync(true);

        // Null Test
        var service = new NotificationConfigsService(_logger.Object, _notificationConfigRepository.Object, _metadataService.Object, _usersService.Object);
        var actual = await service.Update(It.IsAny<string>(), It.IsAny<string>(), null);
        Assert.Equal("N000", actual);

        // Validation FAIL Tests
        actual = await service.Update(It.IsAny<string>(), It.IsAny<string>(), new EditableNotificationConfigViewModel());
        Assert.Equal("N101", actual);
        actual = await service.Update(It.IsAny<string>(), It.IsAny<string>(), new EditableNotificationConfigViewModel() { CategoryCd = "" });
        Assert.Equal("N101", actual);
        actual = await service.Update(It.IsAny<string>(), It.IsAny<string>(), new EditableNotificationConfigViewModel() { CategoryCd = "   " });
        Assert.Equal("N101", actual);

        // Update Exception
        service = new NotificationConfigsService(_logger.Object, notificationConfigRepositoryException.Object, metadataService.Object, _usersService.Object);
        await Assert.ThrowsAnyAsync<Exception>(() => service.Update(It.IsAny<string>(), It.IsAny<string>(), notificationConfigToUpdate));

        // Update Failure
        service = new NotificationConfigsService(_logger.Object, notificationConfigRepositoryFailure.Object, metadataService.Object, _usersService.Object);
        actual = await service.Update(It.IsAny<string>(), It.IsAny<string>(), notificationConfigToUpdate);
        Assert.Equal("N004", actual);

        // Update Success
        service = new NotificationConfigsService(_logger.Object, notificationConfigRepositorySuccess.Object, metadataService.Object, _usersService.Object);
        actual = await service.Update(It.IsAny<string>(), It.IsAny<string>(), notificationConfigToUpdate);
        Assert.Empty(actual);
    }

    [Fact]
    public async Task DeleteTests()
    {
        // Invalid Input Tests
        var service = new NotificationConfigsService(_logger.Object, _notificationConfigRepository.Object, _metadataService.Object, _usersService.Object);
        var actual = await service.Delete(It.IsAny<string>(), It.IsAny<string>(), -1);
        Assert.Equal("N000", actual);
        actual = await service.Delete(It.IsAny<string>(), It.IsAny<string>(), 0);
        Assert.Equal("N000", actual);

        // Setup
        var notificationConfigRepositoryNull = new Mock<INotificationConfigRepository>();
        notificationConfigRepositoryNull.Setup(s => s.GetOne(It.IsAny<NotificationConfig>())).ReturnsAsync((NotificationConfig)null);

        var notificationConfigRepositoryException = new Mock<INotificationConfigRepository>();
        notificationConfigRepositoryException.Setup(s => s.GetOne(It.IsAny<NotificationConfig>())).ThrowsAsync(new Exception() {});

        var notificationConfigRepositoryFailure = new Mock<INotificationConfigRepository>();
        notificationConfigRepositoryFailure.Setup(s => s.GetOne(It.IsAny<NotificationConfig>())).ReturnsAsync(new NotificationConfig()
        {
            NotificationId = 1, Text = "TEXT", Active = true
        });
        notificationConfigRepositoryFailure.Setup(s => s.Delete(It.IsAny<string>(), It.IsAny<NotificationConfig>())).ReturnsAsync(false);

        var notificationConfigRepositorySuccess = new Mock<INotificationConfigRepository>();
        notificationConfigRepositorySuccess.Setup(s => s.GetOne(It.IsAny<NotificationConfig>())).ReturnsAsync(new NotificationConfig()
        {
            NotificationId = 1, Text = "TEXT", Active = true
        });
        notificationConfigRepositorySuccess.Setup(s => s.Delete(It.IsAny<string>(), It.IsAny<NotificationConfig>())).ReturnsAsync(true);

        // Not Found Test
        service = new NotificationConfigsService(_logger.Object, notificationConfigRepositoryNull.Object, _metadataService.Object, _usersService.Object);
        actual = await service.Delete(It.IsAny<string>(), It.IsAny<string>(), 1);
        Assert.Equal("N001", actual);

        // Delete Exception
        service = new NotificationConfigsService(_logger.Object, notificationConfigRepositoryException.Object, _metadataService.Object, _usersService.Object);
        await Assert.ThrowsAnyAsync<Exception>(() => service.Delete(It.IsAny<string>(), It.IsAny<string>(), 1));

        // Delete Failure
        service = new NotificationConfigsService(_logger.Object, notificationConfigRepositoryFailure.Object, _metadataService.Object, _usersService.Object);
        actual = await service.Delete(It.IsAny<string>(), It.IsAny<string>(), 1);
        Assert.Equal("N005", actual);

        // Delete Success
        service = new NotificationConfigsService(_logger.Object, notificationConfigRepositorySuccess.Object, _metadataService.Object, _usersService.Object);
        actual = await service.Delete(It.IsAny<string>(), It.IsAny<string>(), 1);
        Assert.Empty(actual);
    }
}