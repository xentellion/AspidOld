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
                "**$server** - посмотреть информацию о сервере \n" +
                "**$pick** *Вариант 1 | Вариант 2 | ...*  -  выбрать одно из нескольких" ,

                "**$roll** - бросить двадцатигранный кубик \n " +
                "**$roll** *X* - бросить X-гранный кубик \n" +
                "**$roll** *+Y* - бросить 20-гранный кубик и добавить к выпавшему Y \n" +
                "**$roll** *X+Y* - бросить X-гранный кубик и добавить к выпавшему Y \n" +
                "**$hero** *имя персонажа* - посмотреть краткую анкету персонажа \n" +
                "**$hero** *@Пользователь#1234* - посмотреть персонажей пользователей \n" +
                "**$heroes** - посмотреть список всех персонажей \n" +
                "**$tweet** *имя персонажа* *текст*  - отписать от имени персонажа в жучиный твиттер \n" +
                "**$nick** *имя персонажа* *текст*  - изменить никнейм персонажа в жучином твиттере \n" +
                "**$gallery** *имя персонажа*  -  посмотреть все арты персонажа \n" +
                "**$addImage** *имя персонажа  http://adress.jpeg*  -  Добавить арт своему персонажу \n" +
                "**$deleteImage** *имя персонажа  номер в галерее*  -  Удалить арт своего персонажу \n",

                "**$punish** *@Пользователь* - отобрать пистолет \n" +
                "**$mute** *@Пользователь* *Время* *Причина* - выдать мут \n" +
                "**$purge** *X*  - удалить Х сообщений \n" +
                "**$purge** *startswith*  - удалить сообщения начинающиеся с \"(\" \n" +
                "**$purge** *startswith* *символ/слово* *X*  - удалить Х сообщений начинающихся с заданой строки\n" +
                "**$save** - Вернуть всех в живых \n" +
                "**$stop** - остановить бота \n" +
                "**$start** - включить бота",

                "**$add** *Имя_персонажа   @Автор   Уровни персонажа (0 = 10) Биография | Инвентарь*- добавить персонажа\n" +
                "**$delete** *Имя_Персонажа* - удалить анкету \n" +
                "**$updateLevel** *Имя_Персонажа    Новое описание* - Изменить уровень персонажа \n" +
                "**$updateBio** *Имя_Персонажа    Новое описание* - Изменить биографию персонажа \n" +
                "**$updateInv** *Имя_Персонажа    Новое описание* - Изменить инвентарь персонажа \n" +
                "**$updateInt** *Имя_Персонажа    Новое описание* - Изменить навыки интеллекта персонажа \n" +
                "**$updateMag** *Имя_Персонажа    Новое описание* - Изменить заклинания персонажа \n" +
                "**$updateNat** *Имя_Персонажа    Новое описание* - Изменить навыки естества персонажа \n" +
                "**$icon** *Имя_Персонажа    http://adress.jpeg* - добавить иллюстрацию персонажу"
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

        internal static string part1 = @"
            <!DOCTYPE html>
            <html>
              <head>
                  <meta charset=utf-8>
                    <style>
                        @font-face{
                            font-family: TrajanPro;
                            src: url(https://cdn.discordapp.com/attachments/708001747842498623/708008151148003448/ofont.ru_Trajan.ttf) format(truetype);
                        }
                        h1.TrajanPro{
                            font-family: TrajanPro;
                            font-size:24pt
                        }
                        .TrajanPro{
                            font-family: TrajanPro;
                            font-size:10pt
                        }
                        p.TrajanPro{
                            font-family: TrajanPro;
                            font-size:10pt
                        }
                        img {
                                max-width: 100%;
                                max-height: 100%;
                        }
                        table{
                            border-collapse: collapse;
                            border: 1px solid grey;
                            background-color: black;
                        }
                        #imgstats { 
                                width:150px; 
                                height:350px; 
                                float:left; 
                                margin: 7px; 
                                padding: 10px;
                        }
                            #image { 
                                    width:150px; 
                                    height:150px; 
                                    padding: 10px;
                            }
                            #imageicon { 
                                    width:150px; 
                                    height:150px; 
                                    margin: -170px 0 0; 
                                    padding: 10px;
                            }
                            #special { 
                                    width:150px; 
                                    height:180px; 
                                    text-align:center;
                                    margin: 0 0 15px 0;
                                    padding: 10px;
                            }
                        #bio { 
                                width:250px; 
                                height:375px; 
                                background:url(https://media.discordapp.net/attachments/708001747842498623/708641364291878932/270395.png) no-repeat;
                                float:left; 
                                margin: 7px; 
                                text-align:center;
                                padding: 10px;
                        }
                        #skills { 
                                width:216px; 
                                height:300px;
                                background:url(https://media.discordapp.net/attachments/708001747842498623/708641276228141056/250375.png) no-repeat;
                                float: left; 
                                margin: 7px; 
                                text-align:center;
                                padding: 10px;
                        }
                        #head_block {
                            width:730;
                            height:60px; 
                        }
                            #nameblock {
                                display: inline-block;
                            }
                            #larrow {
                                margin:auto;
                                width:60px;
                                height:60px;
                                float:left;
                            }
                            #charname {
                                margin:auto;
                                height:40px;
                                display: inline-block;
                                line-height: 20px;
                                float:left;
                            }
                            #rarrow {
                                margin: auto;
                                width:60px;
                                height:60px;
                                float:left;
                            }
                    </style>
                </head>
                    
                <body bgcolor=#36393E><font color=#FFFFFF>  
                    <div id= head_block align=center>
                        <div id= nameblock>
                        <div id= larrow>
                            <img style= height:100% width:100% src= https://images.wikia.nocookie.net/hollowknight/ru/images/1/1c/%D0%A1%D1%82%D1%80%D0%B5%D0%BB%D0%BE%D1%87%D0%BA%D0%B02%D0%BB%D0%B5%D0%B2%D0%B0%D1%8F.png />
                        </div>
                        <div id= charname>                        
                            <h1 class=TrajanPro>";
        //Name
        internal static string part2 = @"</h1>     
                        </div>
                        <div id= rarrow>
                            <img style= height:100% width:100% src= https://images.wikia.nocookie.net/hollowknight/ru/images/8/82/%D0%A1%D1%82%D1%80%D0%B5%D0%BB%D0%BE%D1%87%D0%BA%D0%B02.png />
                        </div>
                        </div>
                    </div>

                    <div style= height:39px>
                        <img style= height:100% width:100% src= https://media.discordapp.net/attachments/708001747842498623/708659901383311427/Hr.png />
                    </div>                   

                    <div id = imgstats>
                        <div id = image align=center>
                            <img style= height:100% width:100% src= ";


