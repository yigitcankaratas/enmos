using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading;

class Program
{
    static string connectionString = "Data Source=localhost;Initial Catalog=enmos;Integrated Security=True;";
    static object lockObject = new object();

    static void Main(string[] args)
    {
        Thread thread1 = new Thread(UpdateData);
        Thread thread2 = new Thread(UpdateData);

        thread1.Start("Thread 1");
        thread2.Start("Thread 2");

        thread1.Join();
        thread2.Join();
    }

    static void UpdateData(object threadName)
    {
        try
        {
            while (true)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string updateSql = "UPDATE test SET num = num + 1, date = GETDATE()";
                    using (SqlCommand command = new SqlCommand(updateSql, connection))
                    {
                        lock (lockObject)
                        {
                            int rowsAffected = command.ExecuteNonQuery();
                            if (rowsAffected > 0)
                            {
                                Console.WriteLine($"{threadName} - Updated: value increased by 1, Date: {DateTime.Now}");
                            }
                            else
                            {
                                Console.WriteLine($"{threadName} - Update failed.");
                            }
                        }
                    }
                }
                Thread.Sleep(1000); // 1 saniye bekleme süresi
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"{threadName} - Exception: {ex.Message}");
        }
    }
}
