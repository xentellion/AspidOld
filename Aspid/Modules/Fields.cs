﻿using Discord;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aspid.Modules
{
    public class Fields
    {
        internal static int curren = 0;

        public static string[] FielderTitle =
        {
            "**ДЛЯ ВСЕХ**",
            "**ДЛЯ РОЛЕПЛЕЯ**",
            "**ДЛЯ АДМИНОВ**",
            "**УПРАВЛЕНИЕ ПЕРСОНАЖАМИ**"       
        };

        public static string[] Fielder = 
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
                "**$batya** - на свой страх и риск" ,

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
    }
}
