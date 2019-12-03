using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;

namespace flashkit_md
{
    class Device
    {

        const byte CMD_ADDR = 0;
        const byte CMD_LEN = 1;
        const byte CMD_RD = 2;
        const byte CMD_WR = 3;
        const byte CMD_RY = 4;
        const byte CMD_DELAY = 5;
        const byte PAR_INC = 128;
        const byte PAR_SINGE = 64;
        const byte PAR_DEV_ID = 32;
        const byte PAR_MODE8 = 16;

        static SerialPort port;


        public static void connect()
        {
            string[] ports = SerialPort.GetPortNames();
            int id;

            for (int i = 0; i < ports.Length; i++)
            {

                try
                {
                    port = new SerialPort(ports[i]);
                    port.Open();
                    port.ReadTimeout = 200;
                    port.WriteTimeout = 200;
                    id = getID();
                    if ((id & 0xff) == (id >> 8) && id != 0)
                    {
                        setDelay(0);
                        port.WriteTimeout = 2000;
                        port.ReadTimeout = 2000;
                        return;
                    }
                }
                catch (Exception) {
                    try { port.Close(); }
                    catch (Exception) { }
                }

            }

            port = null;
            throw new Exception("Device is not detected");
        }

        

        public static void disconnect()
        {
            if (port == null) return;
            try
            {
                port.Close();
                port = null;
            }
            catch (Exception) { }
        }

        public static string getPortName()
        {
            return port.PortName;
        }

        public static bool isConnected()
        {
            if (port == null) return false;

            return true;
        }

        static int getID()
        {
            int id;
            byte[] cmd = new byte[1];
            cmd[0] = CMD_RD | PAR_SINGE | PAR_DEV_ID;
            port.Write(cmd, 0, 1);
            id = port.ReadByte() << 8;
            id |= port.ReadByte();
            return id;
        }

        public static void setDelay(int val)
        {
            byte[] cmd = new byte[2];
            cmd[0] = CMD_DELAY;
            cmd[1] = (byte)val;
            port.Write(cmd, 0, cmd.Length);
        }

        public static UInt16 readWord(int addr)
        {
            UInt16 val = 0;
            addr /= 2;

            byte[] cmd = new byte[7];

            cmd[0] = CMD_ADDR;
            cmd[1] = (byte)(addr >> 16);
            cmd[2] = CMD_ADDR;
            cmd[3] = (byte)(addr >> 8);
            cmd[4] = CMD_ADDR;
            cmd[5] = (byte)(addr);

            cmd[6] = CMD_RD | PAR_SINGE;

            port.Write(cmd, 0, cmd.Length);

            val = (UInt16)(port.ReadByte() << 8);
            val |= (UInt16)port.ReadByte();

            return val;
        }

        public static void writeWord(int addr, UInt16 data)
        {
            byte[] cmd = new byte[9];
            addr /= 2;

            cmd[0] = CMD_ADDR;
            cmd[1] = (byte)(addr >> 16);
            cmd[2] = CMD_ADDR;
            cmd[3] = (byte)(addr >> 8);
            cmd[4] = CMD_ADDR;
            cmd[5] = (byte)(addr);

            cmd[6] = CMD_WR | PAR_SINGE;
            cmd[7] = (byte)(data >> 8);
            cmd[8] = (byte)(data);

            port.Write(cmd, 0, 9);
        }

        public static void writeByte(int addr, byte data)
        {
            byte[] cmd = new byte[8];
            addr /= 2;

            cmd[0] = CMD_ADDR;
            cmd[1] = (byte)(addr >> 16);
            cmd[2] = CMD_ADDR;
            cmd[3] = (byte)(addr >> 8);
            cmd[4] = CMD_ADDR;
            cmd[5] = (byte)(addr);

            cmd[6] = CMD_WR | PAR_SINGE | PAR_MODE8;
            cmd[7] = data;

            port.Write(cmd, 0, cmd.Length);
        }

