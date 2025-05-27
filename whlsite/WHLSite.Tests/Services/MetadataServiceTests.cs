using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Moq;
using WHLSite.Common.Models;
using WHLSite.Common.Repositories;
using WHLSite.Services;

namespace WHLSite.Tests.Services;

public class MetadataServiceTests
{
    private readonly Mock<ILogger<MetadataService>> _logger = new();
    private readonly Mock<IMetadataRepository> _metadataRepository = new();
    private readonly Mock<IUiHelperService> _uiHelperService = new();

    [Fact]
    public void ConstructorTests()
    {
        // ArgumentNullExceptions
        Assert.Throws<ArgumentNullException>(() => new MetadataService(null, null, null));
        Assert.Throws<ArgumentNullException>(() => new MetadataService(_logger.Object, null, null));
        Assert.Throws<ArgumentNullException>(() => new MetadataService(_logger.Object, _metadataRepository.Object, null));

        // Not Null
        var actual = new MetadataService(_logger.Object, _metadataRepository.Object, _uiHelperService.Object);
        Assert.NotNull(actual);
    }

    [Fact]
    public async void GetAllTests()
    {
        // Setup
        var data = new List<CodeDescription>
        {
            new() { CodeId = 1, Code = "CODE1", Description = "DESC1"  },
            new() { CodeId = 2, Code = "CODE2", Description = "DESC2"  }
        };
        var metadataRepository = new Mock<IMetadataRepository>();
        metadataRepository.Setup(s => s.GetAll()).ReturnsAsync(data);
        var service = new MetadataService(_logger.Object, metadataRepository.Object, _uiHelperService.Object);

        // Basic test
        var actual = await service.GetAll();
        Assert.NotEmpty(actual);
        Assert.Equal(2, actual.Count());

        // Basic test
        actual = await service.GetAll(1);
        Assert.NotEmpty(actual);
        Assert.Single(actual);
    }

