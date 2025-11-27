# 🛒 Azure Orders Serverless – Processamento de Pedidos com Azure Functions, Logic Apps e WebJobs

Este projeto demonstra uma arquitetura **Serverless** usando serviços do **Microsoft Azure** para processar pedidos de forma escalável e desacoplada.  
O objetivo é simular um fluxo real de negócio utilizado em aplicações corporativas e em cenários de integração de sistemas.

---

## 🎯 Objetivos do Projeto

- Receber pedidos via API (HTTP Trigger)
- Processar pedidos de forma assíncrona usando filas (Azure Storage Queue)
- Enviar notificações automáticas integradas com serviços externos (Logic Apps)
- Realizar rotinas de manutenção automatizadas (WebJobs)
- Demonstrar diferenças funcionais entre **Azure Functions**, **Azure Logic Apps** e **Azure WebJobs**
- Entender os **planos de hospedagem** e **mecanismos de escala automática** do Azure Functions

---

## 🧱 Arquitetura da Solução

```mermaid
flowchart LR
    A[Cliente / Front-end / Postman] -->|POST /api/orders| B[Azure Function - HTTP Trigger<br/>CreateOrderFunction]
    B -->|Enfileira Pedido| C[(Storage Queue<br/>orders-queue)]
    C -->|Dispara| D[Azure Function - Queue Trigger<br/>ProcessOrderFunction]
    D -->|Notifica/Chama Webhook| E[Azure Logic Apps]
    D -->|Opcional| F[(Banco de Dados Azure SQL / Cosmos DB)]
    G[Azure WebJob - Rotinas de limpeza] --> F
