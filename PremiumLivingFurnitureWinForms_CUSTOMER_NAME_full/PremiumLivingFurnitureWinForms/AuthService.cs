using System;using MySql.Data.MySqlClient;
namespace PremiumLivingFurnitureWinForms;
public static class AuthService{public static bool Login(string u,string p,string role){using var c=Database.GetConnection();c.Open();using var cmd=new MySqlCommand("SELECT u.Password FROM Users u JOIN UserRoles r ON u.RoleId=r.Id WHERE u.Username=@u AND r.RoleName=@r AND u.Status='Active' LIMIT 1",c);cmd.Parameters.AddWithValue("@u",u.Trim());cmd.Parameters.AddWithValue("@r",role.Trim());return System.Convert.ToString(cmd.ExecuteScalar())==p.Trim();}}
