

namespace api.artpixxel.data.Models
{
    public enum ChristmasTemplate
    {
        Setof3Set1 = 0,
        Setof3Set2 = 1,
        Setof3Set3 = 2,
        Setof4Set1 = 3,
        Setof4Set2 = 4,
        Setof6Set1 = 5,
        Setof6Set2 = 6,
        Setof9Set1 = 7,
        Setof9Set2 = 8,
    }



    public static class ChristmasTemplateExtension
    {
        public static string ParsedName(this ChristmasTemplate template)
        {
            string parsedTemplate = string.Empty;
            switch (template)
            {
                case ChristmasTemplate.Setof3Set1:
                    parsedTemplate = "Set of 3 (Set 1)";
                    break;

                case ChristmasTemplate.Setof3Set2:
                    parsedTemplate = "Set of 3 (Set 2)";
                    break;

                case ChristmasTemplate.Setof3Set3:
                    parsedTemplate = "Set of 3 (Set 3)";
                    break;


                case ChristmasTemplate.Setof4Set1:
                    parsedTemplate = "Set of 4 (Set 1)";
                    break;

                case ChristmasTemplate.Setof4Set2:
                    parsedTemplate = "Set of 4 (Set 2)";
                    break;

                case ChristmasTemplate.Setof6Set1:
                    parsedTemplate = "Set of 6 (Set 1)";
                    break;

                case ChristmasTemplate.Setof6Set2:
                    parsedTemplate = "Set of 6 (Set 2)";
                    break;

                case ChristmasTemplate.Setof9Set1:
                    parsedTemplate = "Set of 9 (Set 1)";
                    break;

                case ChristmasTemplate.Setof9Set2:
                    parsedTemplate = "Set of 9 (Set 2)";
                    break;

                default:
                    parsedTemplate = "Set of 9 (Set 2)";
                    break;
            }

            return parsedTemplate;
        }
    }
}
