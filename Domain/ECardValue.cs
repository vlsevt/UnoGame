using System.ComponentModel;

namespace Domain;

public enum ECardValue
{
    [Description("0")]
    Zero,
    [Description("1")]
    One,
    [Description("2")]
    Two,
    [Description("3")]
    Three,
    [Description("4")]
    Four,
    [Description("5")]
    Five,
    [Description("6")]
    Six,
    [Description("7")]
    Seven,
    [Description("8")]
    Eight,
    [Description("9")]
    Nine,
    [Description("Reverse")]
    Reverse,
    [Description("Skip")]
    Skip,
    [Description("Draw Two")]
    DrawTwo,
    [Description("Draw Four")]
    DrawFour,
    [Description("Change Color")]
    ChangeColor,
}
