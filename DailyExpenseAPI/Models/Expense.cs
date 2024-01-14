using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DailyExpenseAPI.Models
{
    public class Expense
    {
        public int? id { get; set; }
        public string nama_pengeluaran { get; set; }
        public string jumlah_pengeluaran { get; set; }
        public string jumlah_pengeluaran2 { get; set; }
        public string tanggal_pengeluaran { get; set; }
        public string tanggal_pengeluaran2 { get; set; }
        public string? catatan { get; set; }
    }
}
