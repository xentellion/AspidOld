using System.Collections.Generic;

namespace Aspid.Modules.Languages
{
    public class English
    {
        public static readonly Dictionary<int, string> texts = new Dictionary<int, string>
        {
            [0] = "You took the revolver in hand",
            //"You checked the cylinder and spun it"
            [1] = "You checked the cylinder and spinned it",
            [2] = "You put the gun to your head...",
            [3] = "The shot rang out and body of ",
            //An altenrative could be " fell to the ground", maybe have multiple death messages?
            [4] = " fell on the floor loudly",
            //I think a gun tries to fire it just makes a click noise, so maybe
            //"Click! Nothing Happened! Pass the gun to someone else. Let thenm test their luck."
            [5] = "Missfire! Pass the gun to someone else. Let him test his luck.",

            [6] = "It's forbidden to give guns to crimes",
            [7] = "Dead men don't play with luck",

            //"Bots are immune to bullets, the Aspids are no exception"
            [8] = "*YOU CANNOT RID OF THE ASPIDS THIS WAY*",
            [9] = "Suicide? Not this way",
            [10] = "Don't offer guns to the dead",
            [11] = "It's forbidden to give guns to crimes",
            //It might sound better as "Was death not enough?", but that's just my preference.
            [12] = "You are shooting at dead corpse. Death was not enough?",

            [13] = "You shot ",
            [14] = "Miss! What a shame ",

            //Might sound better as
            //"X people were dead. Y people have died in total."
            [15] = " person were dead\n\nTotally died ",
            [16] = " times \n",

            [17] = "Suddenly, kingsmoulds burst in the room, take away ",
            [18] = "'s gun, watching him falling on their feet, claws and room furniture, and then leave",

            [19] = "No characters",
            [20] = "\n\n By ",

            [21] = "User doesn't have any character",
            [22] = "**User characters**",

            [23] = "Character **",
            [24] = "** has been added",

            [25] = "*** has been deleted",

            [26] = "** has been updated",

            [27] = "No characters on server",

            [28] = "**Characters**",

            [29] = "Input must be less than 100",
            [30] = "Input must be 2 or bigger",
            [31] = "\n \n You get `",

            [32] = "***BOT COMMANDS***",
            [33] = "Page ",

            [34] = "Use this link to add bot on your server",

            [35] = "**VOTE**",
            [36] = " - if you are for this \n",
            [37] = " - if you are against this",

            [38] = "**ABOUT**",
            [39] = "**Primal Aspid** - [Opensorce](https://github.com/xentellion/Aspid) **Discord** messenger bot on **C#** language on **.NET Core** platform",

            [40] = "**SERVER INFO**",
            [41] = "Server **",
            [42] = "** had been created at ",
            [43] = "Totally on server **",
            [44] = "** users, **",
            [45] = "** roles and **",
            [46] = "** channels\n\n",

            [47] = "Bot has been disabled",
            [48] = "Bot has been enabled",
            [49] = "Bot has been disabled for technical reasons",

            [50] = "minutes",
            [51] = "hours",
            [52] = "days",
            [53] = "weeks",
            [54] = "years",
            [55] = " *has been muted for ",
            [56] = "unknown reason",
            [57] = "ATTENTION",
            [58] = "You have been muted on **",
            [59] = "** server for **",
            [60] = "** for ",

            [61] = "Aspids don't let dead men pet them",
            [62] = "You didn't ask anything",

            [63] = "** arrived at server",
            [64] = "Someone has appeared from the fog...",
            [65] = "** left the server",
            [66] = "Someone's steps disminished in the distance...",
        };

        public static readonly string[] asks =
        {
            "Undoubtedly",
            "Sure",
            "No doubt",
            "Certainly yes",
            "You can be sure about it",
            "I will die, if this is wrong",
            "I think yes",
            "Aspid Community says 'Yes'",
            "Most likely",
            //"Outlook good". Good perspectives is a little strange in context.
            "Good perspectives",
            "Signs say 'yes'",
            "Yes",
            "I am sure",
            "Question is unclear",
            "Ask me later",
            "It's better not to know the answer",
            "You wanna me die answering this question?!",
            "Not right now",
            "Pet me first",
            "I don't want to answer",
            "Don't you dare",
            "I say 'no'",
            "Aspid Community says 'No'",
            //"Outlook bad" or "Not good outlook".
            "Not really good perspectives",
            "Highly doubtful",
            "No",
            "Don't make me angry with this question",
        };

        public static readonly string[] pets =
        {
            "Aspid didn't like your patting and spat at you. You died.",
            "\"Be careful with my wings\"",
            "\"I want some more\"",
            "Aspid wants more petting",
            "Aspid flew away: you touched his wings",
            "Aspid liked petting and wants more",
            "Aspid wants you to rub his back",
            "*Happy hiss*"
        };
    }
}
