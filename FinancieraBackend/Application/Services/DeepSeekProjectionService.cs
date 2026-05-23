using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using FinancieraBackend.Domain.DTOs;
using FinancieraBackend.Domain.Interfaces;
using FinancieraBackend.Infrastructure;
using FinancieraBackend.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace FinancieraBackend.Application.Services
{
    public class DeepSeekProjectionService : IProjectionService
    {
        private readonly DBConnection _context;
        private readonly HttpClient _httpClient;

        public DeepSeekProjectionService(DBConnection context, HttpClient httpClient, IConfiguration configuration)
        {
            _context = context;
            _httpClient = httpClient;
        }

        public async Task<ProjectionResponseDTO> GenerateSixMonthProjectionAsync(int groupId)
        {
            var group = await _context.FinancialGroups.FindAsync(groupId);
            if (group == null)
            {
                return new ProjectionResponseDTO
                {
                    GroupId = groupId,
                    MarkdownReport = "El grupo financiero no existe."
                };
            }

            var transactions = await _context.Transactions
                .Where(t => t.GroupId == groupId)
                .OrderBy(t => t.CreatedAt)
                .ToListAsync();

            var income = transactions.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount);
            var expenses = transactions.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount);
            var currentBalance = income - expenses;

            // Check Cache
            if (group.LastProjectionBalance.HasValue && 
                group.LastProjectionBalance.Value == currentBalance && 
                !string.IsNullOrEmpty(group.CachedProjection))
            {
                return new ProjectionResponseDTO
                {
                    GroupId = groupId,
                    MarkdownReport = group.CachedProjection
                };
            }

            var threeMonthsAgo = DateTime.UtcNow.AddMonths(-3);
            var recentTransactions = transactions.Where(t => t.CreatedAt >= threeMonthsAgo).ToList();

            if (!recentTransactions.Any())
            {
                return new ProjectionResponseDTO
                {
                    GroupId = groupId,
                    MarkdownReport = "No hay transacciones en los últimos 3 meses para calcular una tendencia precisa."
                };
            }

            var recentIncome = recentTransactions.Where(t => t.Type == TransactionType.Income).Sum(t => t.Amount);
            var recentExpenses = recentTransactions.Where(t => t.Type == TransactionType.Expense).Sum(t => t.Amount);

            var firstDate = recentTransactions.Min(t => t.CreatedAt);
            var lastDate = recentTransactions.Max(t => t.CreatedAt);
            var monthsDifference = ((lastDate.Year - firstDate.Year) * 12) + lastDate.Month - firstDate.Month;
            if (monthsDifference <= 0) monthsDifference = 1;

            var avgMonthlyIncome = recentIncome / monthsDifference;
            var avgMonthlyExpenses = recentExpenses / monthsDifference;
            var projectedMonthlyNet = avgMonthlyIncome - avgMonthlyExpenses;

            var projectionTableBuilder = new StringBuilder();
            projectionTableBuilder.AppendLine("| Mes Proyectado | Ingresos Esperados | Gastos Esperados | Flujo Neto del Mes | Balance Acumulado |");
            projectionTableBuilder.AppendLine("|---|---|---|---|---|");
            
            decimal runningBalance = currentBalance;
            for(int i = 1; i <= 6; i++) 
            {
                runningBalance += projectedMonthlyNet;
                projectionTableBuilder.AppendLine($"| Mes {i} | {avgMonthlyIncome:C} | {avgMonthlyExpenses:C} | {projectedMonthlyNet:C} | {runningBalance:C} |");
            }
            
            decimal projectedFinalBalance = currentBalance + (projectedMonthlyNet * 6);

            var systemPrompt = @"Eres un Asistente Financiero. Tu tarea es generar un reporte Markdown conciso y preciso.
REGLAS ESTRICTAS:
1. NO inventes datos ni hagas cálculos matemáticos por tu cuenta. Usa los valores proporcionados.
2. NO escribas ni generes ninguna tabla mes a mes (el sistema lo hará).
3. Responde ÚNICAMENTE en formato Markdown con esta estructura simple:

# Proyección Financiera a 6 Meses

## 1. Resumen Actual
[Describe brevemente si el flujo neto es positivo o negativo y qué significa esto para la viabilidad a corto plazo.]

## 2. Proyección Cuantitativa Mensual
[Menciona exactamente el 'Balance Final Proyectado' proporcionado en los datos y explica en un párrafo qué significa alcanzar ese monto.]

## 3. Riesgos y Recomendaciones para Aumentar el Balance
[Lista 2 riesgos principales. Luego, da 2 recomendaciones muy específicas sobre cómo aumentar el balance en los siguientes meses (nuevos ingresos o recortes).]

4. No incluyas saludos, despedidas ni texto de relleno repetitivo.";
            
            var transactionDetails = string.Join("\n", recentTransactions.Select(t => $"- {t.CreatedAt:yyyy-MM-dd}: {t.Type} por {t.Amount:C}"));
            
            var userPrompt = $"DATOS DEL GRUPO (ID {groupId}) BASADO EN LOS ÚLTIMOS 3 MESES:\n" +
                             $"- Balance Total Actual: {currentBalance:C}\n" +
                             $"- Promedio Ingresos/Mes (últimos 3 meses): {avgMonthlyIncome:C}\n" +
                             $"- Promedio Gastos/Mes (últimos 3 meses): {avgMonthlyExpenses:C}\n" +
                             $"- Flujo Neto/Mes: {projectedMonthlyNet:C}\n" +
                             $"- Balance Final Proyectado (al Mes 6): {projectedFinalBalance:C}\n\n" +
                             $"Transacciones Recientes:\n{transactionDetails}\n\n" +
                             $"INSTRUCCIÓN: Genera el reporte siguiendo exactamente las 3 secciones solicitadas. Enfócate en recomendar cómo aumentar el balance. Sé conciso y preciso, NO repitas párrafos.";

            var requestBody = new
            {
                model = "qwen2.5:1.5b",
                messages = new[]
                {
                    new { role = "system", content = systemPrompt },
                    new { role = "user", content = userPrompt }
                },
                options = new { temperature = 0.3 },
                stream = false
            };

            var jsonBody = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("http://192.168.18.2:11434/api/chat", content);

            if (!response.IsSuccessStatusCode)
            {
                var errorMsg = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Error de Ollama API: {response.StatusCode} - {errorMsg}");
            }

            var responseJson = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(responseJson);
            
            var rawMarkdown = doc.RootElement
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            var markdownContent = rawMarkdown;

            if (markdownContent != null)
            {
                var injectionPoint = "## 2. Proyección Cuantitativa Mensual";
                var tableString = "\n\n" + projectionTableBuilder.ToString() + "\n\n";

                if (markdownContent.Contains(injectionPoint))
                {
                    markdownContent = markdownContent.Replace(injectionPoint, injectionPoint + tableString);
                }
                else
                {
                    markdownContent += "\n\n## Proyección Cuantitativa Mensual (Insertada Automáticamente)\n\n" + tableString;
                }

                group.CachedProjection = markdownContent;
                group.LastProjectionBalance = currentBalance;
                _context.FinancialGroups.Update(group);
                await _context.SaveChangesAsync();
            }

            return new ProjectionResponseDTO
            {
                GroupId = groupId,
                MarkdownReport = markdownContent ?? "No se generó contenido válido."
            };
        }
    }
}
