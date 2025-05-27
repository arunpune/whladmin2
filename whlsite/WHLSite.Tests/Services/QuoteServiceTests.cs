using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Moq;
using WHLSite.Common.Models;
using WHLSite.Common.Repositories;
using WHLSite.Services;

namespace WHLSite.Tests.Services;

public class QuoteServiceTests
{
    private readonly Mock<ILogger<QuoteService>> _logger = new();
    private readonly Mock<IQuoteRepository> _quoteRepository = new();

    [Fact]
    public void ConstructorTests()
    {
        // ArgumentNullExceptions
        Assert.Throws<ArgumentNullException>(() => new QuoteService(null, null));
        Assert.Throws<ArgumentNullException>(() => new QuoteService(_logger.Object, null));

        // Not Null
        var actual = new QuoteService(_logger.Object, _quoteRepository.Object);
        Assert.NotNull(actual);
    }

    [Fact]
    public async void GetQuoteForHomePageTests()
    {
        // Exception
        var quoteRepository = new Mock<IQuoteRepository>();
        quoteRepository.Setup(s => s.GetAll()).ThrowsAsync(new Exception() {});
        var service = new QuoteService(_logger.Object, quoteRepository.Object);
        await Assert.ThrowsAnyAsync<Exception>(() => service.GetQuoteForHomePage(It.IsAny<string>(), It.IsAny<string>()));

        // Null Quotes
        service = new QuoteService(_logger.Object, _quoteRepository.Object);
        var actual = await service.GetQuoteForHomePage(It.IsAny<string>(), It.IsAny<string>());
        Assert.Null(actual);

        // No Quotes
        var quotes = new List<QuoteConfig>();
        quoteRepository.Setup(s => s.GetAll()).ReturnsAsync(quotes);
        service = new QuoteService(_logger.Object, quoteRepository.Object);
        actual = await service.GetQuoteForHomePage(It.IsAny<string>(), It.IsAny<string>());
        Assert.Null(actual);

        // With Quotes, No Home Page Indidcator
        quotes.Add(new()
        {
            Text = "TEXT"
        });
        quoteRepository.Setup(s => s.GetAll()).ReturnsAsync(quotes);
        service = new QuoteService(_logger.Object, quoteRepository.Object);
        actual = await service.GetQuoteForHomePage(It.IsAny<string>(), It.IsAny<string>());
        Assert.Null(actual);

        // With Quotes, With Home Page Indidcator
        quotes.Add(new()
        {
            Text = "FORHOMEPAGE",
            DisplayOnHomePageInd = true
        });
        quoteRepository.Setup(s => s.GetAll()).ReturnsAsync(quotes);
        service = new QuoteService(_logger.Object, quoteRepository.Object);
        actual = await service.GetQuoteForHomePage(It.IsAny<string>(), It.IsAny<string>());
        Assert.NotNull(actual);
        Assert.Equal("FORHOMEPAGE", actual.Text);
    }
}