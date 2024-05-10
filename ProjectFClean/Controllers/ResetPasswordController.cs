

using ProjectFClean.Models;
//using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
//using ProjectFClean.ViewModel;
using System.Data.SqlClient;
using System.Net;
using System.Net.Mail;
using System.Web.Mvc;
using System.Data.Entity;
using System;
using ProjectFClean.ViewModel;
using System.Linq;
namespace ProjectFClean.Controllers
{


    public class ResetPasswordController : Controller
    {
        private readonly ProjectFClean1Entities db;
        public ResetPasswordController()
        {
            db = new ProjectFClean1Entities();
        }
        public ActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public ActionResult ForgotPassword(string email)
        {
            Account user = FindUserByEmail(email);

            //if (user == null)
            //{
            //    // display error message user email not exist
            //    return view();
            //}

            string fromEmail = "tulade170793@fpt.edu.vn";
            string fromPassword = "gxcg pczw nlfk qbdj";

            string optCode = (new Random()).Next(0, 1000000).ToString("D6");

            MailMessage message = new MailMessage();
            message.From = new MailAddress(fromEmail);
            message.Subject = "OTP for Resetting your password";
            message.To.Add(new MailAddress(email));
            message.Body = $"<html><body>Your OTP for resetting your password: {optCode}</body></html>";
            message.IsBodyHtml = true;

            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(fromEmail, fromPassword),
                EnableSsl = true
            };

            smtpClient.Send(message);

            InsertNewOtp(email, optCode);
            Email mail = new Email();
            mail.email = email;
            return View("Edit", mail);
        }

        [HttpPost]
        public ActionResult Edit(Email mail)
        {
            if (!ModelState.IsValid)
            {
                return View(mail);
            }

            if (mail.inputOtp == null)
            {

                return View(mail);
            }
            var hk = db.Accounts.FirstOrDefault(a => a.Email == mail.email);



            if (hk.OTP != mail.inputOtp)
            {
                ViewBag.ErrorMessage = "Wrong OTP, Enter again.";
                return View(mail);
            }


            UpdateUserPassword(mail.email, mail.password);

            return RedirectToAction("Login", "Accounts");


        }
        [HttpPost]
        public ActionResult VerifyOtp(string email, string inputOtp, string password)
        {
            string otp = GetOtpOfEmail(email);

            if (otp == null || otp != inputOtp)
            {
                // Display error message: OTP is empty or not matched
                return View();
            }

            UpdateUserPassword(email, password);

            return View("Index");
        }
        private Account FindUserByEmail(string email)
        {
            Account user = null;
            using (var connection = new SqlConnection("Server=DESKTOP-V3UDUKA\\SQLEXPRESS;Database=ProjectFClean1;Trusted_Connection=true;TrustServerCertificate=true;"))
            {
                var command = new SqlCommand($"SELECT TOP 1 * FROM [dbo].[Account] WHERE Email = @Email;", connection);
                SqlParameter parameter = new SqlParameter("@Email", email);
                command.Parameters.Add(parameter);

                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        user = new Account();
                        user.Name = reader["Name"].ToString() ?? "";
                        user.Email = reader["Email"].ToString() ?? "";
                        user.Password = reader["Password"].ToString() ?? "";
                    }
                }
            }

            return user;
        }
        private void UpdateUserPassword(string email, string password)
        {
            using (var connection = new SqlConnection("Server=DESKTOP-V3UDUKA\\SQLEXPRESS;Database=ProjectFClean1;Trusted_Connection=true;TrustServerCertificate=true;"))
            {
                var command = new SqlCommand("UPDATE [dbo].[Account] SET Password = @Password WHERE Email = @Email;", connection);
                SqlParameter emailParameter = new SqlParameter("@Email", email);
                SqlParameter otpParameter = new SqlParameter("@Password", password);
                command.Parameters.Add(emailParameter);
                command.Parameters.Add(otpParameter);

                connection.Open();

                command.ExecuteNonQuery();
            }
        }
        private void InsertNewOtp(string email, string otp)
        {
            string query;
            if (GetOtpOfEmail(email) == null)
            {
                query = "INSERT INTO [dbo].[Account] (Email, OTP) VALUES (@Email,@OTP);";
            }
            else
            {
                query = "UPDATE [dbo].[Account] SET OTP = @OTP WHERE Email = @Email;";
            }
            using (var connection = new SqlConnection("Server=DESKTOP-V3UDUKA\\SQLEXPRESS;Database=ProjectFClean1;Trusted_Connection=true;TrustServerCertificate=true;"))
            {
                var command = new SqlCommand(query, connection);
                SqlParameter emailParameter = new SqlParameter("@Email", email);
                SqlParameter otpParameter = new SqlParameter("@OTP", otp);
                command.Parameters.Add(emailParameter);
                command.Parameters.Add(otpParameter);

                connection.Open();

                command.ExecuteNonQuery();
            }
        }
        private string GetOtpOfEmail(string email)
        {
            string otp = null;

            using (var connection = new SqlConnection("Server=DESKTOP-V3UDUKA\\SQLEXPRESS;Database=ProjectFClean1;Trusted_Connection=true;TrustServerCertificate=true;"))
            {
                var command = new SqlCommand("SELECT TOP 1 * FROM [dbo].[Account] WHERE Email = @Email;", connection);
                SqlParameter parameter = new SqlParameter("@Email", email);
                command.Parameters.Add(parameter);

                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        otp = reader["OTP"].ToString();
                    }
                }
            }

            return otp;
        }
    }
}