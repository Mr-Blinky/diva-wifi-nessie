/*
 * Copyright (c) Contributors, http://opensimulator.org/
 * See CONTRIBUTORS.TXT for a full list of copyright holders.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 *     * Redistributions of source code must retain the above copyright
 *       notice, this list of conditions and the following disclaimer.
 *     * Redistributions in binary form must reproduce the above copyright
 *       notice, this list of conditions and the following disclaimer in the
 *       documentation and/or other materials provided with the distribution.
 *     * Neither the name of the OpenSimulator Project nor the
 *       names of its contributors may be used to endorse or promote products
 *       derived from this software without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE DEVELOPERS ``AS IS'' AND ANY
 * EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
 * DISCLAIMED. IN NO EVENT SHALL THE CONTRIBUTORS BE LIABLE FOR ANY
 * DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
 * ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */

using System;
using Npgsql;
using OpenSim.Data;

namespace Diva.Data.PGSQL
{
    /// <summary>
    /// A PostgreSQL interface for grid user data, with the Diva extensions.
    /// </summary>
    /// <remarks>
    /// Migrations come from this assembly's Resources/GridUserStore.migrations, which is
    /// OpenSim's PostgreSQL script plus a Diva version that adds the TOS column.
    /// Column names are quoted because the PostgreSQL schema uses mixed-case identifiers.
    /// </remarks>
    public class PGSQLGridUserData : OpenSim.Data.PGSQL.PGSQLGridUserData, IGridUserData
    {
        // OpenSim stores bool.ToString() ("True"/"False") in a CHAR(5); compare case-insensitively
        // as MySQL does by default.
        private const string OnlineClause = "LOWER(TRIM(\"Online\")) = 'true'";

        public PGSQLGridUserData(string connectionString, string realm)
                : base(connectionString, realm)
        {
        }

        // OpenSim's PGSQLGridUserData hides Get(string where) with Get(string userID),
        // so WHERE-clause queries go through the generic handler explicitly.
        private OpenSim.Data.PGSQL.PGSQLGenericTableHandler<GridUserData> Table
        {
            get { return this; }
        }

        public GridUserData[] GetOnlineUsers()
        {
            return Table.Get(OnlineClause);
        }

        public long GetOnlineUserCount()
        {
            return GetCount(OnlineClause);
        }

        public long GetActiveUserCount(int period)
        {
            // Logout holds Unix seconds as text. Equivalent of MySQL's
            // DATEDIFF(CURRENT_DATE(), FROM_UNIXTIME(Logout)) <= period, skipping non-numeric values.
            string where = OnlineClause +
                " OR (CASE WHEN TRIM(\"Logout\") ~ '^[0-9]+$'" +
                " THEN CURRENT_DATE - CAST(to_timestamp(CAST(TRIM(\"Logout\") AS bigint)) AS date)" +
                " END) <= " + period.ToString();

            return GetCount(where);
        }

        public GridUserData[] GetUsers(string pattern)
        {
            if (string.IsNullOrEmpty(pattern) || pattern.Trim().Length == 0)
                return Table.Get("TRUE ORDER BY \"UserID\"");

            return Table.Get("\"UserID\" ILIKE :pattern ORDER BY \"UserID\"",
                    new NpgsqlParameter("pattern", "%" + pattern + "%"));
        }

        public void ResetTOS()
        {
            using (NpgsqlCommand cmd = new NpgsqlCommand())
            {
                cmd.CommandText = String.Format("update {0} set \"TOS\" = :tos", m_Realm);
                cmd.Parameters.AddWithValue("tos", string.Empty);

                ExecuteNonQuery(cmd);
            }
        }

        public void ResetOnline()
        {
            using (NpgsqlCommand cmd = new NpgsqlCommand())
            {
                cmd.CommandText = String.Format("update {0} set \"Online\" = 'False'", m_Realm);

                ExecuteNonQuery(cmd);
            }
        }
    }
}
