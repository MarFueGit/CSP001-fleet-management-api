namespace TestFleetManagementAPI
{
    public class TestTaxiController
    {
        [Fact]
        public async Task Get_ReturnsTaxisPaginated()
        {
            // Arrange
            using var client = new HttpClient();
            var apiUrl = "https://localhost:7289/api/taxis"; // Update with your API URL

            // Act
            var response = await client.GetAsync(apiUrl);

            // Assert
            response.EnsureSuccessStatusCode(); // Ensure success status code
            // Add additional assertions here to verify the response data, pagination headers, etc.
        }
    }
}