using LoR.NeuroIntegration.Extensions;

namespace LoR.NeuroIntegration.BattleState;

public class AbnormalityPageModel
{
    public string Name { get; set; }
    public string Target { get; set; }
    public string Ability { get; set; }
    public string Description { get; set; }

    public static AbnormalityPageModel From(EmotionCardXmlInfo page)
    {
        var xmlInfo = page.GetXmlInfo();

        return new AbnormalityPageModel
        {
            Name = xmlInfo.cardName,
            Target = page.TargetType switch
            {
                EmotionTargetType.All => "all_librarians",
                EmotionTargetType.AllIncludingEnemy => "all_librarians_and_enemies",
                EmotionTargetType.SelectOne => "one_librarian",
                _ => "unknown"
            },
            Ability = xmlInfo.abilityDesc,
            Description = xmlInfo.flavorText,
        };
    }
}
