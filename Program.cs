using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.IO;

namespace SmellyCoin
{
    class Program
    {
        public static BlockChain chain = new BlockChain();
        static Transaction currentTransaction;
        static Block currentBlock;

        static void Main(string[] args)
        {
            chain = new BlockChain();
            currentBlock = new Block();

            string helpMessage =
                "Available commands: \n" +
                "make \t- make a new transaction\n" +
                "validate- validate the current transaction\n" +
                "print \t- print a block of transactions\n" +
                "push \t- push a block of transactions to the blockchain\n" +
                "edit \t- edit a transaction in the current block\n" +
                "t \t- print out the current transaction\n" +
                "help \t- clear screen and print this message out again\n";

            Console.Write(helpMessage);

            while(true)
            {
                string s = Console.ReadLine();

                switch(s)
                {
                    case "make":
                        currentTransaction = MakeTransaction();
                        break;
                    case "validate":
                        ValidateTransaction();
                        break;
                    case "print":
                        PrintBlock();
                        break;
                    case "push":
                        PushBlock();
                        break;
                    case "edit":
                        EditTransaction();
                        break;
                    case "print -1":
                        currentBlock.PrintTransactions();
                        break;
                    case "t":
                        if(currentTransaction == null)
                        {
                            Console.WriteLine("There is currently no transaction being processed.");
                            break;
                        }
                        Console.WriteLine(currentTransaction.ToString());
                        break;
                    case "help":
                        Console.Clear();
                        Console.Write(helpMessage);
                        break;
                    default:
                        Console.WriteLine("I dont understand!");
                        break;
                }


            }


        }
        

        static void EditTransaction()
        {
            currentBlock.PrintTransactions();
            Console.Write("Which transaction would you like to edit?");
            int i = int.Parse(Console.ReadLine());

            currentBlock.transactions[i] = MakeTransaction();

        }

        static Transaction MakeTransaction()
        {
            Console.Write("What is your username?: ");
            string sender = Console.ReadLine();
            Console.Write("Who are you sending smellycoin to?: ");
            string reciever = Console.ReadLine();
            Console.Write("How much smellycoin do you want to send?: ");
            int amount = int.Parse(Console.ReadLine());

            return new Transaction(reciever, sender, amount);
        }


        static void ValidateTransaction()
        {
            Console.WriteLine("Validating transactione: " + currentTransaction.ToString());
            Console.Write("Who are you?: ");
            string u = Console.ReadLine();
            currentTransaction.Validate(u);
            currentBlock.AddTransaction(currentTransaction);
            currentTransaction = null;
        }

        static void PrintBlock()
        {
            int n = 0;
            foreach(Block b in chain.blocks)
            {
                Console.WriteLine(n + " | " + b.ToString());
                n++;
            }
            Console.Write("Which block do you want to examine(-1 for current)?: ");
            int i = int.Parse(Console.ReadLine());
            if(i == -1)
            {
                currentBlock.PrintTransactions();
                return;
            }
            chain.blocks[i].PrintTransactions();
        }

        static void PushBlock()
        {
            string h = currentBlock.HashBlock();
            chain.AddBlock(currentBlock);
            currentBlock = new Block();
            currentBlock.prevHash = h;
        }

    }

    public class BlockChain
    {
        public List<Block> blocks; 

        public BlockChain()
        {
            blocks = new List<Block>();
        }

        public void AddBlock(Block block)
        {
            if (block.hash != "")
            {
                blocks.Add(block);
            }
        }
    }

    public class Block
    {
        public List<Transaction> transactions;
        public string prevHash = "";
        public string hash = "";

        public Block()
        {
            transactions = new List<Transaction>();
        }
        public Block(string prevHash)
        {
            transactions = new List<Transaction>();
            this.prevHash = prevHash;
        }

        public void AddTransaction(Transaction t)
        {
            if (t.isValid())
            {
                transactions.Add(t);
            }
        }

        public string HashBlock()
        {
            if (transactions.Count != 0) // if block not empty
            {
                string s = prevHash;
                foreach (Transaction t in transactions)
                {
                    s += t.ToString(); //create string with all transactions
                }


                var stream = new MemoryStream(Encoding.UTF8.GetBytes(s));
                using (SHA256 mySHA256 = SHA256.Create())
                {
                    hash = BitConverter.ToString(mySHA256.ComputeHash(stream));// hash string to go brrrr
                    return hash;
                }
                
            }
            return "";
        }
        public override string ToString()
        {
            return "Block of size: " + transactions.Count;
        }

        public void PrintTransactions()
        {
            int i = 0;
            foreach (Transaction t in transactions)
            {
                Console.WriteLine(i + "\t" + t.ToString());
                i++;
            }
        }
    }

    public class Transaction
    {
        public string recieverID = "";
        public string senderID = "";
        public int amount = 0;
        public bool validated = false;


        public Transaction(string r, string s, int a)
        {
            recieverID = r;
            senderID = s;
            amount = a;
        }

        public bool Validate(string user) // validates a transaction, both parties have to approve transaction for it to be validated
        {
            if(user == "admin")
            {
                validated = true;
                return validated;
            }
            if (user == recieverID)
            {
                validated = true;
                return validated;
            }
            validated = false;
            Console.WriteLine("Error Validating!");
            return validated;
        }

        public bool isValid()
        {
            if (amount <= 0 || recieverID == "" || senderID == "" || !validated) // if transaction is fully validated, then return true and transaction can be added to block
            {
                return false;
            }
            validated = true;
            return true;
        }

        public override string ToString()
        {
            return "sent " + amount.ToString() + " from " + senderID + " to " + recieverID + ". ";
        }

    }

}
