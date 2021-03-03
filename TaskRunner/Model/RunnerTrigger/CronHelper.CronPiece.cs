using System;
using System.Collections.Generic;
using System.Text;

namespace TaskRunner.Model.RunnerTrigger
{

    public enum CronPieceType
    {
        Seconds = 0,
        Minutes = 1,
        Hours = 2,
        Days = 3,
        Months = 4,
        WeekDays = 5
    }

    public class CronPiece
    {

        static List<int> MaxCount = new List<int>() { 60, 60, 24, 31, 12, 7 };
        static List<bool> IsWeekday = new List<bool>() { false, false, false, false, false, true };
        static List<string> DefaultPieceValue = new List<string>() { "*", "*", "*", "*", "*", "?" };

        bool _isWeekDay;
        int _maxCount;
        public CronPieceType PieceType { get; private set; }

        public CronPiece(CronPieceType pieceType, string cronPiece)
            :this(pieceType)
        {
            PieceString = cronPiece;

        }

        public CronPiece(CronPieceType pieceType)
        {
            PieceType = pieceType;
            _isWeekDay = IsWeekday[(int)pieceType];
            _maxCount = MaxCount[(int)pieceType];
            PieceString = DefaultPieceValue[(int)pieceType];
        }

        public override string ToString()
        {
            return PieceString;
        }

        public List<int> Items { get; set; } = new List<int>();
        public bool FromLast { get; set; } = false;



        public string PieceString
        {
            get
            {
                string items = "";

                if (Items.Count >= _maxCount) return "*";
                if (Items.Count == 0) return "*";
                if (Items.Count == 1)
                {
                    //TODO handle # and /

                    if (FromLast == true)
                    {
                        items = $"{Items[0]}L";
                    }
                    else
                    {
                        items = $"{Items[0]}";
                    }
                }
                else
                {
                    items = RangeValue.RangeFromNumberList(Items, _isWeekDay);
                }

                if (_isWeekDay)
                {
                    items = items.Replace("0", "SUN");
                    items = items.Replace("1", "MON");
                    items = items.Replace("2", "TUE");
                    items = items.Replace("3", "WED");
                    items = items.Replace("4", "THU");
                    items = items.Replace("5", "FRI");
                    items = items.Replace("6", "SAT");
                }

                return items;
            }
            set
            {
                Items.Clear();
                FromLast = false;

                if (_isWeekDay)
                {
                    value = value.Replace("SUN", "0");
                    value = value.Replace("MON", "1");
                    value = value.Replace("TUE", "2");
                    value = value.Replace("WED", "3");
                    value = value.Replace("THU", "4");
                    value = value.Replace("FRI", "5");
                    value = value.Replace("SAT", "6");
                }

                string[] items = value.Split(',');

                if (items.Length == 0)
                {
                    Items.Clear();
                    return;
                }
                if (items.Length == 1)
                {
                    if (items[0].Length == 0)
                    {
                        Items.Clear();
                        return;
                    }
                    if (items[0] == "*")
                    {
                        Items.Clear();
                        return;
                    }

                    if (items[0][items[0].Length - 1] == 'L')
                    {
                        FromLast = true;
                        items[0] = items[0].Substring(0, items[0].Length - 1);
                    }

                    //TODO handle # and /
                }

                foreach (var item in items)
                {
                    //  check for ranges
                    if (item.Contains('-') == true)
                    {
                        string[] itemSplit = item.Split('-');
                        int low = int.Parse(itemSplit[0]);
                        int high = int.Parse(itemSplit[1]);
                        if (low <= high)
                        {
                            for (int i = low; i <= high; i++)
                            {
                                if (Items.Contains(i) == false) Items.Add(i);
                            }
                        }
                    }
                    else
                    {
                        if (int.TryParse(item, out int itemInt) == true)
                        {
                            if (Items.Contains(itemInt) == false) Items.Add(itemInt);
                        }
                    }
                }

            }
        }

        #region Range Helper



        #endregion



    }


}
