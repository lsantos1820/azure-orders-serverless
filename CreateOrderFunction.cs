using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OrderProcessor.Models;

namespace OrderProcessor.Functions
{
    public static class CreateOrderFunction
    {
        [FunctionName("CreateOrderFunction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "orders")] HttpRequest req,
            [Queue("orders-queue", Connection = "AzureWebJobsStorage")] ICollector<Order> orderQueue,
            ILogger log)
        {
            log.LogInformation("CreateOrderFunction - Nova requisição recebida.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Order? order = JsonConvert.DeserializeObject<Order>(requestBody);

            if (order == null ||
                string.IsNullOrWhiteSpace(order.CustomerName) ||
                string.IsNullOrWhiteSpace(order.CustomerEmail) ||
                order.TotalAmount <= 0)
            {
                log.LogWarning("CreateOrderFunction - Dados inválidos no pedido.");
                return new BadRequestObjectResult("Pedido inválido. Verifique os campos obrigatórios.");
            }

            order.OrderId = string.IsNullOrWhiteSpace(order.OrderId)
                ? Guid.NewGuid().ToString()
                : order.OrderId;

            order.CreatedAt = DateTime.UtcNow;
            order.Status = "Received";

            orderQueue.Add(order);

            log.LogInformation($"CreateOrderFunction - Pedido {order.OrderId} enviado para a fila.");

            return new CreatedResult($"/api/orders/{order.OrderId}", new
            {
                message = "Pedido recebido com sucesso.",
                orderId = order.OrderId,
                status = order.Status
            });
        }
    }
}
