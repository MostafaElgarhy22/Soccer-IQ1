namespace Soccer_IQ.Models
{
    public class LeagueStanding
    {
        public int Id { get; set; }

        // Foreign Key to Club
        public int ClubId { get; set; }
        public Club Club { get; set; }

        public int Played { get; set; }
        public int Wins { get; set; }
        public int Draws { get; set; }
        public int Losses { get; set; }
        public int GoalsFor { get; set; }
        public int GoalsAgainst { get; set; }

        public int GoalDifference => GoalsFor - GoalsAgainst;

        public int Points => (Wins * 3) + Draws;

        public int Position { get; set; }

        public int Season {  get; set; }
    }
}
