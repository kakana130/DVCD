using FinalProject.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace FinalProject.Pages
{
    [Authorize]
    public class ReadEmailModel : PageModel
    {
        public List<EmailInfo> listEmails = new List<EmailInfo>();
        private readonly SignInManager<FinalProjectUser> _signInManager;
        private readonly ILogger<ReadEmailModel> _logger;

        public ReadEmailModel(SignInManager<FinalProjectUser> signInManager, ILogger<ReadEmailModel> logger)
        {
            _signInManager = signInManager;
            _logger = logger;
        }

        public IActionResult OnGet(int EmailID)
        {
            // เช็คว่า user ล็อกอินหรือยัง
            if (!_signInManager.IsSignedIn(User))
            {
                return RedirectToPage("/Account/Login"); // ถ้ายังไม่ได้ล็อกอินให้ไปที่หน้า Login
            }

            try
            {
                string connectionString = "Server=tcp:ic-connect.database.windows.net,1433;Initial Catalog=IC;Persist Security Info=False;User ID=DVCD;Password=ICconnect1;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sql = "SELECT * FROM emails WHERE EmailID = @EmailID";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@EmailID", EmailID);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string loggedInUser = User.Identity.Name; // ชื่อผู้ใช้ที่ล็อกอิน
                                string emailReceiver = reader.GetString(6); // ค่าผู้รับจากฐานข้อมูล

                                // ตรวจสอบว่าผู้ใช้ล็อกอินตรงกับผู้รับหรือไม่
                                if (loggedInUser == emailReceiver)
                                {
                                    EmailInfo emailInfo = new EmailInfo
                                    {
                                        EmailID = reader.GetInt32(0).ToString(),
                                        EmailSubject = reader.GetString(1),
                                        EmailMessage = reader.GetString(2),
                                        EmailDate = reader.GetDateTime(3).ToString(),
                                        EmailIsRead = reader.GetInt32(4).ToString(),
                                        EmailSender = reader.GetString(5),
                                        EmailReceiver = reader.GetString(6)
                                    };
                                    listEmails.Add(emailInfo);

                                    // อัปเดตสถานะการอ่าน
                                    string updateSql = "UPDATE emails SET EmailIsRead = 1 WHERE EmailID = @EmailID";
                                    using (SqlCommand updateCommand = new SqlCommand(updateSql, connection))
                                    {
                                        updateCommand.Parameters.AddWithValue("@EmailID", EmailID);
                                        updateCommand.ExecuteNonQuery(); // อัปเดตสถานะการอ่าน
                                    }
                                }
                                else
                                {
                                    // หากผู้ใช้ไม่ใช่เจ้าของอีเมล, เปลี่ยนเส้นทางไปที่หน้าหลัก
                                    return RedirectToPage("/Index");
                                }
                            }
                            else
                            {
                                // หากไม่พบอีเมลในฐานข้อมูล, เปลี่ยนเส้นทางไปที่หน้าหลัก
                                return RedirectToPage("/Index");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred: {ex.Message}");
            }

            return Page(); // ส่งค่ากลับไปยังหน้า Razor page
        }
    }


}
