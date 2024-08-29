using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class DataModel
    {
        private SqlConnection con;
        private SqlCommand cmd;
        private readonly string _connectionString = ConnectionStrings.ConStr; // ConnectionStrings sınıfındaki bağlantı dizesini kullanın.

        public DataModel()
        {
            con = new SqlConnection(_connectionString);
            cmd = con.CreateCommand();
        }

        #region Personal Metot
        public Employee personalLogin(string username, string password)
        {
            Employee model = new Employee();
            try
            {
                cmd.CommandText = "SELECT Kimlik FROM kullanici_liste WHERE kullanici_adi = @uName AND sifre = @password";
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@uName", username);
                cmd.Parameters.AddWithValue("@password", password);
                con.Open();
                int id = Convert.ToInt32(cmd.ExecuteScalar());
                if (id > 0)
                {
                    model = getPersonal(id);
                }
                return model;

            }
            catch
            {
                return null;
            }
            finally { con.Close(); }
        }

        public Employee getPersonal(int id)
        {
            try
            {
                Employee model = new Employee();
                cmd.CommandText = "SELECT Kimlik, kullanici_adi, sifre, ad_soyad, durum, pcAd, versiyon, KisaAd, Departman \r\nFROM kullanici_liste\r\nWHERE Kimlik = @id";
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@id", id);
                if (con.State != System.Data.ConnectionState.Open)
                {
                    con.Open();
                }
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    model.ID = Convert.ToInt32(reader["Kimlik"]);
                    model.Username = reader.GetString(1);
                    model.Password = reader.GetString(2);
                    model.NameSurname = reader.GetString(3);
                    model.Status = reader.GetByte(4);
                    model.PcName = reader.GetString(5);
                    model.Version = reader.GetString(6);
                    model.ShortName = reader.GetString(7);
                    model.Department = reader.GetString(8);
                }
                return model;
            }
            catch
            {
                return null;
            }
            finally { con.Close(); }
        }
        #endregion

        public List<Products> getBarcodeQuality(string barcode)
        {
            List<Products> pr = new List<Products>();
            try
            {
                cmd.CommandText = "SELECT ID, Quality FROM Products WHERE Barcode = @barcode";
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@barcode", barcode);
                con.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Products model = new Products
                        {
                            ID = Convert.ToInt32(reader["ID"]),
                            QualityID = Convert.ToInt32(reader["Quality"])
                        };
                        pr.Add(model);
                    }
                }
                return pr;
            }
            catch
            {
                return null;
            }
            finally
            {
                con.Close();
            }
        }

        #region Product Metot

        public bool isThereBarcode(string barcode)
        {
            try
            {
                cmd.CommandText = "SELECT COUNT(*) FROM Products WHERE Barcode = @barcode";
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@barcode", barcode);
                con.Open();
                int count = (int)cmd.ExecuteScalar();
                return count > 0;
            }
            catch
            {
                return false;
            }
            finally
            {
                con.Close();
            }
        }

        public List<Products> isThereQuality(string barcode)
        {
            List<Products> pr = new List<Products>();
            try
            {
                cmd.CommandText = "SELECT ID, Quality FROM Products WHERE Barcode = @barcode";
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@barcode", barcode);
                con.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Products model = new Products
                        {
                            ID = Convert.ToInt32(reader["ID"]),
                            QualityID = Convert.ToInt32(reader["Quality"])
                        };
                        pr.Add(model);
                    }
                }
                return pr;
            }
            catch
            {
                return null;
            }
            finally
            {
                con.Close();
            }
        }


        // Update the Products table with the new quality

        public bool UpdateProductQuality(string barcode, byte newQualityID)
        {
            try
            {
                // Bağlantıyı kontrol et ve aç
                if (con.State != System.Data.ConnectionState.Open)
                {
                    con.Open();
                }

                cmd.CommandText = "UPDATE Products SET Quality = @qualityID WHERE Barcode = @barcode";
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@qualityID", newQualityID);
                cmd.Parameters.AddWithValue("@barcode", barcode);

                cmd.ExecuteNonQuery();
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                // Bağlantıyı kapat
                if (con.State == System.Data.ConnectionState.Open)
                {
                    con.Close();
                }
            }
        }


        public bool UpdateProductCodeID(string barcode, int newProductCodeID)
        {
            try
            {
                // Bağlantıyı kontrol et ve aç
                if (con.State != System.Data.ConnectionState.Open)
                {
                    con.Open();
                }

                cmd.CommandText = "UPDATE Products SET ProductCode = @productCodeID WHERE Barcode = @barcode";
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@productCodeID", newProductCodeID);
                cmd.Parameters.AddWithValue("@barcode", barcode);

                cmd.ExecuteNonQuery();
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                // Bağlantıyı kapat
                if (con.State == System.Data.ConnectionState.Open)
                {
                    con.Close();
                }
            }
        }


        #endregion

        #region Code Metot

        public bool isThereCode(string number)
        {
            try
            {
                cmd.CommandText = "SELECT COUNT(*) FROM kod_liste WHERE numara = @number";
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@number", number);
                con.Open();
                int count = (int)cmd.ExecuteScalar();
                return count > 0;
            }
            catch
            {
                return false;
            }
            finally
            {
                con.Close();
            }
        }

        public List<ListQuality> GetResult()
        {
            List<ListQuality> result = new List<ListQuality>();
            try
            {
                cmd.CommandText = "SELECT Kimlik, tanim FROM kalite_liste";
                cmd.Parameters.Clear();
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    ListQuality pc = new ListQuality() { ID = reader.GetByte(0), Definition = reader.GetString(1) };
                    result.Add(pc);
                }
                return result;
            }
            catch
            {
                return null;
            }
            finally
            {
                con.Close();
            }
        }

        public int GetProductCodeID(string number)
        {
            int productCodeID = 0;
            try
            {
                cmd.CommandText = "SELECT Kimlik FROM kod_liste WHERE numara = @number"; // Assuming "Code" is the column name
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@number", number);
                con.Open();
                object result = cmd.ExecuteScalar();
                if (result != null)
                {
                    productCodeID = Convert.ToInt32(result);
                }
            }
            catch
            {
                // Handle exceptions if necessary
            }
            finally
            {
                con.Close();
            }
            return productCodeID;
        }



        #endregion

        #region Hole Drilling Metot

        public List<HoleDrilling> logEntryListHoleDrilling(HoleDrilling filter)
        {
            List<HoleDrilling> hd = new List<HoleDrilling>();
            try
            {
                cmd.CommandText = @"SELECT dd.ID, dd.Barcode, dd.DateTime, qlt.tanim, pc.tanim, qp.ad_soyad
FROM kalite_DelikDelme AS dd
JOIN kalite_liste AS qlt ON qlt.Kimlik = dd.QualityID
JOIN kod_liste AS pc ON pc.Kimlik = dd.ProductCodeID
JOIN kullanici_liste AS qp ON qp.Kimlik = dd.QualityPersonalID
WHERE CONVERT(date, dd.DateTime) = @datetime";

                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@datetime", DateTime.Now.Date);

                con.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        HoleDrilling model = new HoleDrilling
                        {
                            // Bu satırların veri tiplerini kontrol edin ve doğru dönüşümler yapın
                            ID = reader.GetInt32(0),
                            Barcode = reader.GetString(1),
                            DateTime = reader.GetDateTime(2),
                            Quality = reader.GetString(3),
                            ProductCode = reader.GetString(4),
                            QualityPersonal = reader.GetString(5),
                        };
                        hd.Add(model);
                    }
                }
                return hd;
            }
            catch
            {
                return null;
            }
            finally
            {
                con.Close();
            }
        }

        public bool createHoleDrilling(HoleDrilling hd)
        {
            try
            {
                cmd.CommandText = "INSERT INTO kalite_DelikDelme (Barcode, DateTime, QualityID, ProductCodeID, QualityPersonalID) " +
                                  "VALUES (@barcode, FORMAT(@date, 'yyyy-MM-dd HH:mm:ss'), @qid, @pcid, @qpid)";
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@barcode", hd.Barcode);
                cmd.Parameters.AddWithValue("@date", hd.DateTime);
                cmd.Parameters.AddWithValue("@qid", hd.QualityID);
                cmd.Parameters.AddWithValue("@pcid", hd.ProductCodeID);
                cmd.Parameters.AddWithValue("@qpid", hd.QualityPersonalID);
                con.Open();
                cmd.ExecuteNonQuery();

                // Kalite güncelleme
                UpdateProductQuality(hd.Barcode, Convert.ToByte(hd.QualityID));

                // Kod güncelleme
                UpdateProductCodeID(hd.Barcode, hd.ProductCodeID);

                return true;
            }
            catch
            {
                // Handle exceptions
                return false;
            }
            finally
            {
                con.Close();
            }
        }


        #endregion
    }
}
