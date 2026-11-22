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
using OpenMetaverse;
using OpenSim.Data;

namespace Diva.Data.PGSQL
{
    /// <summary>
    /// A RegionData interface to the PostgreSQL database, with the Diva extensions.
    /// </summary>
    /// <remarks>
    /// Migrations come from this assembly's Resources/GridStore.migrations (a copy of OpenSim's),
    /// because OpenSim's PGSQLRegionData migrates from GetType().Assembly.
    /// </remarks>
    public class PGSQLRegionData : OpenSim.Data.PGSQL.PGSQLRegionData, IRegionData
    {
        // The base class keeps these private.
        private readonly string m_ConnString;
        private readonly string m_RealmName;

        public PGSQLRegionData(string connectionString, string realm)
            : base(connectionString, realm)
        {
            m_ConnString = connectionString;
            m_RealmName = realm;
        }

        public RegionData[] Get(UUID scopeID, int regionFlags, int excludeFlags)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(m_ConnString))
            using (NpgsqlCommand cmd = new NpgsqlCommand())
            {
                cmd.CommandText = "SELECT * FROM " + m_RealmName + " WHERE " +
                        CreateWhereClause(cmd, scopeID, regionFlags, excludeFlags);
                cmd.Connection = conn;
                conn.Open();

                return RunCommand(cmd).ToArray();
            }
        }

        public long GetCount(UUID scopeID, int regionFlags, int excludeFlags)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(m_ConnString))
            using (NpgsqlCommand cmd = new NpgsqlCommand())
            {
                cmd.CommandText = "SELECT count(*) FROM " + m_RealmName + " WHERE " +
                        CreateWhereClause(cmd, scopeID, regionFlags, excludeFlags);
                cmd.Connection = conn;
                conn.Open();

                return Convert.ToInt64(cmd.ExecuteScalar());
            }
        }

        private static string CreateWhereClause(NpgsqlCommand cmd, UUID scopeID, int regionFlags, int excludeFlags)
        {
            string where = "(\"flags\" & :regionFlags) <> 0 AND (\"flags\" & :excludeFlags) = 0";
            cmd.Parameters.AddWithValue("regionFlags", regionFlags);
            cmd.Parameters.AddWithValue("excludeFlags", excludeFlags);

            if (!scopeID.IsZero())
            {
                where += " AND \"ScopeID\" = :scopeID";
                cmd.Parameters.AddWithValue("scopeID", scopeID.Guid);
            }

            return where;
        }
    }
}
