using System.Net;
using FluentAssertions;
using Helpdesk.IntegrationTests.Support;

namespace Helpdesk.IntegrationTests;

public class SwaggerTests : IntegrationTestBase
{
    [Fact]
    public async Task GetSwaggerJson_Anonymous_DescribesAllApiEndpoints()
    {
        // Arrange
        var client = CreateAnonymousClient();

        // Act
        var response = await client.GetAsync("/swagger/v1/swagger.json");
        var json = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        json.Should().Contain("/api/auth/login")
            .And.Contain("/api/tickets")
            .And.Contain("/api/tickets/{id}/status")
            .And.Contain("/api/users");
    }
}
