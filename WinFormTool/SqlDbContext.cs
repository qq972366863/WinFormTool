using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinFormTool
{
    class SqlDbContext : DbContext
    {

        public SqlDbContext() : base("name=ConnectionString")
        {
            Database.SetInitializer<SqlDbContext>(null);

        }
    }
}