    [Fact]
    public async void GetAccountTypesTests()
    {
        // Setup
        var data = new List<CodeDescription>
        {
            new() { CodeId = 124, Code = "CODE1", Description = "DESC1"  }
        };

        var expectedDataForUi = new Dictionary<string, string>()
        {
            { "", "Select Account Type" },
            { "CODE1", "DESC1" }
        };

        var expectedDataForNonUi = new Dictionary<string, string>()
        {
            { "CODE1", "DESC1" }
        };

        var metadataRepository = new Mock<IMetadataRepository>();
        metadataRepository.Setup(s => s.GetAll()).ReturnsAsync(data);

        var uiHelperService = new Mock<IUiHelperService>();
        uiHelperService.Setup(s => s.ToDropdownList(It.IsAny<IEnumerable<CodeDescription>>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(expectedDataForUi);
        uiHelperService.Setup(s => s.ToDictionary(It.IsAny<IEnumerable<CodeDescription>>()))
            .Returns(expectedDataForNonUi);

        var service = new MetadataService(_logger.Object, metadataRepository.Object, uiHelperService.Object);

        // Basic test
        var actual = await service.GetAccountTypes(false);
        Assert.Single(actual);
        var first = actual.First();
        Assert.Equal("CODE1", first.Key);
        Assert.Equal("DESC1", first.Value);

        // UI test
        actual = await service.GetAccountTypes(true);
        Assert.NotEmpty(actual);
        first = actual.First();
        Assert.Equal("", first.Key);
        Assert.Equal("Select Account Type", first.Value);
        var second = actual.ToArray()[1];
        Assert.Equal("CODE1", second.Key);
        Assert.Equal("DESC1", second.Value);
    }

    [Fact]
    public async void GetEthnicityTypesTests()
    {
        // Setup
        var data = new List<CodeDescription>
        {
            new() { CodeId = 112, Code = "CODE1", Description = "DESC1"  }
        };

        var expectedDataForUi = new Dictionary<string, string>()
        {
            { "", "Select Ethnicity" },
            { "CODE1", "DESC1" }
        };

        var expectedDataForNonUi = new Dictionary<string, string>()
        {
            { "CODE1", "DESC1" }
        };

        var metadataRepository = new Mock<IMetadataRepository>();
        metadataRepository.Setup(s => s.GetAll()).ReturnsAsync(data);

        var uiHelperService = new Mock<IUiHelperService>();
        uiHelperService.Setup(s => s.ToDropdownList(It.IsAny<IEnumerable<CodeDescription>>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(expectedDataForUi);
        uiHelperService.Setup(s => s.ToDictionary(It.IsAny<IEnumerable<CodeDescription>>()))
            .Returns(expectedDataForNonUi);

        var service = new MetadataService(_logger.Object, metadataRepository.Object, uiHelperService.Object);

        // Basic test
        var actual = await service.GetEthnicityTypes(false);
        Assert.Single(actual);
        var first = actual.First();
        Assert.Equal("CODE1", first.Key);
        Assert.Equal("DESC1", first.Value);

        // UI test
        actual = await service.GetEthnicityTypes(true);
        Assert.NotEmpty(actual);
        first = actual.First();
        Assert.Equal("", first.Key);
        Assert.Equal("Select Ethnicity", first.Value);
        var second = actual.ToArray()[1];
        Assert.Equal("CODE1", second.Key);
        Assert.Equal("DESC1", second.Value);
    }

    [Fact]
    public async void GetGenderTypesTests()
    {
        // Setup
        var data = new List<CodeDescription>
        {
            new() { CodeId = 110, Code = "CODE1", Description = "DESC1"  }
        };

        var expectedDataForUi = new Dictionary<string, string>()
        {
            { "", "Select Gender" },
            { "CODE1", "DESC1" }
        };

        var expectedDataForNonUi = new Dictionary<string, string>()
        {
            { "CODE1", "DESC1" }
        };

        var metadataRepository = new Mock<IMetadataRepository>();
        metadataRepository.Setup(s => s.GetAll()).ReturnsAsync(data);

        var uiHelperService = new Mock<IUiHelperService>();
        uiHelperService.Setup(s => s.ToDropdownList(It.IsAny<IEnumerable<CodeDescription>>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(expectedDataForUi);
        uiHelperService.Setup(s => s.ToDictionary(It.IsAny<IEnumerable<CodeDescription>>()))
            .Returns(expectedDataForNonUi);

        var service = new MetadataService(_logger.Object, metadataRepository.Object, uiHelperService.Object);

        // Basic test
        var actual = await service.GetGenderTypes(false);
        Assert.Single(actual);
        var first = actual.First();
        Assert.Equal("CODE1", first.Key);
        Assert.Equal("DESC1", first.Value);

        // UI test
        actual = await service.GetGenderTypes(true);
        Assert.NotEmpty(actual);
        first = actual.First();
        Assert.Equal("", first.Key);
        Assert.Equal("Select Gender", first.Value);
        var second = actual.ToArray()[1];
        Assert.Equal("CODE1", second.Key);
        Assert.Equal("DESC1", second.Value);
    }

    [Fact]
    public async void GetIdTypesTests()
    {
        // Setup
        var data = new List<CodeDescription>
        {
            new() { CodeId = 125, Code = "CODE1", Description = "DESC1"  }
        };

        var expectedDataForUi = new Dictionary<string, string>()
        {
            { "", "Select Id" },
            { "CODE1", "DESC1" }
        };

        var expectedDataForNonUi = new Dictionary<string, string>()
        {
            { "CODE1", "DESC1" }
        };

        var metadataRepository = new Mock<IMetadataRepository>();
        metadataRepository.Setup(s => s.GetAll()).ReturnsAsync(data);

        var uiHelperService = new Mock<IUiHelperService>();
        uiHelperService.Setup(s => s.ToDropdownList(It.IsAny<IEnumerable<CodeDescription>>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(expectedDataForUi);
        uiHelperService.Setup(s => s.ToDictionary(It.IsAny<IEnumerable<CodeDescription>>()))
            .Returns(expectedDataForNonUi);

        var service = new MetadataService(_logger.Object, metadataRepository.Object, uiHelperService.Object);

        // Basic test
        var actual = await service.GetIdTypes(false);
        Assert.Single(actual);
        var first = actual.First();
        Assert.Equal("CODE1", first.Key);
        Assert.Equal("DESC1", first.Value);

        // UI test
        actual = await service.GetIdTypes(true);
        Assert.NotEmpty(actual);
        first = actual.First();
        Assert.Equal("", first.Key);
        Assert.Equal("Select Id", first.Value);
        var second = actual.ToArray()[1];
        Assert.Equal("CODE1", second.Key);
        Assert.Equal("DESC1", second.Value);
    }

    [Fact]
    public async void GetLanguagesTests()
    {
        // Setup
        var data = new List<CodeDescription>
        {
            new() { CodeId = 113, Code = "CODE1", Description = "DESC1"  }
        };

        var expectedDataForUi = new Dictionary<string, string>()
        {
            { "", "Select Language" },
            { "CODE1", "DESC1" }
        };

        var expectedDataForNonUi = new Dictionary<string, string>()
        {
            { "CODE1", "DESC1" }
        };

        var metadataRepository = new Mock<IMetadataRepository>();
        metadataRepository.Setup(s => s.GetAll()).ReturnsAsync(data);

        var uiHelperService = new Mock<IUiHelperService>();
        uiHelperService.Setup(s => s.ToDropdownList(It.IsAny<IEnumerable<CodeDescription>>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(expectedDataForUi);
        uiHelperService.Setup(s => s.ToDictionary(It.IsAny<IEnumerable<CodeDescription>>()))
            .Returns(expectedDataForNonUi);

        var service = new MetadataService(_logger.Object, metadataRepository.Object, uiHelperService.Object);

        // Basic test
        var actual = await service.GetLanguages(false);
        Assert.Single(actual);
        var first = actual.First();
        Assert.Equal("CODE1", first.Key);
        Assert.Equal("DESC1", first.Value);

        // UI test
        actual = await service.GetLanguages(true);
        Assert.NotEmpty(actual);
        first = actual.First();
        Assert.Equal("", first.Key);
        Assert.Equal("Select Language", first.Value);
        var second = actual.ToArray()[1];
        Assert.Equal("CODE1", second.Key);
        Assert.Equal("DESC1", second.Value);
    }

    [Fact]
    public async void GetLeadTypesTests()
    {
        // Setup
        var data = new List<CodeDescription>
        {
            new() { CodeId = 120, Code = "CODE1", Description = "DESC1"  }
        };

        var expectedDataForUi = new Dictionary<string, string>()
        {
            { "", "Select Lead" },
            { "CODE1", "DESC1" }
        };

        var expectedDataForNonUi = new Dictionary<string, string>()
        {
            { "CODE1", "DESC1" }
        };

        var metadataRepository = new Mock<IMetadataRepository>();
        metadataRepository.Setup(s => s.GetAll()).ReturnsAsync(data);

        var uiHelperService = new Mock<IUiHelperService>();
        uiHelperService.Setup(s => s.ToDropdownList(It.IsAny<IEnumerable<CodeDescription>>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(expectedDataForUi);
        uiHelperService.Setup(s => s.ToDictionary(It.IsAny<IEnumerable<CodeDescription>>()))
            .Returns(expectedDataForNonUi);

        var service = new MetadataService(_logger.Object, metadataRepository.Object, uiHelperService.Object);

        // Basic test
        var actual = await service.GetLeadTypes(false);
        Assert.Single(actual);
        var first = actual.First();
        Assert.Equal("CODE1", first.Key);
        Assert.Equal("DESC1", first.Value);

        // UI test
        actual = await service.GetLeadTypes(true);
        Assert.NotEmpty(actual);
        first = actual.First();
        Assert.Equal("", first.Key);
        Assert.Equal("Select Lead", first.Value);
        var second = actual.ToArray()[1];
        Assert.Equal("CODE1", second.Key);
        Assert.Equal("DESC1", second.Value);
    }

    [Fact]
    public async void GetListingTypesTests()
    {
        // Setup
        var data = new List<CodeDescription>
        {
            new() { CodeId = 107, Code = "CODE1", Description = "DESC1"  }
        };

        var expectedDataForUi = new Dictionary<string, string>()
        {
            { "", "Select Listing" },
            { "CODE1", "DESC1" }
        };

        var expectedDataForNonUi = new Dictionary<string, string>()
        {
            { "CODE1", "DESC1" }
        };

        var metadataRepository = new Mock<IMetadataRepository>();
        metadataRepository.Setup(s => s.GetAll()).ReturnsAsync(data);

        var uiHelperService = new Mock<IUiHelperService>();
        uiHelperService.Setup(s => s.ToDropdownList(It.IsAny<IEnumerable<CodeDescription>>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(expectedDataForUi);
        uiHelperService.Setup(s => s.ToDictionary(It.IsAny<IEnumerable<CodeDescription>>()))
            .Returns(expectedDataForNonUi);

        var service = new MetadataService(_logger.Object, metadataRepository.Object, uiHelperService.Object);

        // Basic test
        var actual = await service.GetListingTypes(false);
        Assert.Single(actual);
        var first = actual.First();
        Assert.Equal("CODE1", first.Key);
        Assert.Equal("DESC1", first.Value);

        // UI test
        actual = await service.GetListingTypes(true);
        Assert.NotEmpty(actual);
        first = actual.First();
        Assert.Equal("", first.Key);
        Assert.Equal("Select Listing", first.Value);
        var second = actual.ToArray()[1];
        Assert.Equal("CODE1", second.Key);
        Assert.Equal("DESC1", second.Value);
    }

    [Fact]
    public async void GetPhoneNumberTypesTests()
    {
        // Setup
        var data = new List<CodeDescription>
        {
            new() { CodeId = 114, Code = "CODE1", Description = "DESC1"  }
        };

        var expectedDataForUi = new Dictionary<string, string>()
        {
            { "", "Select PhoneNumber" },
            { "CODE1", "DESC1" }
        };

        var expectedDataForNonUi = new Dictionary<string, string>()
        {
            { "CODE1", "DESC1" }
        };

        var metadataRepository = new Mock<IMetadataRepository>();
        metadataRepository.Setup(s => s.GetAll()).ReturnsAsync(data);

        var uiHelperService = new Mock<IUiHelperService>();
        uiHelperService.Setup(s => s.ToDropdownList(It.IsAny<IEnumerable<CodeDescription>>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(expectedDataForUi);
        uiHelperService.Setup(s => s.ToDictionary(It.IsAny<IEnumerable<CodeDescription>>()))
            .Returns(expectedDataForNonUi);

        var service = new MetadataService(_logger.Object, metadataRepository.Object, uiHelperService.Object);

        // Basic test
        var actual = await service.GetPhoneNumberTypes(false);
        Assert.Single(actual);
        var first = actual.First();
        Assert.Equal("CODE1", first.Key);
        Assert.Equal("DESC1", first.Value);

        // UI test
        actual = await service.GetPhoneNumberTypes(true);
        Assert.NotEmpty(actual);
        first = actual.First();
        Assert.Equal("", first.Key);
        Assert.Equal("Select PhoneNumber", first.Value);
        var second = actual.ToArray()[1];
        Assert.Equal("CODE1", second.Key);
        Assert.Equal("DESC1", second.Value);
    }

    [Fact]
    public async void GetRaceTypesTests()
    {
        // Setup
        var data = new List<CodeDescription>
        {
            new() { CodeId = 111, Code = "CODE1", Description = "DESC1"  }
        };

        var expectedDataForUi = new Dictionary<string, string>()
        {
            { "", "Select Race" },
            { "CODE1", "DESC1" }
        };

        var expectedDataForNonUi = new Dictionary<string, string>()
        {
            { "CODE1", "DESC1" }
        };

        var metadataRepository = new Mock<IMetadataRepository>();
        metadataRepository.Setup(s => s.GetAll()).ReturnsAsync(data);

        var uiHelperService = new Mock<IUiHelperService>();
        uiHelperService.Setup(s => s.ToDropdownList(It.IsAny<IEnumerable<CodeDescription>>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(expectedDataForUi);
        uiHelperService.Setup(s => s.ToDictionary(It.IsAny<IEnumerable<CodeDescription>>()))
            .Returns(expectedDataForNonUi);

        var service = new MetadataService(_logger.Object, metadataRepository.Object, uiHelperService.Object);

        // Basic test
        var actual = await service.GetRaceTypes(false);
        Assert.Single(actual);
        var first = actual.First();
        Assert.Equal("CODE1", first.Key);
        Assert.Equal("DESC1", first.Value);

        // UI test
        actual = await service.GetRaceTypes(true);
        Assert.NotEmpty(actual);
        first = actual.First();
        Assert.Equal("", first.Key);
        Assert.Equal("Select Race", first.Value);
        var second = actual.ToArray()[1];
        Assert.Equal("CODE1", second.Key);
        Assert.Equal("DESC1", second.Value);
    }

    [Fact]
    public async void GetRelationTypesTests()
    {
        // Setup
        var data = new List<CodeDescription>
        {
            new() { CodeId = 109, Code = "CODE1", Description = "DESC1"  }
        };

        var expectedDataForUi = new Dictionary<string, string>()
        {
            { "", "Select Relation Type" },
            { "CODE1", "DESC1" }
        };

        var expectedDataForNonUi = new Dictionary<string, string>()
        {
            { "CODE1", "DESC1" }
        };

        var metadataRepository = new Mock<IMetadataRepository>();
        metadataRepository.Setup(s => s.GetAll()).ReturnsAsync(data);

        var uiHelperService = new Mock<IUiHelperService>();
        uiHelperService.Setup(s => s.ToDropdownList(It.IsAny<IEnumerable<CodeDescription>>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(expectedDataForUi);
        uiHelperService.Setup(s => s.ToDictionary(It.IsAny<IEnumerable<CodeDescription>>()))
            .Returns(expectedDataForNonUi);

        var service = new MetadataService(_logger.Object, metadataRepository.Object, uiHelperService.Object);

        // Basic test
        var actual = await service.GetRelationTypes(false);
        Assert.Single(actual);
        var first = actual.First();
        Assert.Equal("CODE1", first.Key);
        Assert.Equal("DESC1", first.Value);

        // UI test
        actual = await service.GetRelationTypes(true);
        Assert.NotEmpty(actual);
        first = actual.First();
        Assert.Equal("", first.Key);
        Assert.Equal("Select Relation Type", first.Value);
        var second = actual.ToArray()[1];
        Assert.Equal("CODE1", second.Key);
        Assert.Equal("DESC1", second.Value);
    }

    [Fact]
    public async void GetStatussTests()
    {
        // Setup
        var data = new List<CodeDescription>
        {
            new() { CodeId = 106, Code = "CODE1", Description = "DESC1"  }
        };

        var expectedDataForUi = new Dictionary<string, string>()
        {
            { "", "Select Status" },
            { "CODE1", "DESC1" }
        };

        var expectedDataForNonUi = new Dictionary<string, string>()
        {
            { "CODE1", "DESC1" }
        };

        var metadataRepository = new Mock<IMetadataRepository>();
        metadataRepository.Setup(s => s.GetAll()).ReturnsAsync(data);

        var uiHelperService = new Mock<IUiHelperService>();
        uiHelperService.Setup(s => s.ToDropdownList(It.IsAny<IEnumerable<CodeDescription>>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(expectedDataForUi);
        uiHelperService.Setup(s => s.ToDictionary(It.IsAny<IEnumerable<CodeDescription>>()))
            .Returns(expectedDataForNonUi);

        var service = new MetadataService(_logger.Object, metadataRepository.Object, uiHelperService.Object);

        // Basic test
        var actual = await service.GetStatuses(false);
        Assert.Single(actual);
        var first = actual.First();
        Assert.Equal("CODE1", first.Key);
        Assert.Equal("DESC1", first.Value);

        // UI test
        actual = await service.GetStatuses(true);
        Assert.NotEmpty(actual);
        first = actual.First();
        Assert.Equal("", first.Key);
        Assert.Equal("Select Status", first.Value);
        var second = actual.ToArray()[1];
        Assert.Equal("CODE1", second.Key);
        Assert.Equal("DESC1", second.Value);
    }

    [Fact]
    public async void GetUnitTypesTests()
    {
        // Setup
        var data = new List<CodeDescription>
        {
            new() { CodeId = 106, Code = "CODE1", Description = "DESC1"  }
        };

        var expectedDataForUi = new Dictionary<string, string>()
        {
            { "", "Select Unit" },
            { "CODE1", "DESC1" }
        };

        var expectedDataForNonUi = new Dictionary<string, string>()
        {
            { "CODE1", "DESC1" }
        };

        var metadataRepository = new Mock<IMetadataRepository>();
        metadataRepository.Setup(s => s.GetAll()).ReturnsAsync(data);

        var uiHelperService = new Mock<IUiHelperService>();
        uiHelperService.Setup(s => s.ToDropdownList(It.IsAny<IEnumerable<CodeDescription>>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(expectedDataForUi);
        uiHelperService.Setup(s => s.ToDictionary(It.IsAny<IEnumerable<CodeDescription>>()))
            .Returns(expectedDataForNonUi);

        var service = new MetadataService(_logger.Object, metadataRepository.Object, uiHelperService.Object);

        // Basic test
        var actual = await service.GetUnitTypes(false);
        Assert.Single(actual);
        var first = actual.First();
        Assert.Equal("CODE1", first.Key);
        Assert.Equal("DESC1", first.Value);

        // UI test
        actual = await service.GetUnitTypes(true);
        Assert.NotEmpty(actual);
        first = actual.First();
        Assert.Equal("", first.Key);
        Assert.Equal("Select Unit", first.Value);
        var second = actual.ToArray()[1];
        Assert.Equal("CODE1", second.Key);
        Assert.Equal("DESC1", second.Value);
    }

    [Fact]
    public async void GetViewTypesTests()
    {
        // Setup
        var data = new List<CodeDescription>
        {
            new() { CodeId = 126, Code = "CODE1", Description = "DESC1"  }
        };

        var expectedDataForUi = new Dictionary<string, string>()
        {
            { "", "Select View" },
            { "CODE1", "DESC1" }
        };

        var expectedDataForNonUi = new Dictionary<string, string>()
        {
            { "CODE1", "DESC1" }
        };

        var metadataRepository = new Mock<IMetadataRepository>();
        metadataRepository.Setup(s => s.GetAll()).ReturnsAsync(data);

        var uiHelperService = new Mock<IUiHelperService>();
        uiHelperService.Setup(s => s.ToDropdownList(It.IsAny<IEnumerable<CodeDescription>>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(expectedDataForUi);
        uiHelperService.Setup(s => s.ToDictionary(It.IsAny<IEnumerable<CodeDescription>>()))
            .Returns(expectedDataForNonUi);

        var service = new MetadataService(_logger.Object, metadataRepository.Object, uiHelperService.Object);

        // Basic test
        var actual = await service.GetListingDateTypeSearchOptions(false);
        Assert.Single(actual);
        var first = actual.First();
        Assert.Equal("CODE1", first.Key);
        Assert.Equal("DESC1", first.Value);

        // UI test
        actual = await service.GetListingDateTypeSearchOptions(true);
        Assert.NotEmpty(actual);
        first = actual.First();
        Assert.Equal("", first.Key);
        Assert.Equal("Select View", first.Value);
        var second = actual.ToArray()[1];
        Assert.Equal("CODE1", second.Key);
        Assert.Equal("DESC1", second.Value);
    }

    [Fact]
    public async void GetVoucherTypesTests()
    {
        // Setup
        var data = new List<CodeDescription>
        {
            new() { CodeId = 123, Code = "CODE1", Description = "DESC1"  }
        };

        var expectedDataForUi = new Dictionary<string, string>()
        {
            { "", "Select Voucher" },
            { "CODE1", "DESC1" }
        };

        var expectedDataForNonUi = new Dictionary<string, string>()
        {
            { "CODE1", "DESC1" }
        };

        var metadataRepository = new Mock<IMetadataRepository>();
        metadataRepository.Setup(s => s.GetAll()).ReturnsAsync(data);

        var uiHelperService = new Mock<IUiHelperService>();
        uiHelperService.Setup(s => s.ToDropdownList(It.IsAny<IEnumerable<CodeDescription>>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(expectedDataForUi);
        uiHelperService.Setup(s => s.ToDictionary(It.IsAny<IEnumerable<CodeDescription>>()))
            .Returns(expectedDataForNonUi);

        var service = new MetadataService(_logger.Object, metadataRepository.Object, uiHelperService.Object);

        // Basic test
        var actual = await service.GetVoucherTypes(false);
        Assert.Single(actual);
        var first = actual.First();
        Assert.Equal("CODE1", first.Key);
        Assert.Equal("DESC1", first.Value);

        // UI test
        actual = await service.GetVoucherTypes(true);
        Assert.NotEmpty(actual);
        first = actual.First();
        Assert.Equal("", first.Key);
        Assert.Equal("Select Voucher", first.Value);
        var second = actual.ToArray()[1];
        Assert.Equal("CODE1", second.Key);
        Assert.Equal("DESC1", second.Value);
    }
}