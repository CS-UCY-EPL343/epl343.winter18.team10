﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using InvoiceX.Models;
using MySql.Data.MySqlClient;
namespace InvoiceX.ViewModels
{
    class ProductViwModel
    {
        public List<Product> ProductList { get; set; }
        public ProductViwModel()
        {
            ProductList = new List<Product>();



            MySqlConnection conn;
            string myConnectionString;

            myConnectionString = "server=dione.in.cs.ucy.ac.cy;uid=invoice;" +
                                 "pwd=CCfHC5PWLjsSJi8G;database=invoice";

            try
            {
                conn = new MySqlConnection(myConnectionString);
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM Product", conn);
                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());



                foreach (DataRow dataRow in dt.Rows)
                {
                    var idProductsdb = dataRow.Field<int>("idProduct");
                    var ProductNamedb = dataRow.Field<string>("ProductName");
                    var ProductDescriptiondb = dataRow.Field<string>("Description");
                    var Stockdb = dataRow.Field<int>("Stock");
                    var MinStockdb = dataRow.Field<int>("MinStock");
                    var Costdb = dataRow.Field<float>("Cost");
                    var SellPricedb = dataRow.Field<float>("SellPrice");
                    




                    ProductList.Add(
                        new Product()
                        {
                            idProducts= idProductsdb,
                            ProductName= ProductNamedb,
                            ProductDescription= ProductDescriptiondb,
                            Stock= Stockdb,
                            MinStock= MinStockdb,
                            Cost= Costdb,
                            SellPrice= SellPricedb
                        });

                }


                conn.Close();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show(ex.Message + "\nMallon dn ise sto VPN tou UCY");
            }
        }






    }

}

