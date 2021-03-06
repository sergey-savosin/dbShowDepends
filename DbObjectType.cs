﻿using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace dbShowDepends
{
    /*
    type	type_desc
    C 	CHECK_CONSTRAINT
    D 	DEFAULT_CONSTRAINT
    F 	FOREIGN_KEY_CONSTRAINT
    FN	SQL_SCALAR_FUNCTION
    FS	CLR_SCALAR_FUNCTION
    IF	SQL_INLINE_TABLE_VALUED_FUNCTION
    IT	INTERNAL_TABLE
    P 	SQL_STORED_PROCEDURE
    PK	PRIMARY_KEY_CONSTRAINT
    S 	SYSTEM_TABLE
    SQ	SERVICE_QUEUE
    TF	SQL_TABLE_VALUED_FUNCTION
    TR	SQL_TRIGGER
    TT	TYPE_TABLE
    U 	USER_TABLE
    UQ	UNIQUE_CONSTRAINT
    V 	VIEW
     */

    public enum DbObjectType { P, FN, IF, TF, U, FK, V, TR, TT };

    public class DbObjectTypes
    {
        private static List<string> objectTypeList = new List<string>();

        /// <summary>
        /// Вернуть список типов объектов БД
        /// </summary>
        /// <returns>список строк</returns>
        public static List<string> GetObjectTypes()
        {
            ensureList();

            return objectTypeList;
        }

        /// <summary>
        /// Относится ли индекс типа объекта к определённому типу?
        /// </summary>
        /// <param name="objTypeIndex">Индекс типа объекта, >= 0</param>
        /// <returns>true, если индекс соответствует типу объекта</returns>
        public static bool IsDetermineType(int objTypeIndex)
        {
            return (objTypeIndex == 0 || objTypeIndex == 1) ? false : true;
        }

        /// <summary>
        /// Найти указанный тип объекта БД
        /// </summary>
        /// <param name="objTypeStr">тип объекта БД (строка)</param>
        /// <returns>-1 если не найдено</returns>
        public static int GetObjectTypeIndex(string objTypeStr)
        {
            int res;
            ensureList();

            if (string.IsNullOrEmpty(objTypeStr) )
                objTypeStr = "?";

            res = objectTypeList.FindIndex(p => p == objTypeStr);
            if (res == -1)
            {
                objectTypeList.Add(objTypeStr);
            }
            else
            {
                return res;
            }

            return objectTypeList.FindIndex(p => p == objTypeStr);

        }

        /// <summary>
        /// Finds caption for object type (U=>User table)
        /// </summary>
        /// <param name="objTypeStr"></param>
        /// <returns></returns>
        public static string GetObjectTypeText(string objTypeStr)
        {
            string res;
            ensureList();

            if (string.IsNullOrEmpty(objTypeStr))
                return "?";

            switch (objTypeStr)
            {
                case "C": res = "CHECK_CONSTRAINT"; break;
                case "D": res = "DEFAULT_CONSTRAINT"; break;
                case "F": res = "FOREIGN_KEY_CONSTRAINT"; break;
                case "FN": res = "SQL_SCALAR_FUNCTION"; break;
                case "FS": res = "CLR_SCALAR_FUNCTION"; break;
                case "IF": res = "SQL_INLINE_TABLE_VALUED_FUNCTION"; break;
                case "IT": res = "INTERNAL_TABLE"; break;
                case "P": res = "SQL_STORED_PROCEDURE"; break;
                case "PK": res = "PRIMARY_KEY_CONSTRAINT"; break;
                case "S": res = "SYSTEM_TABLE"; break;
                case "SQ": res = "SERVICE_QUEUE"; break;
                case "TF": res = "SQL_TABLE_VALUED_FUNCTION"; break;
                case "TR": res = "SQL_TRIGGER"; break;
                case "TT": res = "TYPE_TABLE"; break;
                case "U": res = "USER_TABLE"; break;
                case "UQ": res = "UNIQUE_CONSTRAINT"; break;
                case "V": res = "VIEW"; break;
                default: res = "?"; break;
            }

            return res;

        }

        /// <summary>
        /// Поддержка списка заполненным
        /// </summary>
        private static void ensureList()
        {
            if (objectTypeList.Count == 0)
            {
                objectTypeList.Add("?");

                foreach (var objType in Enum.GetValues(typeof(DbObjectType)).Cast<DbObjectType>())
                {
                    objectTypeList.Add(objType.ToString());
                }
            }
        }

    }
}
