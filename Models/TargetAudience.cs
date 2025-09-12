using System.Text.Json.Serialization;

namespace Models
{
    public enum TargetAudience
    {
        AllUsers,
        PremiumUsers,
        SpecificGamePlayers,
        InactiveUsers
    }
}
