using System;

namespace AltSourceBankAppAPI.Models
{
    /* transaction types we track */
    public enum TransactionTypes
    {
        REGISTER,
        DEPOSIT,
        WITHDRAWAL,
        WITHDRAWAL_ATTEMPT,
        BALANCE_CHECK,
        LOGIN,
        LOGIN_ATTEMPT,
        LOGOFF
    };

    /* Transaction Model */
    public class Transactions
    {
        public int Id { get; set; }
        public string UserID { get; set; }
        public DateTime TransactionTime { get; set; }
        public TransactionTypes TransactionType { get; set; }
        public double EndBalance { get; set; }
        public double ChangeAmount { get; set; }
        public double StartBalance { get; set; }
        public string Description { get; set; }
    }
}
