using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Eut.Interface.WebAPI01.Models
{
    public class Todo
    {
        public int ID { get; set; }
        public string Description { get; set; }
        public string Owner { get; set; }
    }
}