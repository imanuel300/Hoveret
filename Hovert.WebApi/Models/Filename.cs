using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WEBAPIODATAV3.Models
{
    public class Filename
    {
        [Key]
        public int Key { get; set; }
        public string Name { get; set; }
        public Filename()
        {
            this.Key = 0;
            this.Name = "Default";
        }
        public Filename(int Key, string Name)
        {
            this.Key = Key;
            this.Name = Name;
        }
    }
}