namespace Aspid
{
    class Queries
    {
        public static string CreateTable(ulong id)
        {
            return 
                $"CREATE TABLE GUILD_{id} (№ SERIAL NOT NULL UNIQUE, U_ID BIGINT NOT NULL PRIMARY KEY, ISMUITED BOOL DEFAULT FALSE, MUTETIME INT DEFAULT 0, ISPUNISHED BOOL DEFAULT FALSE, PUNISHTIME INT DEFAULT 0); " +

                $"CREATE TABLE RP_{id} (№ SERIAL PRIMARY KEY, CHAR_NAME TEXT NOT NULL UNIQUE, CHAR_OWNER BIGINT NOT NULL REFERENCES GUILD_{id} (U_ID) ON UPDATE CASCADE, DESCRIPTION TEXT, CHAR_IMAGE TEXT)";
        }

        #region Users

        public static string AddUser(ulong guild, ulong id)
        {
            return $"INSERT INTO GUILD_{guild} (U_ID) VALUES({id});";
        }

        public static string GetUser(ulong guild, ulong id)
        {
            return $"SELECT * FROM GUILD_{guild} WHERE U_ID={id};";
        }

        public static string DeleteUser(ulong guild, ulong id)
        {
            return $"DELETE FROM GUILD_{guild} WHERE U_ID={id};";
        }

        #endregion

        public static string PenaltyHandler(ulong guild)
        {
            return
            $"UPDATE GUILD_{guild} SET MUTETIME = MUTETIME - 1 WHERE ISMUITED = TRUE; " +
            $"UPDATE GUILD_{guild} SET PUNISHTIME = PUNISHTIME - 1 WHERE ISPUNISHED = TRUE; ";
        }


        #region Mute

        public static string GetMuted(ulong guild)
        {
            return $"SELECT U_ID FROM GUILD_{guild} WHERE ISMUITED = TRUE AND MUTETIME <= 0;";
        }

        public static string RemoveMute(ulong guild, ulong id)
        {
            return $"UPDATE GUILD_{guild} SET ISMUITED = FALSE WHERE U_ID = {id};";
        }

        public static string AddMute(ulong guild, ulong id, ulong mutetime)
        {
            return $"UPDATE GUILD_{guild} SET ISMUITED = TRUE, MUTETIME = {mutetime} WHERE U_ID = {id};";
        }
        #endregion

        #region Punish

        public static string GetPunished(ulong guild)
        {
            return $"SELECT U_ID FROM GUILD_{guild} WHERE ISPUNISHED = TRUE AND PUNISHTIME <= 0;";
        }

        
        public static string RemovePunish(ulong guild, ulong id)
        {
            return $"UPDATE GUILD_{guild} SET ISPUNISHED = FALSE WHERE U_ID = {id};";
        }

        public static string AddPunish(ulong guild, ulong id)
        {
            return $"UPDATE GUILD_{guild} SET ISPUNISHED = TRUE, PUNISHTIME = 360 WHERE U_ID = {id};";
        }

        #endregion

        #region Roleplay

        public static string AddChar(ulong guild, string name, ulong dude, string description)
        {
            return $"INSERT INTO RP_{guild} (CHAR_NAME, CHAR_OWNER, DESCRIPTION) VALUES('{name}', {dude}, '{description}');";
        }

        public static string ChangeDescription(ulong guild, string name, string description)
        {
            return $"UPDATE RP_{guild} SET DESCRIPTION = '{description}' WHERE CHAR_NAME = '{name}'";
        }

        public static string ChangeImage(ulong guild, string name, string description)
        {
            return $"UPDATE RP_{guild} SET CHAR_IMAGE = '{description}' WHERE CHAR_NAME = '{name}'";
        }

        public static string DeleteChar(ulong guild, string name)
        {
            return $"DELETE FROM RP_{guild} WHERE CHAR_NAME = '{name}'";
        }

        public static string GetCharacter(ulong guild, string name)
        {
            return $"SELECT * FROM RP_{guild} WHERE CHAR_NAME = '{name}'";
        }

        public static string GetCharacter(ulong guild, ulong id)
        {
            return $"SELECT * FROM RP_{guild} WHERE CHAR_OWNER = {id}";
        }

        public static string GetAllCharacters(ulong guild)
        {
            return $"SELECT * FROM RP_{guild}";
        }
        #endregion
    }
}
