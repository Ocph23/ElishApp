using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace ElishAppMobile.Models
{

    public class SqlDataModel: ISqlDataModel
    {
        [PrimaryKey]
        public int Id { get; set; }
        public string Data { get; set; }
    }

    public interface ISqlDataModel
    {
        int Id { get; set; }
        string Data { get; set; }

    }


    public class SqlDataModelStock : ISqlDataModel
    {
        [PrimaryKey]
        public int Id { get; set; }
        public string Data { get; set; }
    }
    public class SqlDataModelCategory: ISqlDataModel
    {
        [PrimaryKey]
        public int Id { get; set; }
        public string Data { get; set; }
    }
    public class SqlDataModelSupplier: ISqlDataModel
    {
        [PrimaryKey]
        public int Id { get; set; }
        public string Data { get; set; }
    }
    public class SqlDataModelCustomer: ISqlDataModel
    {
        [PrimaryKey]
        public int Id { get; set; }
        public string Data { get; set; }
    }


    public class SqlDataModelOrder: ISqlDataModel
    {
        [PrimaryKey]
        public int Id { get; set; }
        public string Data { get; set; }
    }
}
