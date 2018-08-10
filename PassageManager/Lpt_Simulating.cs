using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace PassageManager
{
    class LPTSimulating
    {
        [DllImport("inpout32.dll")]
        private static extern UInt32 IsInpOutDriverOpen();
        [DllImport("inpout32.dll")]
        private static extern void Out32(short PortAddress, short Data);
        [DllImport("inpout32.dll")]
        private static extern byte Inp32(short PortAddress);

        [DllImport("inpoutx64.dll", EntryPoint = "IsInpOutDriverOpen")]
        private static extern UInt32 IsInpOutDriverOpen_x64();
        [DllImport("inpoutx64.dll", EntryPoint = "Out32")]
        private static extern void Out32_x64(short PortAddress, short Data);
        [DllImport("inpoutx64.dll", EntryPoint = "Inp32")]
        private static extern char Inp32_x64(short PortAddress);

     //    public Byte preInput { get; set; } no need for it
        public Byte preOutput { get; set; }
        public Byte preData { get; set; }
        public LPTSimulating ()
        {
            preData = 0x00;
            preOutput = 0x0B;
           // preInput = 0x80; no need for it
            Out32(0x37A, preOutput);
            Out32(0x378, preData);
        }
        private void timer_name()
        {
            preData &= 0xCF;
            Out32(0x378,preData);
            Byte normalSensitive = Inp32(0x379);
            preData |= 0x20;
            Out32(0x378, preData);
            Byte peopleSensitive = Inp32(0x379);

            if ((normalSensitive & 0x10)==0x00) //I7 = on
            {
                //call fierHappen function and shutdown all lamps and fans
                fierHappen();
            }
            else 
            {
                //call no fierHappen function
                noFierHappen();
            }


            if ((normalSensitive & 0xA0)== 0x00)//I6 I5 off off
            {
                //call heatSoft function
                heatSoft();
            }
            else if ((normalSensitive & 0xA0) == 0x20) //I6 is off and I5 is on
            {
                //call heatRise function
                heatRise();
            }
            else if ((normalSensitive & 0xA0) == 0x40)//I6 is on and I5 is off
            {
                //call heatLow function
                heatLow();
            }
            else //I6 and I5 is off and off 
            {
                //call heatUpset function
                heatUpset();
            }


            if ((normalSensitive & 0x10) == 0x10)//I4 is on
            {
                //call airUpset
                airUpset();
            }
            else
            {
                //call airOk
                airOk();
            }

        }

        private void fierHappen()
        {
            //make out3 on
            preOutput &= 0xF7;
        }

        private void noFierHappen()
        {
            // make out3 off
            preOutput |= 0x08;
        }
        private void heatSoft()
        {
            //make out0 and out1 off and off
            preOutput |= 0x03;
        }
        private void heatRise()
        {
            //make out0 and out1 on and off
            preOutput |= 0x01;
            preOutput &= 0xFD;
        }
        private void heatLow()
        {
            //make out0 and out1 off and on
            preOutput |= 0x02;
            preOutput &= 0xFE;
        }
        private void heatUpset()
        {
            //make out0 and out1 on and on
            preOutput &= 0xFC;
        }
        private void airUpset()
        {
            //make out2 on
            preOutput |= 0x04;
        }
        private void airOk()
        {
            //make out2 off
            preOutput &= 0xFB;
        }
    }
}
