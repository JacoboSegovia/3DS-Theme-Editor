﻿using System;
using System.IO;
using System.Text;

using ThemeEditor.Common.Graphics;

namespace ThemeEditor.Common.SMDH
{
    public class SMDH
    {
        public static byte[] MAGIC =
        {
            0x53, 0x4D, 0x44, 0x48
        };

        public AppTitle[] AppTitles;
        public RawTexture LargeIcon;
        public short Reserved_1;
        public long Reserved_2;
        public AppSettings Settings;
        public RawTexture SmallIcon;
        public short Version;

        public SMDH() : this(true)
        {
            Version = 0;
            Reserved_1 = 0;
            Reserved_2 = 0;
            for (int i = 0; i < 16; i++)
                AppTitles[i] = new AppTitle();
            Settings = new AppSettings();
        }

        protected SMDH(bool inter)
        {
            AppTitles = new AppTitle[16];
            SmallIcon = new RawTexture(24, 24, RawTexture.DataFormat.Bgr565);
            LargeIcon = new RawTexture(48, 48, RawTexture.DataFormat.Bgr565);
        }

        public static SMDH Read(Stream s)
        {
            if (!s.CanSeek)
                throw new ArgumentException("Stream can't Seek", nameof(s));

            var smdh = new SMDH(true);
            using (var br = new BinaryReader(s, Encoding.ASCII, true))
            {
                var flag = br.ReadUInt32() == BitConverter.ToUInt32(MAGIC, 0);
                if (!flag)
                    throw new InvalidDataException("Not a SMDH File");

                smdh.Version = br.ReadInt16();
                smdh.Reserved_1 = br.ReadInt16();

                smdh.AppTitles = new AppTitle[16];
                for (int i = 0; i < 16; i++)
                    smdh.AppTitles[i] = AppTitle.Read(s);

                smdh.Settings = AppSettings.Read(s);

                smdh.Reserved_2 = br.ReadInt64();
                smdh.SmallIcon.Read(s);
                smdh.LargeIcon.Read(s);
            }
            return smdh;
        }

        public static void Write(SMDH smdh, Stream s)
        {
            using (var bw = new BinaryWriter(s, Encoding.ASCII, true))
            {
                bw.Write(MAGIC);
                bw.Write(smdh.Version);
                bw.Write(smdh.Reserved_1);

                for (int i = 0; i < 16; i++)
                    AppTitle.Write(smdh.AppTitles[i], s);

                AppSettings.Write(smdh.Settings, s);
                bw.Write(smdh.Reserved_2);
                bw.Write(smdh.SmallIcon.Data);
                bw.Write(smdh.LargeIcon.Data);
            }
        }
    }
}
