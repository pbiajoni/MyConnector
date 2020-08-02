using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyConnector.Tests
{
    [TestClass()]
    public class ExtensionsTests
    {
        class Pessoa
        {
            public int Idade { get; set; }
            public string Nome { get; set; }
        }


        [TestMethod()]
        public void PutTest()
        {
            QueryBuilder qb = new QueryBuilder();
            Pessoa pessoa = new Pessoa();
            pessoa.Idade = 32;
            pessoa.Nome = "Pablo Biajoni";

            qb.AddParameters(pessoa);
            qb.AddParameters(pessoa, new List<string>() { "Idade" });

        }
    }
}