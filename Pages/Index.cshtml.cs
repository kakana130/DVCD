using FinalProject.Areas.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Identity;

namespace FinalProject.Pages
{
    public class IndexModel : PageModel
    {

        public List<EmailInfo> listEmails = new List<EmailInfo>();

        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            try
            {
                String connectionString = "Server=tcp:buem.database.windows.net,1433;Initial Catalog=buem;Persist Security Info=False;User ID=[USERNAME];Password=[PASSWORD];MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string username = "";
                    if (User.Identity.Name == null)
                    {
                        username = "";
                    } else
                    {
                        username = User.Identity.Name;
                    }

                    String sql = "SELECT * FROM emails WHERE emailreceiver='"+username+"'";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                EmailInfo emailInfo = new EmailInfo();
                                emailInfo.EmailID = "" + reader.GetInt32(0);
                                emailInfo.EmailSubject = reader.GetString(1);
                                emailInfo.EmailMessage = reader.GetString(2);
                                emailInfo.EmailDate = reader.GetDateTime(3).ToString();
                                emailInfo.EmailIsRead = "" + reader.GetInt32(4);
                                emailInfo.EmailSender = reader.GetString(5);
                                emailInfo.EmailReceiver = reader.GetString(6);

                                listEmails.Add(emailInfo);
                            }
                        }
                    }
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
    public class EmailInfo
    {
        public String EmailID;
        public String EmailSubject;
        public String EmailMessage;
        public String EmailDate;
        public String EmailIsRead;
        public String EmailSender;
        public String EmailReceiver;
    }

}
