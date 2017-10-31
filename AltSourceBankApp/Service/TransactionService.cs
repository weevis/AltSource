using System.Collections.Generic;
using AltSourceBankAppAPI.Models;
using AltSourceBankAppAPI.Repository;

namespace AltSourceBankAppAPI.Service
{
    /* Service methods */
    public interface ITransactionService
    {
        IEnumerable<Transactions> GetAllByID(string userid);
        IEnumerable<Transactions> GetAllByIDPaged(string userid, int count, int page);
        Transactions GetByID(int id);
        Transactions Add(Transactions trans);
        void Update(Transactions trans);
        void Remove(int id);
    }

    /* A service we can bind to the service manager and access transactions easily */
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository transactionRepository;

        public TransactionService(ITransactionRepository transRepo)
        {
            transactionRepository = transRepo;
        }

        /// <summary>
        /// Get all Transactions by user id
        /// </summary>
        /// <returns>Transactions Collection</returns>
        public IEnumerable<Transactions> GetAllByID(string userid)
        {
            return transactionRepository.GetAllByUserID(userid);
        }

        /// <summary>
        /// Get paginated transactions by user id
        /// </summary>
        /// <param name="userid">user id</param>
        /// <param name="count">Number of records per page</param>
        /// <param name="page">Page number</param>
        /// <returns>Collection of Transactions</returns>
        public IEnumerable<Transactions> GetAllByIDPaged(string userid, int count, int page)
        {
            return transactionRepository.GetAllByUserIDPaged(userid, count, page);
        }

        /// <summary>
        /// Get Transaction By Id
        /// </summary>
        /// <param name="id">Transaction Id</param>
        /// <returns>Transactions</returns>
        public Transactions GetByID(int id)
        {
            return transactionRepository.GetByID(id);
        }

        /// <summary>
        /// Create Transaction
        /// </summary>
        /// <param name="trans">Transaction to save</param>
        /// <returns>Transaction with Id</returns>
        public Transactions Add(Transactions trans)
        {
            return transactionRepository.Add(trans);
        }

        /// <summary>
        /// Update Transaction
        /// </summary>
        /// <param name="trans">Transaction to update</param>
        public void Update(Transactions trans)
        {
            transactionRepository.Update(trans);
        }

        /// <summary>
        /// Remove Transaction
        /// </summary>
        /// <param name="id">id of Transaction</param>
        public void Remove(int id)
        {
            transactionRepository.Remove(id);
        }
    }
}
