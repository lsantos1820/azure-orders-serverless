using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OrderProcessor.Models;

namespace OrderProcessor.Functions
{
    public static class ProcessOrderFunction
    {
        private static readonly HttpClient httpClient = new HttpClient();

        // Substitua com a URL do seu Logic App quando tiver
        private const string LogicAppUrl = "";

        [FunctionName("ProcessOrderFunction")]
        public static async Task Run(
            [QueueTrigger("orders-queue", Connection = "AzureWebJobsStorage")] Order order,
            ILogger log)
        {
            log.LogInformation($"ProcessOrderFunction - Processando pedido {order.OrderId}.");

            // Aqui você poderia salvar no banco de dados
            order.Status = "Processed";

            var payload = new
            {
                orderId = order.OrderId,
                customerName = order.CustomerName,
                customerEmail = order.CustomerEmail,
                totalAmount = order.TotalAmount,
                status = order.Status
            };

            var json = JsonConvert.SerializeObject(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            if (!string.IsNullOrWhiteSpace(LogicAppUrl))
            {
                var response = await httpClient.PostAsync(LogicAppUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    log.LogInformation($"ProcessOrderFunction - Logic App notificado para o pedido {order.OrderId}.");
                }
                else
                {
                    log.LogWarning($"ProcessOrderFunction - Falha ao chamar Logic App. Status: {response.StatusCode}");
                }
            }
            else
            {
                log.LogWarning("ProcessOrderFunction - LogicAppUrl não configurado. Simulando processamento sem notificação.");
            }

            log.LogInformation($"ProcessOrderFunction - Pedido {order.OrderId} processado.");
        }
    }
}
