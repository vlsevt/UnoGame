using System.ComponentModel;

namespace Domain;

public enum ECardColor
{
    [Description("🔴")]
    Red,
    [Description("🔵")]
    Blue,
    [Description("🟡")]
    Yellow,
    [Description("🟢")]
    Green,
    [Description("⚫")]
    Wild
}
