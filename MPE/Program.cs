using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using MPE.Models;
using MPE.MongoDB.Logic;
using MPE.MongoDB.Repositories;
using NPoco;

namespace MPE
{
    class Program
    {
        private static Initializer _initialization;
        static void Main(string[] args)
        {
            _initialization = new Initializer();
            _initialization.Start();

            AppDomain.CurrentDomain.ProcessExit += OnProcessExit;

            Console.WriteLine("Started...");

            var objs = new List<Member>();
            using (var db = new Database("VenmaDb"))
            {
                objs = db.Fetch<Member>("SELECT CreatedOn, MemberID AS EntityId, Firstname, Lastname FROM Member").ToList();
            }

            var repo = new MongoRepositoryBase<Member>();

            var all = repo.GetAll();
            foreach (var member in all)
            {
                repo.Delete(member);
            }

            foreach (var member in objs)
            {
                repo.Save(member);
            }

            Console.WriteLine("done...");

            Console.ReadLine();
        }

        static void OnProcessExit(object sender, EventArgs e)
        {
            _initialization.Stop();
        }
    }
}