        public static void read(byte[] buff, int offset, int len)
        {

            int rd_len;

            while (len > 0)
            {
                rd_len = len > 65536 ? 65536 : len;
                byte[] cmd = new byte[5];
                cmd[0] = CMD_LEN;
                cmd[1] = (byte)(rd_len / 2 >> 8);
                cmd[2] = CMD_LEN;
                cmd[3] = (byte)(rd_len / 2);
                cmd[4] = CMD_RD | PAR_INC;

                port.Write(cmd, 0, 5);

                for (int i = 0; i < rd_len; )
                {
                    i += port.Read(buff, offset + i, rd_len - i);
                }
                len -= rd_len;
                offset += rd_len;
            }
        }

        public static void write(byte[] buff, int offset, int len)
        {

            int wr_len;

            while (len > 0)
            {
                wr_len = len > 65536 ? 65536 : len;
                byte[] cmd = new byte[5];
                cmd[0] = CMD_LEN;
                cmd[1] = (byte)(wr_len / 2 >> 8);
                cmd[2] = CMD_LEN;
                cmd[3] = (byte)(wr_len / 2);
                cmd[4] = CMD_WR | PAR_INC;

                port.Write(cmd, 0, 5);

                port.Write(buff, offset, wr_len);

                len -= wr_len;
                offset += wr_len;
            }
        }

        public static void setAddr(int addr)
        {
            byte[] buff = new byte[6];
            addr /= 2;

            buff[0] = CMD_ADDR;
            buff[1] = (byte)(addr >> 16);
            buff[2] = CMD_ADDR;
            buff[3] = (byte)(addr >> 8);
            buff[4] = CMD_ADDR;
            buff[5] = (byte)(addr);

            port.Write(buff, 0, 6);

        }

        public static void flashErase(int addr)
        {

            byte[] cmd;
            addr /= 2;

            cmd = new byte[8 * 8];

            for (int i = 0; i < cmd.Length; i += 8)
            {
                cmd[0 + i] = CMD_ADDR;
                cmd[1 + i] = (byte)(addr >> 16);
                cmd[2 + i] = CMD_ADDR;
                cmd[3 + i] = (byte)(addr >> 8);
                cmd[4 + i] = CMD_ADDR;
                cmd[5 + i] = (byte)(addr);

                cmd[6 + i] = CMD_WR | PAR_SINGE | PAR_MODE8;
                cmd[7 + i] = 0x30;
                addr += 4096;
            }

            Device.writeWord(0x555 * 2, 0xaa);
            Device.writeWord(0x2aa * 2, 0x55);
            Device.writeWord(0x555 * 2, 0x80);
            Device.writeWord(0x555 * 2, 0xaa);
            Device.writeWord(0x2aa * 2, 0x55);
            //writeWord(addr, 0x30);

            port.Write(cmd, 0, cmd.Length);
            flashRY();

        }

        static void flashRY()
        {
            byte[] cmd = new byte[2];
            cmd[0] = CMD_RY;
            cmd[1] = CMD_RD | PAR_SINGE;

            port.Write(cmd, 0, 2);
            port.ReadByte();
            port.ReadByte();
        }

        public static void flashUnlockBypass()
        {
            Device.writeByte(0x555 * 2, 0xaa);
            Device.writeByte(0x2aa * 2, 0x55);
            Device.writeByte(0x555 * 2, 0x20);
        }

        public static void flashResetByPass()
        {
            Device.writeWord(0, 0xf0);
            Device.writeByte(0, 0x90);
            Device.writeByte(0, 0x00);
        }

        public static void flashWrite(byte[] buff, int offset, int len)
        {

            len /= 2;
            byte[] cmd = new byte[6 * len];

            for (int i = 0; i < cmd.Length; i += 6)
            {
                cmd[0 + i] = CMD_WR | PAR_SINGE | PAR_MODE8;
                cmd[1 + i] = 0xA0;
                cmd[2 + i] = CMD_WR | PAR_SINGE | PAR_INC;
                cmd[3 + i] = buff[offset++];
                cmd[4 + i] = buff[offset++];
                cmd[5 + i] = CMD_RY;
            }


            port.Write(cmd, 0, cmd.Length);

        }

        
    }
}
