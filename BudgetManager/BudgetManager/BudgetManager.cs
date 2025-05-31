using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BudgetManager
{
    public enum TransactionType
    {
        Доход,
        Расход
    }

    public class Transaction
    {
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public TransactionType Type { get; set; }
        public DateTime Date { get; set; }

        public Transaction(string description, decimal amount, TransactionType type, DateTime date)
        {
            Description = description;
            Amount = amount;
            Type = type;
            Date = date;
        }
    }

    public class BudgetManager
    {
        public List<Transaction> Transactions { get; private set; }

        public decimal TotalBudget
        {
            get { return Transactions.Sum(t => t.Type == TransactionType.Доход ? t.Amount : -t.Amount); }
        }

        public BudgetManager()
        {
            Transactions = new List<Transaction>();
            LoadTransactions();
        }

        public void AddTransaction(Transaction transaction)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException(nameof(transaction));
            }
            Transactions.Add(transaction);
            SaveTransactions();
        }

        public void RemoveTransaction(Transaction transaction)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException(nameof(transaction));
            }
            if (!Transactions.Remove(transaction))
            {
                throw new InvalidOperationException("Transaction not found.");
            }
            SaveTransactions();
        }

        public void UpdateTransaction(Transaction transaction, string newDescription, decimal newAmount, TransactionType newType)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException(nameof(transaction));
            }
            if (!Transactions.Contains(transaction))
            {
                throw new InvalidOperationException("Transaction not found.");
            }
            transaction.Description = newDescription;
            transaction.Amount = newAmount;
            transaction.Type = newType;
            SaveTransactions();
        }

        private void SaveTransactions()
        {
            var lines = Transactions.Select(t => $"{t.Description}|{t.Amount}|{(int)t.Type}|{t.Date:yyyy-MM-dd HH:mm:ss}");
            File.WriteAllLines("transactions.txt", lines);
        }

        private void LoadTransactions()
        {
            if (File.Exists("transactions.txt"))
            {
                var lines = File.ReadAllLines("transactions.txt");
                foreach (var line in lines)
                {
                    var parts = line.Split('|');
                    if (parts.Length == 4)
                    {
                        try
                        {
                            if (decimal.TryParse(parts[1], out var amount) &&
                                Enum.TryParse(parts[2], true, out TransactionType type) &&
                                DateTime.TryParse(parts[3], out var date))
                            {
                                Transactions.Add(new Transaction(parts[0], amount, type, date));
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Ошибка при добавлении транзакции: {ex.Message}");
                        }
                    }
                }
            }
        }
    }
}