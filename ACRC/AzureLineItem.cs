using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACRDiff
{
    class AzureLineItem
    {
        // Private variables
        private string _key = string.Empty;
        private List<string> _keyNames = new List<string>();

        // Properties
        public string Key
        {
            get { return _key; }
            set
            {
                _key = value;
                _keyNames = new List<string>();
                foreach(string name in _key.Split('|'))
                {
                    _keyNames.Add(name.Trim());
                }
            }
        }
        public Decimal Amount1 { get; set; }
        public Decimal Amount2 { get; set; }

        #region Generated Properties
        public List<string> Names
        {
            get
            {
                return _keyNames;
            }
        }
        public bool IsAdded
        {
            get
            {
                return (Amount1 < 0 && Amount2 >= 0);
            }
        }
        public bool IsRemoved
        {
            get
            {
                return (Amount1 >= 0 && Amount2 < 0);
            }
        }
        public string Diff
        {
            get
            {
                if (IsAdded)
                {
                    return "Added";
                } else if (IsRemoved)
                {
                    return "Removed";
                } else
                {
                    return "$" + (Amount2 - Amount1).ToString();
                }
            }
        }
        #endregion

        public AzureLineItem()
        {
            Key = "Unknown";
            Amount1 = -1;
            Amount2 = -1;
        }
        public AzureLineItem(string key, decimal amount1, decimal amount2)
        {
            Key = key;
            Amount1 = amount1;
            Amount2 = amount2;
        }

        public string Output()
        {
            StringBuilder sb = new StringBuilder();
            foreach(var n in Names)
            {
                sb.Append(n);
                sb.Append(",");
            }

            if (Amount1 >= 0)
            {
                sb.Append("$" + Amount1.ToString());
            } else { sb.Append(" "); }
            sb.Append(",");

            if (Amount2 >= 0)
            {
                sb.Append("$" + Amount2.ToString());
            }
            else { sb.Append(" "); }
            sb.Append(",");

            sb.Append(Diff);
            sb.Append(System.Environment.NewLine);

            return sb.ToString();
        }
    }
}
