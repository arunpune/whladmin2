using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Moq;
using WHLSite.Common.Models;
using WHLSite.Common.Repositories;
using WHLSite.Services;

namespace WHLSite.Tests.Services;

public class FaqServiceTests
{
    private readonly Mock<ILogger<FaqService>> _logger = new();
    private readonly Mock<IFaqRepository> _faqRepository = new();

    [Fact]
    public void ConstructorTests()
    {
        // ArgumentNullExceptions
        Assert.Throws<ArgumentNullException>(() => new FaqService(null, null));
        Assert.Throws<ArgumentNullException>(() => new FaqService(_logger.Object, null));

        // Not Null
        var actual = new FaqService(_logger.Object, _faqRepository.Object);
        Assert.NotNull(actual);
    }

    [Fact]
    public async void GetDataTests()
    {
        // Exception
        var faqRepository = new Mock<IFaqRepository>();
        faqRepository.Setup(s => s.GetAll()).ThrowsAsync(new Exception() {});
        var service = new FaqService(_logger.Object, faqRepository.Object);
        await Assert.ThrowsAnyAsync<Exception>(() => service.GetData(It.IsAny<string>(), It.IsAny<string>()));

        // Null FAQs
        service = new FaqService(_logger.Object, _faqRepository.Object);
        var actual = await service.GetData(It.IsAny<string>(), It.IsAny<string>());
        Assert.NotNull(actual);
        Assert.Empty(actual.Categories);
        Assert.Empty(actual.Faqs);

        // No FAQs
        var faqs = new List<FaqConfig>();
        faqRepository.Setup(s => s.GetAll()).ReturnsAsync(faqs);
        service = new FaqService(_logger.Object, faqRepository.Object);
        actual = await service.GetData(It.IsAny<string>(), It.IsAny<string>());
        Assert.NotNull(actual);
        Assert.Empty(actual.Categories);
        Assert.Empty(actual.Faqs);

        // With FAQs
        faqs.Add(new()
        {
            CategoryName = "CATEGORY",
            Title = "TITLE",
            Text = "TEXT"
        });
        faqRepository.Setup(s => s.GetAll()).ReturnsAsync(faqs);
        service = new FaqService(_logger.Object, faqRepository.Object);
        actual = await service.GetData(It.IsAny<string>(), It.IsAny<string>());
        Assert.NotNull(actual);
        Assert.NotEmpty(actual.Categories);
        Assert.Contains("CATEGORY", actual.Categories.Values);
        Assert.NotEmpty(actual.Faqs);
        Assert.Contains("TITLE", actual.Faqs.Select(s => s.Title));
    }
}