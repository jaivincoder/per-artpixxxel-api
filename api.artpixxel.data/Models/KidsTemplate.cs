
namespace api.artpixxel.data.Models
{
    public enum KidsTemplate
    {
        Setof3Set1 = 0,
        Setof4Set1 = 1,
        Setof4Set2 = 2,
        Setof4Set3 = 3,
        Setof4Set4 = 4,
        Setof4Set5= 5,
        Setof4Set6 = 6,
        Setof4Set7 = 7,
        Setof5Set1 = 8,
        Setof6Set1 = 9,
        Setof6Set2 = 10,
        Setof7Set1 = 11,
        Setof6Set3 = 12,
        Setof6Set4 = 13,
        Setof4Set8 = 14,
        Setof4Set9 = 15,
        Setof7Set2 = 16,
        Setof4Set10 = 17,
        Setof4Set11 = 18,
        Setof4Set12 = 19,
        Setof4Set13 = 20,
        Setof4Set14 = 21,
        Setof4Set15 = 22,
        Setof4Set16 = 23,
        Setof4Set17 = 24,
        Setof4Set18 = 25,
        Setof4Set19 = 26,
        Setof3Set5 = 27,
        Setof3Set6 = 28,
        Setof6Set5 = 29,
        Setof6Set6 = 30,
        Setof6Set7 = 31,
        Setof3Set2 = 32,
        Setof3Set3 = 33,
        Setof3Set4 = 34,
    }

    public static class KidsTemplateExtension
    {
        public static string ParsedName(this KidsTemplate template)
        {
            string parsedTemplate = string.Empty;
            switch (template)
            {
                case KidsTemplate.Setof3Set1:
                    parsedTemplate = "Set of 3 (Set 1)";
                    break;
                case KidsTemplate.Setof3Set2:
                    parsedTemplate = "Set of 3 (Set 2)";
                    break;
                case KidsTemplate.Setof3Set3:
                    parsedTemplate = "Set of 3 (Set 3)";
                    break;
                case KidsTemplate.Setof3Set4:
                    parsedTemplate = "Set of 3 (Set 4)";
                    break;
                case KidsTemplate.Setof4Set1:
                    parsedTemplate = "Set of 4 (Set 1)";
                    break;
                case KidsTemplate.Setof4Set2:
                    parsedTemplate = "Set of 4 (Set 2)";
                    break;
                case KidsTemplate.Setof4Set3:
                    parsedTemplate = "Set of 4 (Set 3)";
                    break;
                case KidsTemplate.Setof4Set4:
                    parsedTemplate = "Set of 4 (Set 4)";
                    break;
                case KidsTemplate.Setof4Set5:
                    parsedTemplate = "Set of 4 (Set 5)";
                    break;
                case KidsTemplate.Setof4Set6:
                    parsedTemplate = "Set of 4 (Set 6)";
                    break;
                case KidsTemplate.Setof4Set7:
                    parsedTemplate = "Set of 4 (Set 7)";
                    break;
                case KidsTemplate.Setof4Set8:
                    parsedTemplate = "Set of 4 (Set 8)";
                    break;
                case KidsTemplate.Setof4Set9:
                    parsedTemplate = "Set of 4 (Set 9)";
                    break;
                case KidsTemplate.Setof4Set10:
                    parsedTemplate = "Set of 4 (Set 10)";
                    break;
                case KidsTemplate.Setof4Set11:
                    parsedTemplate = "Set of 4 (Set 11)";
                    break;
                case KidsTemplate.Setof4Set12:
                    parsedTemplate = "Set of 4 (Set 12)";
                    break;
                case KidsTemplate.Setof4Set13:
                    parsedTemplate = "Set of 4 (Set 13)";
                    break;
                case KidsTemplate.Setof4Set14:
                    parsedTemplate = "Set of 4 (Set 14)";
                    break;
                case KidsTemplate.Setof4Set15:
                    parsedTemplate = "Set of 4 (Set 15)";
                    break;
                case KidsTemplate.Setof4Set16:
                    parsedTemplate = "Set of 4 (Set 16)";
                    break;
                case KidsTemplate.Setof4Set17:
                    parsedTemplate = "Set of 4 (Set 17)";
                    break;
                case KidsTemplate.Setof4Set18:
                    parsedTemplate = "Set of 4 (Set 18)";
                    break;
                case KidsTemplate.Setof4Set19:
                    parsedTemplate = "Set of 4 (Set 19)";
                    break;
                case KidsTemplate.Setof3Set5:
                    parsedTemplate = "Set of 3 (Set 5)";
                    break;
                case KidsTemplate.Setof3Set6:
                    parsedTemplate = "Set of 3 (Set 6)";
                    break;
                case KidsTemplate.Setof5Set1:
                    parsedTemplate = "Set of 5 (Set 1)";
                    break;
                case KidsTemplate.Setof6Set1:
                    parsedTemplate = "Set of 6 (Set 1)";
                    break;
                case KidsTemplate.Setof6Set2:
                    parsedTemplate = "Set of 6 (Set 2)";
                    break;
                case KidsTemplate.Setof6Set3:
                    parsedTemplate = "Set of 6 (Set 3)";
                    break;
                case KidsTemplate.Setof6Set4:
                    parsedTemplate = "Set of 6 (Set 4)";
                    break;
                case KidsTemplate.Setof6Set5:
                    parsedTemplate = "Set of 6 (Set 5)";
                    break;
                case KidsTemplate.Setof6Set6:
                    parsedTemplate = "Set of 6 (Set 6)";
                    break;
                case KidsTemplate.Setof6Set7:
                    parsedTemplate = "Set of 6 (Set 7)";
                    break;
                case KidsTemplate.Setof7Set1:
                    parsedTemplate = "Set of 7 (Set 1)";
                    break;
                case KidsTemplate.Setof7Set2:
                    parsedTemplate = "Set of 7 (Set 2)";
                    break;

                default:
                    parsedTemplate = "Set of 7 (Set 1)";
                    break;
            }

            return parsedTemplate;
        }
    }
}
