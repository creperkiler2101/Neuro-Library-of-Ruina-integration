using LOR_XML;

namespace LoR.NeuroIntegration.Extensions;

public static class AbnormalityPageExtensions
{
    public static AbnormalityCard GetXmlInfo(this EmotionCardXmlInfo card)
    {
        return Singleton<AbnormalityCardDescXmlList>.Instance.GetAbnormalityCard(card.Name);
    }
}
