﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TPMeshEditor
{
    class TPEntity : ILoggable
    {
        TPEntity(string _filename)
        {

        }

        private List<TPMesh> meshes;
        private StringBuilder log;

        /// <summary>
        /// Returns the log without deleting it.
        /// </summary>
        /// <returns>Log as String.</returns>
        /// <remarks>Prefer DumpLog instead.</remarks>
        public string PeekLog()
        {
            return log.ToString();
        }

        /// <summary>
        /// Returns the log and deletes it (like flushing a stream).
        /// </summary>
        /// <returns>Log as String.</returns>
        /// <remarks>This function should be preferred to PeekLog.</remarks>
        public string DumpLog()
        {
            string output;
            output = PeekLog();
            log.Clear();

            return output;
        }
    }
}
