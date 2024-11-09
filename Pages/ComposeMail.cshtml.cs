using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace FinalProject.Pages
{
    public class ComposeEmailModel : PageModel
    {
        private readonly ILogger<ComposeEmailModel> _logger;

        public ComposeEmailModel(ILogger<ComposeEmailModel> logger)
        {
            _logger = logger;
        }

        [BindProperty]
        public string EmailReceiver { get; set; }

        [BindProperty]
        public string EmailSubject { get; set; }

        [BindProperty]
        public string EmailMessage { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            

            try
            {
                string connectionString = "Server=tcp:ic-connect.database.windows.net,1433;Initial Catalog=IC;Persist Security Info=False;User ID=DVCD;Password=ICconnect1;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string sql = "INSERT INTO emails (emailreceiver, emailsubject, emailmessage, emaildate, emailisread, emailsender) " +
                                 "VALUES (@receiver, @subject, @message, @date, @isread, @sender)";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@receiver", EmailReceiver);
                        command.Parameters.AddWithValue("@subject", EmailSubject);
                        command.Parameters.AddWithValue("@message", EmailMessage);
                        command.Parameters.AddWithValue("@date", DateTime.Now);
                        command.Parameters.AddWithValue("@isread", 0);
                        command.Parameters.AddWithValue("@sender", User.Identity.Name);

                        await command.ExecuteNonQueryAsync();
                    }
                }

                return RedirectToPage("/Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email.");
                return Page();
            }
        }
    }
}