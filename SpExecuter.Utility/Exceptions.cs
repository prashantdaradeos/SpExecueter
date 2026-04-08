using Sigil;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;



namespace SpExecuter.Utility
{

  
    public class SpExecuterException : Exception
    {
        public string Information { get; set; }

        public SpExecuterException(StringBuilder info, Exception ex) : base(ex.Message, ex)
        {
            Information = info.ToString();
        }
    }
}
