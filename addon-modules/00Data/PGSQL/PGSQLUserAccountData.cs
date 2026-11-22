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
    /// A PostgreSQL interface for user accounts, with the Diva extensions.
    /// </summary>
    /// <remarks>
    /// Migrations come from this assembly's Resources/UserAccount.migrations (a copy of OpenSim's),
    /// because OpenSim's PostgreSQL generic handler migrates from GetType().Assembly.
    /// ILIKE matches MySQL's default case-insensitive LIKE.
    /// </remarks>
    public class PGSQLUserAccountData : OpenSim.Data.PGSQL.PGSQLUserAccountData, IUserAccountData
    {
        public PGSQLUserAccountData(string connectionString, string realm)
                : base(connectionString, realm)
        {
        }

        public UserAccountData[] GetActiveAccounts(UUID scopeID, string query, string excludeTerm)
        {
            string[] words = query.Split(new char[] { ' ' });

            for (int i = 0; i < words.Length; i++)
            {
                if (words[i].Length < 3)
                {
                    if (i != words.Length - 1)
                        Array.Copy(words, i + 1, words, i, words.Length - i - 1);
                    Array.Resize(ref words, words.Length - 1);
                }
            }

            if (words.Length == 0)
                return new UserAccountData[0];

            if (words.Length > 2)
                return new UserAccountData[0];

            using (NpgsqlConnection conn = new NpgsqlConnection(m_ConnectionString))
            using (NpgsqlCommand cmd = new NpgsqlCommand())
            {
                string sql = String.Format("select * from {0} where {1} and ", m_Realm, ScopeClause(cmd, scopeID));

                if (words.Length == 1)
                {
                    sql += "(\"FirstName\" ILIKE :search or \"LastName\" ILIKE :search)";
                    cmd.Parameters.AddWithValue("search", "%" + words[0] + "%");
                }
                else
                {
                    sql += "(\"FirstName\" ILIKE :searchFirst or \"LastName\" ILIKE :searchLast)";
                    cmd.Parameters.AddWithValue("searchFirst", "%" + words[0] + "%");
                    cmd.Parameters.AddWithValue("searchLast", "%" + words[1] + "%");
                }

                sql += " and (\"FirstName\" NOT ILIKE :exclude)";
                cmd.Parameters.AddWithValue("exclude", excludeTerm + "%");

                cmd.CommandText = sql;
                cmd.Connection = conn;
                conn.Open();

                return DoQuery(cmd);
            }
        }

        public long GetActiveAccountsCount(UUID scopeID, string excludeTerm)
        {
            using (NpgsqlCommand cmd = new NpgsqlCommand())
            {
                cmd.CommandText = String.Format("select count(*) from {0} where {1} and (\"FirstName\" NOT ILIKE :exclude)",
                        m_Realm, ScopeClause(cmd, scopeID));
                cmd.Parameters.AddWithValue("exclude", excludeTerm + "%");

                return Convert.ToInt64(DoQueryScalar(cmd));
            }
        }

        private static string ScopeClause(NpgsqlCommand cmd, UUID scopeID)
        {
            cmd.Parameters.AddWithValue("ScopeID", scopeID.Guid);
            cmd.Parameters.AddWithValue("UUIDZero", UUID.Zero.Guid);
            return "(\"ScopeID\" = :ScopeID or \"ScopeID\" = :UUIDZero)";
        }
    }
}
