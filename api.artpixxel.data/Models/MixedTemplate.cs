

namespace api.artpixxel.data.Models
{
    public enum MixedTemplate
    {
        Setof3 = 0,
        Setof4 = 1,
        Setof6 = 2,
        Setof8 = 3,
        Setof9 = 4,
        Setof3Set1 = 5,
        Setof4Set1 = 6,
        Setof4Set2 = 7,
        Setof5Set1 = 8,
        Setof6Set1 = 9,
        Setof7Set1 = 10,
    }

    public static class MixedTemplateExtension
    {
        public static string ParsedName(this MixedTemplate template)
        {
            string parsedTemplate = string.Empty;
            switch (template)
            {
                case MixedTemplate.Setof3:
                    parsedTemplate = "Set of 3";
                    break;
                case MixedTemplate.Setof4:
                    parsedTemplate = "Set of 4";
                    break;
                case MixedTemplate.Setof6:
                    parsedTemplate = "Set of 6";
                    break;
                case MixedTemplate.Setof8:
                    parsedTemplate = "Set of 8";
                    break;
                case MixedTemplate.Setof9:
                    parsedTemplate = "Set of 9";
                    break;
                case MixedTemplate.Setof3Set1:
                    parsedTemplate = "Set of 3 (Set 1)";
                    break;
                case MixedTemplate.Setof4Set1:
                    parsedTemplate = "Set of 4 (Set 1)";
                    break;
                case MixedTemplate.Setof4Set2:
                    parsedTemplate = "Set of 4 (Set 2)";
                    break;
                    
                case MixedTemplate.Setof5Set1:
                    parsedTemplate = "Set of 5 (Set 1)";
                    break;
                case MixedTemplate.Setof6Set1:
                    parsedTemplate = "Set of 6 (Set 1)";
                    break;
              

                default:
                    parsedTemplate = "Set of 7 (Set 1)";
                    break;
            }

            return parsedTemplate;
        }
    }

    
}