//https://media.discordapp.net/attachments/567796677113676080/708007404830195814/20200507_202825.jpg 


        internal static string part3 = @" />
                        </div>
                        
                        <div id = imageicon>
                            <img style= height:100% width:100% src= https://cdn.discordapp.com/attachments/708001747842498623/708008561543741602/Jornal.png />
                        </div>

                        <div id= special> 
                            <table class= TrajanPro border= 2 width= 100%>";
        internal static string SetLevels(string input) 
        {
            string[] a = new string[7];
            for(int i = 0; i < a.Length; i++)
            {
                if (input[i] != '0')
                    a[i] = input[i].ToString();
                else
                    a[i] = "10";
            }

            return  $"<tr><td>Интеллект</td><td>{ a[0] }</td></tr>" +
                    $"<tr><td>Харизма</td><td>{ a[1] }</td></tr>" +
                    $"<tr><td>Ловкость</td><td>{ a[2] }</td></tr>" +
                    $"<tr><td>Магия</td><td>{ a[3] }</td></tr>" +
                    $"<tr><td>Сила</td><td>{ a[4] }</td></tr>" +
                    $"<tr><td>Выносливость</td><td>{ a[5] }</td></tr>" +
                    $"<tr><td>Естество</td><td>{ a[6] }</td></tr>"; 
        }
        
        internal static string part5 = @"</table>
                        </div> 
                    </div>

                    <div class= TrajanPro id= bio> 
                       <strong>Биография</strong>
                        <img src= https://vignette.wikia.nocookie.net/hollowknight/images/9/92/Spacer.png/revision/latest/scale-to-width-down/321?cb=20190126033524 />
                        <p style=text-align:left>";
        //Biography
        internal static string part6 = @"</p>
                    </div> 
                    
                    <div class= TrajanPro id= bio>
                        <strong>Инвентарь</strong>
                        <img src= https://vignette.wikia.nocookie.net/hollowknight/images/9/92/Spacer.png/revision/latest/scale-to-width-down/321?cb=20190126033524 />
                        <p class=TrajanPro style=text-align:left>";
        //Inventory
        internal static string part7 = @"</p>
                    </div>

                    <div class= TrajanPro id= skills> 
                        <strong>Навыки Интеллекта</strong>
                        <img src= https://vignette.wikia.nocookie.net/hollowknight/images/9/92/Spacer.png/revision/latest/scale-to-width-down/321?cb=20190126033524 />
                        <p style=text-align:left>";
        //Here we have Intellect skills
        internal static string part8 = @"
                </p>
                    </div>

                    <div class= TrajanPro id= skills> 
                        <strong>Заклинания</strong>
                        <img src= https://vignette.wikia.nocookie.net/hollowknight/images/9/92/Spacer.png/revision/latest/scale-to-width-down/321?cb=20190126033524 />
                        <p style=text-align:left>";
        //Here we have spells
        internal static string part9 = @"
                </p>
                    </div> 

                    <div class= TrajanPro id= skills> 
                        <strong>Навыки Естества</strong>
                        <img src= https://vignette.wikia.nocookie.net/hollowknight/images/9/92/Spacer.png/revision/latest/scale-to-width-down/321?cb=20190126033524 />
                        <p style=text-align:left>";
        //Here we have Nature skills 
        internal static string part10 = @"
                </p>
                    </div> 
                </body>
            </html>";
    }
}
