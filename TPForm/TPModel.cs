﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TPMeshEditor
{
    public class TPModel : ILoggable, IByteArrayCapable
    {
        /// <summary>
        /// Main constructor.
        /// </summary>
        /// <param name="_rawdata">
        /// Supply data from ModelSize to the last animation frame byte.
        /// </param>
        public TPModel(List<byte> _rawdata)
        {
            if (_rawdata.Count() < 1 || _rawdata.Count() < ((Data4Bytes)(_rawdata.GetRange(0, 4))).ui + 4)
            {
                throw new ArgumentException("Incorrect initialisation array size: " + _rawdata.Count + ", expected: " + ((Data4Bytes)(_rawdata.GetRange(0, 4))).ui + 1);
            }

            log = new StringBuilder();
            vertices = new List<TPVertex>();
            faces = new List<TPFace>();
            animationFrames = new List<TPAnimFrame>();

            //using internal method to import to avoid having a complicated constructor.
            try
            {
                Set(_rawdata);
            }
            catch (ArgumentException e)
            {

            }
        }

        private List<TPVertex> vertices;
        private List<TPFace> faces;
        private List<TPAnimFrame> animationFrames;
        private StringBuilder log;

        public uint Size
        {
            get;
            private set;
        }
        public uint ID
        {
            get;
            private set;
        }
        public uint VertexCount
        {
            get;
            private set;
        }
        public uint FaceCount
        {
            get;
            private set;
        }
        public uint AnimationFrameCount
        {
            get;
            private set;
        }

        public void Set(List<byte> _rawdata)
        {
            /*
                 * DO KEEP IN MIND THAT THE COUNTERS HERE REFER TO BYTES.
             */

            /*Using BitConverter would be a bit longer, but maybe clearer?
             * I think that once understood the principle, this is better.
             * This would be the same thing with BitConverter:             * 
             * */
            //Size = BitConverter.ToUInt32(_rawdata.GetRange(0, 4).ToArray(), 0);
            Size = ((Data4Bytes)(_rawdata.GetRange(0, 4))).ui;
            ID = ((Data4Bytes)(_rawdata.GetRange(4, 4))).ui;
            VertexCount = ((Data4Bytes)(_rawdata.GetRange(8, 4))).ui;

            uint currentPositionInData = 12;

            //should allow for vertex size that is more than 32 bytes.
            uint i = VertexCount;
            while (i-- > 0)
            {
                uint tempsize = ((Data4Bytes)(_rawdata.GetRange((int)currentPositionInData, 4))).ui;

                TPVertex temp;

                /* We get a number of bytes that is the size + 4, because we also supply the size to the vertex object. 
                 */
                temp = new TPVertex(_rawdata.GetRange((int)currentPositionInData, (int)(tempsize + 4)));
                vertices.Add(temp);
                if (temp.PeekLog() != "")
                {
                    log.AppendLine("Vertex " + i + ": " + temp.DumpLog());
                }

                currentPositionInData += tempsize + 4;
            }

            FaceCount = ((Data4Bytes)(_rawdata.GetRange((int)currentPositionInData, 4))).ui;
            currentPositionInData += 4;

            i = FaceCount;
            while (i-- > 0)
            {
                uint tempsize = ((Data4Bytes)(_rawdata.GetRange((int)currentPositionInData, 4))).ui;

                TPFace temp;

                //adds face data to temp face, remembering that we are dealing with 4-byte data and the size counter counts single bytes
                temp = new TPFace(_rawdata.GetRange((int)currentPositionInData, (int)(tempsize + 4)));
                faces.Add(temp);
                if (temp.PeekLog() != "")
                {
                    log.AppendLine("Face " + i + ": " + temp.DumpLog());
                }

                currentPositionInData += tempsize + 4;
            }

            AnimationFrameCount = ((Data4Bytes)(_rawdata.GetRange((int)currentPositionInData, 4))).ui;
            currentPositionInData += 4;

            i = AnimationFrameCount;
            while (i-- > 0)
            {
                uint tempsize = ((Data4Bytes)(_rawdata.GetRange((int)currentPositionInData, 4))).ui;

                TPAnimFrame temp;

                temp = new TPAnimFrame(_rawdata.GetRange((int)currentPositionInData, (int)(tempsize + 4)));
                animationFrames.Add(temp);
                if (temp.PeekLog() != null)
                {
                    log.AppendLine("Face " + i + ": " + temp.DumpLog());
                }

                currentPositionInData += tempsize + 4;
            }
        }

        public List<byte> Get()
        {
            List<byte> output = new List<byte>((int)Size + 4);

            output.AddRange(((Data4Bytes)Size).B);
            output.AddRange(((Data4Bytes)ID).B);

            output.AddRange(((Data4Bytes)VertexCount).B);
            foreach (TPVertex v in vertices)
            {
                output.AddRange(v.Get());
            }

            output.AddRange(((Data4Bytes)FaceCount).B);
            foreach (TPFace f in faces)
            {
                output.AddRange(f.Get());
            }

            output.AddRange(((Data4Bytes)AnimationFrameCount).B);
            foreach (TPAnimFrame a in animationFrames)
            {
                output.AddRange(a.Get());
            }

            return output;
        }

        public void Transform(float[,] _matrix)
        {
            foreach(TPVertex v in vertices)
            {
                v.Transform(_matrix);
            }
        }

        public string PeekLog()
        {
            return log.ToString();
        }

        public string DumpLog()
        {
            string output;
            output = PeekLog();
            log.Clear();

            return output;
        }
    }

}