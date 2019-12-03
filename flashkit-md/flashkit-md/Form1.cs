using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Security.Cryptography;

namespace flashkit_md
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.Text = this.Text + " " + this.ProductVersion;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
             
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void btn_check_Click(object sender, EventArgs e)
        {
            consWriteLine("-----------------------------------------------------");
            int ram_size;


            try
            {
                
                Device.connect();
                Device.setDelay(1);
                consWriteLine("Connected to: " + Device.getPortName());
                
              
                
                consWriteLine("ROM name : " + Cart.getRomName());
                consWriteLine("ROM size : " + Cart.getRomSize() / 1024 + "K");
                ram_size = Cart.getRamSize();
                if (ram_size < 1024)
                {
                    consWriteLine("RAM size : " + ram_size + "B");
                }
                else
                {
                    consWriteLine("RAM size : " + ram_size / 1024 + "K");
                }
                
                //consWrite("CART type: ");


            }catch(Exception x){
                
                consWriteLine(x.Message);
            }
            Device.disconnect();
        }

        void consWrite(string str)
        {
            consoleBox.AppendText(str);
        }


        void consWriteLine(string str)
        {
            consoleBox.AppendText(str + "\r\n");
        }

        

        private void btn_rd_ram_Click(object sender, EventArgs e)
        {

            try
            {

                int ram_size;
                byte[] ram;
                Device.connect();
                Device.setDelay(1);
                string rom_name = Cart.getRomName();
                rom_name += ".srm";
                saveFileDialog1.FileName = rom_name;
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    consWriteLine("-----------------------------------------------------");
                    ram_size = Cart.getRamSize();
                    if (ram_size == 0) throw new Exception("RAM is not detected");
                    consWriteLine("Read RAM to " + saveFileDialog1.FileName);
                    if (ram_size < 1024)
                    {
                        consWriteLine("RAM size : " + ram_size + "B");
                    }
                    else
                    {
                        consWriteLine("RAM size : " + ram_size / 1024 + "K");
                    }
                    Device.writeWord(0xA13000, 0xffff);
                    Device.setAddr(0x200000);
                    ram = new byte[ram_size * 2];
                    Device.read(ram, 0, ram.Length);

                    FileStream f = File.OpenWrite(saveFileDialog1.FileName);
                    f.Write(ram, 0, ram.Length);
                    f.Close();
                    printMD5(ram);
                    consWriteLine("OK");
                }

            }
            catch (Exception x)
            {
                consWriteLine(x.Message);
            }
            Device.disconnect();
        }

 

        private void btn_rd_rom_Click(object sender, EventArgs e)
        {
            try
            {

                
                byte[] rom;
                int rom_size;
                int block_size = 32768;
                Device.connect();
                Device.setDelay(1);
                string rom_name = Cart.getRomName();
                rom_name += ".bin";
                saveFileDialog1.FileName = rom_name;
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    consWriteLine("-----------------------------------------------------");
                    rom_size = Cart.getRomSize();
                    progressBar1.Value = 0;
                    progressBar1.Maximum = rom_size;
                    rom = new byte[rom_size];
                    
                    consWriteLine("Read ROM to " + saveFileDialog1.FileName);
                    consWriteLine("ROM size : " + rom_size / 1024 + "K");

                    Device.writeWord(0xA13000, 0x0000);
                    Device.setAddr(0);
                    DateTime t = DateTime.Now;
                    for (int i = 0; i < rom_size; i += block_size)
                    {
                        Device.read(rom, i, block_size);
                        progressBar1.Value = i;
                        this.Update();
                    }
                    progressBar1.Value = rom_size;
                    int time = (int)(DateTime.Now.Ticks - t.Ticks);
                   // consWriteLine("Time: " + time / 10000);

                    FileStream f = File.OpenWrite(saveFileDialog1.FileName);
                    f.Write(rom, 0, rom.Length);
                    f.Close();

                    printMD5(rom);

                    consWriteLine("OK");
                }

            }
            catch (Exception x)
            {
                consWriteLine(x.Message);
            }
            Device.disconnect();
        }

        private void printMD5(byte []buff)
        {
            MD5 hash = MD5.Create();
            byte[] hash_data = hash.ComputeHash(buff);
            consWriteLine("MD5: " + BitConverter.ToString(hash_data));
        }
        

        private void btn_wr_ram_Click(object sender, EventArgs e)
        {
            try
            {

                int ram_size;
                int copy_len;
                byte[] ram;
                Device.connect();
                Device.setDelay(1);
                
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    consWriteLine("-----------------------------------------------------");
                    consWriteLine("Write RAM...");
                    this.Update();
                    FileStream f = File.OpenRead(openFileDialog1.FileName);
                    ram = new byte[f.Length];
                    f.Read(ram, 0, ram.Length);
                    f.Close();

                    ram_size = Cart.getRamSize();
                    if (ram_size == 0) throw new Exception("RAM is not detected");
                    this.Update();

                    ram_size *= 2;
                    copy_len = ram.Length;
                    if (ram_size < copy_len) copy_len = ram_size;
                    if (copy_len % 2 != 0) copy_len--;
                    Device.writeWord(0xA13000, 0xffff);
                    Device.setAddr(0x200000);
                    Device.write(ram, 0, copy_len);
                    consWriteLine("Verify...");
                    this.Update();
                    byte[] ram2 = new byte[copy_len];
                    Device.setAddr(0x200000);
                    Device.read(ram2, 0, copy_len);
                    for (int i = 0; i < copy_len; i++)
                    {
                        if (i % 2 == 0) continue;
                        if (ram[i] != ram2[i]) throw new Exception("Verify error at " + i);
                    }

                    copy_len /= 2;
                    consWriteLine("" + copy_len+ " words sent");

                    printMD5(ram);
                    consWriteLine("OK");
                }

            }
            catch (Exception x)
            {
                consWriteLine(x.Message);
            }
            Device.disconnect();
        }


        private void btn_wr_rom_Click(object sender, EventArgs e)
        {

            

            try
            {
                byte[] rom;
                int rom_size;
                int block_len = 4096;
                Device.connect();
                Device.setDelay(0);
              

                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    consWriteLine("-----------------------------------------------------");
                    FileStream f = File.OpenRead(openFileDialog1.FileName);
                    rom_size = (int)f.Length;
                    if (rom_size % 65536 != 0) rom_size = rom_size / 65536 * 65536 + 65536;
                    if (rom_size > 0x400000) rom_size = 0x400000;
                    rom = new byte[rom_size];
                    f.Read(rom, 0, rom_size);
                    f.Close();

                    progressBar1.Value = 0;
                    progressBar1.Maximum = rom_size;
                    consWriteLine("Flash erase...");
                    Device.flashResetByPass();

                    
                    for (int i = 0; i < rom_size; i += 65536)
                    {
                        Device.flashErase(i);
                        progressBar1.Value = i;
                        this.Update();
                    }

                    progressBar1.Value = 0;

                    consWriteLine("Flash write...");

                    Device.flashUnlockBypass();
                    Device.setAddr(0);
                    DateTime t = DateTime.Now;
                    for (int i = 0; i < rom_size; i += block_len)
                    {
                        Device.flashWrite(rom, i, block_len);
                        progressBar1.Value = i;
                        this.Update();
                    }
                    int time = (int)(DateTime.Now.Ticks - t.Ticks);
                    //consWriteLine("Time: " + time / 10000);
                    progressBar1.Value = 0;
                    Device.flashResetByPass();

                    consWriteLine("Flash verify...");
                    byte[] rom2 = new byte[rom.Length];

                    Device.setAddr(0);
                    for (int i = 0; i < rom_size; i += block_len)
                    {
                        Device.read(rom2, i, block_len);
                        progressBar1.Value = i;
                        this.Update();
                    }
                    progressBar1.Value = rom_size;
                    for (int i = 0; i < rom_size; i++)
                    {
                        if (rom[i] != rom2[i]) throw new Exception("Verify error at " + i);
                    }

                    consWriteLine("OK");
                }
            }
            catch (Exception x)
            {
                try
                {
                    Device.flashResetByPass();
                }
                catch (Exception) { }
                consWriteLine(x.Message);
            }
            Device.disconnect();
        }


    }
}
