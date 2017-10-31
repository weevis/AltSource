using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using AltSourceBankAppAPI.Models;
using AltSourceBankAppAPI.Contexts;

namespace AltSourceBankAppAPI.Repository
{
    /* Base repository methods */
    public interface ITransactionRepository
    {
        IEnumerable<Transactions> GetAllByUserID(string userid);
        IEnumerable<Transactions> GetAllByUserIDPaged(string userid, int count, int page);
        Transactions GetByID(int id);
        Transactions Add(Transactions trans);
        void Update(Transactions trans);
        void Remove(int id);
    }

    /* A transaction repository for easy access to the transactions */
    public class TransactionRepository : ITransactionRepository
    {
        private readonly ApiContext context;

        public TransactionRepository(ApiContext apiContext)
        {
            this.context = apiContext;
        }

        /// <summary>
        /// Get all Transactions by user id
        /// </summary>
        /// <returns>Transactions Collection</returns>
        public IEnumerable<Transactions> GetAllByUserID(string userid)
        {
            return context.Transactions.Where(m => m.UserID == userid).ToList();
        }

        /// <summary>
        /// Get Transaction By Id
        /// </summary>
        /// <param name="id">Transaction Id</param>
        /// <returns>Transactions</returns>
        public Transactions GetByID(int id)
        {
            return context.Transactions.FirstOrDefault(m => m.Id == id);
        }


        /// <summary>
        /// Create Transaction
        /// </summary>
        /// <param name="trans">Transaction to save</param>
        /// <returns>Transaction with Id</returns>
        public Transactions Add(Transactions trans)
        {
            context.Transactions.Add(trans);
            context.SaveChanges();

            return trans;
        }

        /// <summary>
        /// Update Transaction
        /// </summary>
        /// <param name="trans">Transaction to update</param>
        public void Update(Transactions trans)
        {
            var transToUpdate = context.Transactions.FirstOrDefault(m => m.Id == trans.Id);

            transToUpdate.EndBalance = trans.EndBalance;
            transToUpdate.Description = trans.Description;
            transToUpdate.ChangeAmount = trans.ChangeAmount;
            transToUpdate.TransactionTime = trans.TransactionTime;
            transToUpdate.TransactionType = trans.TransactionType;
            transToUpdate.UserID = trans.UserID;

            context.Entry(transToUpdate).State = EntityState.Modified;

            context.SaveChanges();
        }

        /// <summary>
        /// Remove Transaction
        /// </summary>
        /// <param name="id">id of Transaction</param>
        public void Remove(int id)
        {
            var transToRemove = context.Transactions.FirstOrDefault(m => m.Id == id);
            context.Transactions.Remove(transToRemove);
            context.SaveChanges();
        }

        /// <summary>
        /// Get paginated transactions by user id
        /// </summary>
        /// <param name="userid">user id</param>
        /// <param name="count">Number of records per page</param>
        /// <param name="page">Page number</param>
        /// <returns>Collection of Transactions</returns>
        public IEnumerable<Transactions> GetAllByUserIDPaged(string userid, int count, int page)
        {
            return context.Transactions.Where(m => m.UserID == userid)
                .Skip((page - 1) * count)
                .Take(count).ToList();
        }
    }
}
