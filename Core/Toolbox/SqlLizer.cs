using System;
using System.IO;


namespace Toolbox
{
    public class SqlLizer
    {
        protected Jsonfier origion { get; private set;}

        public SqlLizer (Jsonfier origion)
        {
            origion = origion;
        }
    }
}