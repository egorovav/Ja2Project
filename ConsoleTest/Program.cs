using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            string _tileset = @"C:\Users\Alexey\Documents\Программы\JA2Edit\Ja2Hds\Hds";
            string[] _files = Directory.GetFiles(_tileset, "*.*", SearchOption.AllDirectories);

            foreach (string _fileName in _files)
            {
                string _fileExtention = Path.GetExtension(_fileName);
                if (_fileExtention.ToUpperInvariant() != ".JSD")
                {
                    File.Delete(_fileName);
                }
            }
        }
    }
}
