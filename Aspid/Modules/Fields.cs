using Discord;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aspid.Modules
{
    public class Fields
    {
        internal static int curren = 0;

        public static string[] FielderTitleRU =
        {
            "**ДЛЯ ВСЕХ**",
            "**ДЛЯ РОЛЕПЛЕЯ**",
            "**ДЛЯ АДМИНОВ**",
            "**УПРАВЛЕНИЕ ПЕРСОНАЖАМИ**"       
        };

        public static string[] FielderTitleEN =
        {
            "**EVERYONE**",
            "**ROLEPLAY**",
            "**MODERATION**",
            "**CHARACTER MANAGEMENT**"
        };

        public static string[] FielderRU = 
        {
                "**f** - отдать честь \n" +
                "**$vote** *сообщение пользователя* - начать голосование \n" +
                "**$gun** - сыграть в русскую рулетку \n" +
                "**$shoot** *@Пользователь*  -  застрелить другого человека \n" +
                "**$join** - добавить бота на ваш сервер \n" +
                "**$code** - узнать больше о боте\n" +
                "**$pet** - погладить аспида \n" +
                "**$ask** *вопрос* - спросить аспида\n" +
                "**$grub** - посмотреть на гусениц-косплееров \n" +
                "**$batya** - на свой страх и риск \n" +
                "**$server** - посмотреть информацию о сервере" ,

                "**$roll** - бросить двадцатигранный кубик \n " +
                "**$roll** *X* - бросить X-гранный кубик \n" +
                "**$roll** *+Y* - бросить 20-гранный кубик и добавить к выпавшему Y \n" +
                "**$roll** *X+Y* - бросить X-гранный кубик и добавить к выпавшему Y \n" +
                "**$hero** *имя персонажа* - посмотреть краткую анкету персонажа \n" +
                "**$hero** *@Пользователь#1234* - посмотреть персонажей пользователей \n" +
                "**$heroes** - посмотреть список всех персонажей",

                "**$punish** *@Пользователь* - отобрать пистолет \n" +
                "**$mute** *@Пользователь* *Время* *Причина* - выдать мут \n" +
                "**$purge** *X*  - удалить Х сообщений \n" +
                "**$purge** *startswith*  - удалить сообщения начинающиеся с \"(\" \n" +
                "**$purge** *startswith* *символ/слово* *X*  - удалить Х сообщений начинающихся с заданой строки\n" +
                "**$save** - Вернуть всех в живых \n" +
                "**$stop** - остановить бота \n" +
                "**$start** - включить бота",

                "**$add** *Имя_персонажа   @Автор   Описание персонажа*- добавить персонажа\n" +
                "**$delete** *Имя_Персонажа* - удалить анкету \n" +
                "**$update** *Имя_Персонажа    Новое описание* - Изменить персонажа \n" +
                "**$image** *Имя_Персонажа    http://adress.jpeg* - добавить иллюстрацию персонажу"
        };

        public static string[] FielderEN =
        {
                "**f** - press to pay respects \n" +
                "**$vote** *user message* - start vote \n" +
                "**$gun** - play russian roulette \n" +
                "**$shoot** *@User#1234*  -  shoot at this person \n" +
                "**$join** - add bot on your server \n" +
                "**$code** - about the bot\n" +
                "**$pet** - pet aspid \n" +
                "**$ask** *question* - ask for aspid's wise\n" +
                "**$grub** - take a look at grubs cosplayers! \n" +
                "**$batya** - for your own risk \n" +
                "**$server** - get server info" ,

                "**$roll** - drop 20 edged dice \n " +
                "**$roll** *X* - drop X-edged dice \n" +
                "**$roll** *+Y* - drop 20-edged dice and add Y \n" +
                "**$roll** *X+Y* - drop X-edged dice and add Y \n" +
                "**$hero** *name* - see character's form \n" +
                "**$hero** *@User#1234* - see all user's characters \n" +
                "**$heroes** - list of all characters on server",

                "**$punish** *@User#1234* - take off the gun \n" +
                "**$mute** *@User#1234* *Time* *Reason* - mute user \n" +
                "**$purge** *X*  - delete X messages \n" +
                "**$purge** *startswith*  - delete messages starting with \"(\" \n" +
                "**$purge** *startswith* *symbol/word* *X*  - delete X messages starting with X\n" +
                "**$save** - revive everyone \n" +
                "**$stop** - stop bot \n" +
                "**$start** - start bot",

                "**$add** *Char_name   @User#1234   Description*- add character form\n" +
                "**$delete** *Char_name* - delete character \n" +
                "**$update** *Char_name    New description* - change description \n" +
                "**$image** *Char_name    http://adress.jpeg* - add character image"
        };
    }
}
