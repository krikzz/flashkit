using System;
using System.Collections.Generic;
using System.Text;

namespace flashkit_md
{
    class Cart
    {
        public const int MAP_2M = 1;
        public const int MAP_3M = 2;
        public const int MAP_SSF = 3;

        static string getRomRegion(byte[] rom_hdr)
        {

            byte val = rom_hdr[0x1f0];
            if (val != rom_hdr[0x1f1] && rom_hdr[0x1f1] != 0x20 && rom_hdr[0x1f1] != 0) return "W";

            switch (val)
            {

                case (byte)'F':
                case (byte)'C':
                    return "W";

                case (byte)'U':
                case (byte)'W':
                case (byte)'4':
                case 4:
                    return "U";

                case (byte)'J':
                case (byte)'B':
                case (byte)'1':
                case 1:
                    return "J";

                case (byte)'E':
                case (byte)'A':
                case (byte)'8':
                case 8:
                    return "E";

            }

            return "X";

        }

        public static string getRomName()
        {
            string name = null;
            byte[] rom_hdr = new byte[512];
            Device.setAddr(0);
            Device.read(rom_hdr, 0, 512);

            name = getRomName(0x120, rom_hdr);

            if (name == null) name = getRomName(0x150, rom_hdr);

            if (name == null) name = "Unknown";

            name += " (" + getRomRegion(rom_hdr)+")";

            return name;
        }

        static string getRomName(int offset, byte []buff)
        {
            string name = "";
            int name_empty = 1;
        

            for (int i = offset + 47; i >= offset; i--)
            {
                if (buff[i] != 0 & buff[i] != 0x20) break;
                if (buff[i] == 0x20) buff[i] = 0;
            }

            for (int i = offset; i < offset + 48; i++)
            {
                if (buff[i] == 0) break;
                if (buff[i] == '/' || buff[i] == ':') buff[i] = (byte)'-';
                try
                {
                    name += (char)buff[i];
                    if (buff[i] != 0x20) name_empty = 0;
                    if (buff[i] == ' ' || buff[i] == '!' || buff[i] == '(' || buff[i] == ')' || buff[i] == '_' || buff[i] == '-') continue;
                    if (buff[i] == '.' || buff[i] == '[' || buff[i] == ']' || buff[i] == '|') continue;
                    if (buff[i] == '&' || buff[i] == 0x27 || buff[i] == 0x60) continue;
                    if (buff[i] >= '0' && buff[i] <= '9') continue;
                    if (buff[i] >= 'A' && buff[i] <= 'Z') continue;
                    if (buff[i] >= 'a' && buff[i] <= 'z') continue;

                    throw new Exception("name error");
                }
                catch (Exception)
                {
                    return null;
                }
            }

            if (name_empty != 0) return null;
            return name;
        }

        static int checkRomSize(int base_addr, int max_len)
        {
            int eq;
            int base_len = 0x8000;
            int len = 0x8000;
            byte[] sector0 = new byte[256];
            byte[] sector = new byte[256];
            Device.writeWord(0xA13000, 0x0000);

            Device.setAddr(base_addr);
            Device.read(sector0, 0, sector.Length);

            for (; ; )
            {

                Device.setAddr(base_addr + len);
                Device.read(sector, 0, sector.Length);

                eq = 1;
                for (int i = 0; i < sector.Length; i++) if (sector0[i] != sector[i]) eq = 0;
                if (eq == 1) break;

                len *= 2;
                if (len >= max_len) break;
            }

            if (len == base_len) return 0;
            return len;
        }



        public static int getRomSize()
        {

            
            byte[] sector0 = new byte[512];
            byte[] sector = new byte[512];
            int ram = 0;
            int extra_rom = 0;

            if (ramAvailable())
            {
                ram = 1;
                extra_rom = 1;
                Device.writeWord(0xA13000, 0x0000);
                Device.setAddr(0x200000);
                Device.read(sector0, 0, 512);
                Device.setAddr(0x200000);
                Device.read(sector, 0, 512);
                for (int i = 0; i < sector.Length; i++) if (sector[i] != sector0[i]) extra_rom = 0;

                if (extra_rom != 0)
                {
                    extra_rom = 0;
                    Device.setAddr(0x200000 + 0x10000);
                    Device.read(sector, 0, 512);

                    Device.writeWord(0xA13000, 0xffff);
                    Device.setAddr(0x200000);
                    Device.read(sector, 0, 512);
                    for (int i = 0; i < sector.Length; i++) if (sector[i] != sector0[i]) extra_rom = 1;

                }
              
            }

            int max_rom_size = ram != 0 && extra_rom == 0 ? 0x200000 : 0x400000;
            int len = checkRomSize(0, max_rom_size);

            if (len == 0x400000)
            {
                len = 0x200000;
                int len2 = checkRomSize(0x200000, 0x200000);
                if (len2 == 0x200000)
                {
                    len2 = checkRomSize(0x300000, 0x100000);
                    len2 = len2 >= 0x80000 ? 0x200000 : 0x100000;
                }
                if (len2 >= 0x80000) len += len2;
            }

            return len;

        }

        static bool ramAvailable()
        {
            int first_word;
            UInt16 tmp;

            Device.writeWord(0xA13000, 0xffff);

            first_word = Device.readWord(0x200000);
            Device.writeWord(0x200000, (UInt16)(first_word ^ 0xffff));
            tmp = Device.readWord(0x200000);
            Device.writeWord(0x200000, (UInt16)first_word);
            tmp ^= 0xffff;
            if ((first_word & 0x00ff) != (tmp & 0x00ff)) return false;

            return true;
        }

        public static int getRamSize()
        {
            int ram_szie = 256;
            int first_word;
            int first_word_tmp;
            UInt16 tmp;
            UInt16 tmp2;

           int ram_type = 0x00ff;

            //Device.writeWord(0xA13000, 0x0001);

            if (!ramAvailable()) return 0;

            first_word = Device.readWord(0x200000);

            while (ram_szie < 0x100000)
            {
                tmp = Device.readWord(0x200000 + ram_szie);
                Device.writeWord(0x200000 + ram_szie, (UInt16)(tmp ^ 0xffff));
                tmp2 = Device.readWord(0x200000 + ram_szie);
                first_word_tmp = Device.readWord(0x200000);
                Device.writeWord(0x200000 + ram_szie, tmp);
                tmp2 ^= 0xffff;
                if ((tmp & 0xff) != (tmp2 & 0xff)) break;
                if ((first_word & ram_type) != (first_word_tmp & ram_type)) break;
                ram_szie *= 2;
            }

            return ram_szie / 2;
        }

        public static byte[] getRam()
        {
            byte[] buff = null;
            

            int ram_size = 0;
            ram_size = Cart.getRamSize() * 2;

            buff = new byte[ram_size];

            Device.writeWord(0xA13000, 0xffff);
            Device.setAddr(0x200000);
            
            Device.read(buff, 0, buff.Length);

            return buff;
        }

        
    }
}
