using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace TaskRunner.Model.RunnerTrigger
{

    public class CronHelper
    {

        #region constructors

        public CronHelper()
        {
            Pieces = new ObservableCollection<CronPiece>()
            {
                 new CronPiece(CronPieceType.Seconds),
                 new CronPiece(CronPieceType.Minutes),
                 new CronPiece(CronPieceType.Hours),
                 new CronPiece(CronPieceType.Days),
                 new CronPiece(CronPieceType.Months),
                 new CronPiece(CronPieceType.WeekDays)
            };

            CronExpression = "* * 8 * * ?";
        }


        public CronHelper(string expr)
            :this()
        {
            CronExpression = expr;
        }

        #endregion

        
        public ObservableCollection<CronPiece> Pieces { get; set; }

        public string CronExpression
        {
            get
            {
                string days = Days;
                string weekdays = WeekDays;
                if (days == "*" && weekdays == "*") { days = "*"; weekdays = "?"; }
                else if (days == "*" && weekdays != "*") { days = "?"; }
                else if (days != "*" && weekdays == "*") { weekdays = "?"; }
                else { weekdays = "?"; } //  either days or week days.  if both, days wins

                return $"{Seconds} {Minutes} {Hours} {days} {Months} {weekdays}";
            }
            set
            {
                string[] cronParts = value.Split(' ');

                Seconds = cronParts[0];
                Minutes = cronParts[1];
                Hours = cronParts[2];
                Days = cronParts[3];
                Months = cronParts[4];
                WeekDays = cronParts[5];
            }
        }

        public CronPiece Piece(CronPieceType pieceType)
        {
            return Pieces.First(x => x.PieceType == pieceType);
        }

        public string Seconds
        {
            get
            {

                return Piece(CronPieceType.Seconds).PieceString;
            }
            set
            {
                Piece(CronPieceType.Seconds).PieceString = value;
            }
        }
        public string Minutes
        {
            get
            {
                return Piece(CronPieceType.Minutes).PieceString;
            }
            set
            {
                Piece(CronPieceType.Minutes).PieceString = value;
            }
        }
        public string Hours
        {
            get
            {
                return Piece(CronPieceType.Hours).PieceString;
            }
            set
            {
                Piece(CronPieceType.Hours).PieceString = value;
            }
        }
        public string Days
        {
            get
            {
                return Piece(CronPieceType.Days).PieceString;
            }
            set
            {
                Piece(CronPieceType.Days).PieceString = value;
            }
        }
        public string Months
        {
            get
            {
                return Piece(CronPieceType.Months).PieceString;
            }
            set
            {
                Piece(CronPieceType.Months).PieceString = value;
            }
        }
        public string WeekDays
        {
            get
            {
                return Piece(CronPieceType.WeekDays).PieceString;
            }
            set
            {
                Piece(CronPieceType.WeekDays).PieceString = value;
            }
        }

        public string CronDescriptor
        {
            get
            {
                return CronExpressionDescriptor.ExpressionDescriptor.GetDescription(CronExpression);
            }
        }

    }




}
