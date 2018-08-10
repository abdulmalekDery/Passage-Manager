using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;


namespace PassageManager
{
    public class LPTSimulation
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
        private static extern byte Inp32_x64(short PortAddress);

        public Byte preInput { get; set; }
        public Byte preOutput { get; set; }
        public Byte preData { get; set; }
        public bool fireHappen { get; set; }
        public double periodOfFire { get; set; }
        public MainWindow mainWindow { get; set; }
        public Byte poepleSensitive { get; set; }






        public LPTSimulation(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            preData = 0x00;
            preOutput = 0x0B;
            preInput = 0x80;
          //  var x = 0b0000_0010;
            //Out32_x64(0x37A, preOutput);
            //Out32_x64(0x378, preData);
        }
        public async void Tick()
        {
           

            //checkPeopleStatus

            var peopleInRegion1 = mainWindow.peopleList.Where(c => c.X >= 0 && c.X <= 200);
            var peopleInRegion2 = mainWindow.peopleList.Where(c => c.X >= 201 && c.X <= 425);
            var peopleInRegion3 = mainWindow.peopleList.Where(c => c.X >= 426 && c.X <= 650);
            var peopleInRegion4 = mainWindow.peopleList.Where(c => c.X >= 651 && c.X <= 875);
            var peopleInRegion5 = mainWindow.peopleList.Where(c => c.X >= 876 && c.X <= 1100);
            var peopleInRegion6 = mainWindow.peopleList.Where(c => c.X >= 1101 && c.X <= 1325);



            await Task.Run(() => Thread.Sleep(1500));

            //Byte InputsSensor = Inp32(0x379);
            Byte InputsSensor = preInput;

            //Out32_x64(0x37A,preOutput);
            preOutput |= 0x20; //make out5 on for reading the movement sensor

            Byte temp = preData;
            preData ^= preData;
            
           
                if (peopleInRegion1.Count()!=0)
                    preData |= 0x01;//put 1 in d0
                else
                    preData &= 0xFE;//put 0 in d0
                if (peopleInRegion2.Count() != 0)
                    preData |= 0x02;//put 1 in d1
                else
                    preData &= 0xFD;//put 0 in d1
                if (peopleInRegion3.Count() != 0)
                    preData |= 0x04;//put 1 in d2
                else
                    preData &= 0xFB;//put 0 in d2
                if (peopleInRegion4.Count() != 0)
                    preData |= 0x08;//put 1 in d3
                else
                    preData &= 0xF7;//put 0 in d3
                if (peopleInRegion5.Count() != 0)
                    preData |= 0x10;//put 1 in d4
                else
                    preData &= 0xEF;//put 0 in d4
                if (peopleInRegion6.Count() != 0)
                    preData |= 0x20;//put 1 in d5
                else
                    preData &= 0xDF;//put 0 in d5
            


            mainWindow.modeTextBlock.Text = "Read";
            mainWindow.modeTextBlock.Foreground = Brushes.Green;

            mainWindow.setTheDataPort();
            //await Task.Run(()=> Thread.Sleep(1500));


            //Byte poepleSensitive = Inp32_x64(0x378);
            poepleSensitive = preData;

            preData = temp;

            //Out32_x64(0x37A, preOutput);
            preOutput &= 0xDF; //make out5 off


            if ((InputsSensor & 0x80) == 0x00) //I7 is 1 as a pin but 0 in the register//remeber that I7 is negative
            {
                //call fierHappen function and shutdown all lamps and fans
                periodOfFire += 1;

                if (periodOfFire >= 5)
                {
                    //call Extinction
                    needForExtinction();
                }
                else
                {
                    //don't call Extinction
                    noNeedForExtinction();
                }

                fierHappen();
                fireHappen = true;

                if (poepleSensitive != 0x00)//there is people in the passage
                {
                    //call Ambulance because there is poeple in the passage
                    needAmbulance();
                }
                else
                {
                    //there isn't poeple in the passage so no need for Ambulance
                    noNeedAmbulance();
                }
            }
            else //I7 is 0 as a pin but in register 1 
            {
                //call no fierHappen function
                //don't call Extinction
                //there isn't poeple in the passage so no need for Ambulance
                noNeedAmbulance();
                noNeedForExtinction();
                periodOfFire = 0;
                noFierHappen();
                fireHappen = false;
            }



            if (!fireHappen)
            {
                /*
                 * 00 Moderate heat 
                 * 
                 */
                if ((InputsSensor & 0x60) == 0x00)//I6 I5 off off
                {
                    //call ModerateHeat function
                    ModerateHeat();
                }
                else if ((InputsSensor & 0x60) == 0x20) //I6 is off and I5 is on
                {
                    //call heatRise function
                    heatRise();
                }
                else if ((InputsSensor & 0x60) == 0x40)//I6 is on and I5 is off
                {
                    //call heatLow function
                    heatLow();
                }
                else //I6 and I5 is on and on 
                {
                    //call heatUpset function
                    heatUpset();
                }


                if ((InputsSensor & 0x10) == 0x10)//I4 is on
                {
                    //call airUpset
                    airUpset();
                }
                else
                {
                    //call airOk
                    airOk();
                }

                if ((InputsSensor & 0x08) == 0x08)//I3 is on
                {
                    //no need for ony lamp becuase we have the morning light
                    shutdownAllLamp();
                    mainWindow.section1.Fill = Brushes.Transparent;
                    mainWindow.section2.Fill = Brushes.Transparent;
                    mainWindow.section3.Fill = Brushes.Transparent;
                    mainWindow.section4.Fill = Brushes.Transparent;
                    mainWindow.section5.Fill = Brushes.Transparent;
                    mainWindow.section6.Fill = Brushes.Transparent;
                }
                else
                {
                    //we have to test if there is any one and where he is
                    if ((poepleSensitive & 0x01) == 0x01)
                    {
                        //turn lamp 1 on D0 on
                        turnLampOn(0x01);
                        mainWindow.section1.Fill = Brushes.Transparent;
                        mainWindow.r1.Fill = Brushes.White;
                        mainWindow.r7.Fill = Brushes.White;
                    }
                    else if(!((bool)mainWindow.allLightsToggleButton.IsChecked || (bool) mainWindow.passage1ToggleButton.IsChecked))
                    {
                        //turn lamp 1 on D0 off
                        turnLampOff(0x01);
                        mainWindow.section1.Fill = Brushes.Black;
                        mainWindow.r1.Fill = Brushes.Transparent;
                        mainWindow.r7.Fill = Brushes.Transparent;
                        if ((preInput & 0x08) == 0x08)
                        {
                            mainWindow.section1.Fill = Brushes.Transparent;
                        }
                    }
                    if ((poepleSensitive & 0x02) == 0x02)
                    {
                        //turn lamp 2 on D1 on
                        turnLampOn(0x02);
                        mainWindow.section2.Fill = Brushes.Transparent;
                        mainWindow.r2.Fill = Brushes.White;
                        mainWindow.r8.Fill = Brushes.White;
                    }
                    else if (!((bool)mainWindow.allLightsToggleButton.IsChecked || (bool)mainWindow.passage2ToggleButton.IsChecked))
                    {
                        //turn lamp 2 on D1 off
                        turnLampOff(0x02);
                        mainWindow.section2.Fill = Brushes.Black;
                        mainWindow.r2.Fill = Brushes.Transparent;
                        mainWindow.r8.Fill = Brushes.Transparent;
                        if ((preInput & 0x08) == 0x08)
                        {
                            mainWindow.section2.Fill = Brushes.Transparent;
                        }
                    }
                    if ((poepleSensitive & 0x04) == 0x04)
                    {
                        //turn lamp 3 on D2 on
                        turnLampOn(0x04);
                        mainWindow.section3.Fill = Brushes.Transparent;
                        mainWindow.r3.Fill = Brushes.White;
                        mainWindow.r9.Fill = Brushes.White;
                    }
                    else if (!((bool)mainWindow.allLightsToggleButton.IsChecked || (bool)mainWindow.passage3ToggleButton.IsChecked))
                    {
                        //turn lamp 3 on D2 off
                        turnLampOff(0x04);
                        mainWindow.section3.Fill = Brushes.Black;
                        mainWindow.r3.Fill = Brushes.Transparent;
                        mainWindow.r9.Fill = Brushes.Transparent;
                        if ((preInput & 0x08) == 0x08)
                        {
                            mainWindow.section3.Fill = Brushes.Transparent;
                        }
                    }
                    if ((poepleSensitive & 0x08) == 0x08)
                    {
                        //turn lamp 4 on D3 on
                        turnLampOn(0x08);
                        mainWindow.section4.Fill = Brushes.Transparent;
                        mainWindow.r4.Fill = Brushes.White;
                        mainWindow.r10.Fill = Brushes.White;
                    }
                    else if (!((bool)mainWindow.allLightsToggleButton.IsChecked || (bool)mainWindow.passage4ToggleButton.IsChecked))
                    {
                        //turn lamp 4 on D3 off
                        turnLampOff(0x08);
                        mainWindow.section4.Fill = Brushes.Black;
                        mainWindow.r4.Fill = Brushes.Transparent;
                        mainWindow.r10.Fill = Brushes.Transparent;
                        if((preInput&0x08)==0x08)
                        {
                            mainWindow.section4.Fill = Brushes.Transparent;
                        }
                    }
                    if ((poepleSensitive & 0x10) == 0x10)
                    {
                        //turn lamp 5 on D4 on
                        turnLampOn(0x10);
                        mainWindow.section5.Fill = Brushes.Transparent;
                        mainWindow.r5.Fill = Brushes.White;
                        mainWindow.r11.Fill = Brushes.White;
                    }
                    else if (!((bool)mainWindow.allLightsToggleButton.IsChecked || (bool)mainWindow.passage5ToggleButton.IsChecked))
                    {
                        //turn lamp 5 on D4 off
                        turnLampOff(0x10);
                        mainWindow.section5.Fill = Brushes.Black;
                        mainWindow.r5.Fill = Brushes.Transparent;
                        mainWindow.r11.Fill = Brushes.Transparent;
                        if ((preInput & 0x08) == 0x08)
                        {
                            mainWindow.section5.Fill = Brushes.Transparent;
                        }
                    }
                    if ((poepleSensitive & 0x20) == 0x20)
                    {
                        //turn lamp 6 on D5 on
                        turnLampOn(0x20);
                        mainWindow.section6.Fill = Brushes.Transparent;
                        mainWindow.r6.Fill = Brushes.White;
                        mainWindow.r12.Fill = Brushes.White;
                    }
                    else if (!((bool)mainWindow.allLightsToggleButton.IsChecked || (bool)mainWindow.passage6ToggleButton.IsChecked))
                    {
                        //turn lamp 6 on D5 off
                        turnLampOff(0x20);
                        mainWindow.section6.Fill = Brushes.Black;
                        mainWindow.r6.Fill = Brushes.Transparent;
                        mainWindow.r12.Fill = Brushes.Transparent;
                        if ((preInput & 0x08) == 0x08)
                        {
                            mainWindow.section6.Fill = Brushes.Transparent;
                        }
                    }
                }
            }
            else
            {
                //turn off all lamps fans and airDevices
                TurnOffAllLampsFansAndAirDevices();

            }

           // Out32_x64(0x378, preData);
           //Out32_x64(0x37A, preOutput);

        }

       

        private void fierHappen()
        {
            //make out3 on //remeber that o3 is negative
            preOutput &= 0xF7;
        }

        private void noFierHappen()
        {
            // make out3 off
            preOutput |= 0x08;
        }
        private void ModerateHeat()
        {
            //make out0 and out1 off and off
            preOutput |= 0x03;
        }
        private void heatRise()
        {
            //make out0 is on and out1 is off
            preOutput &= 0xFE;//put 0 on O0 reg to get 5v on O0 pin
            preOutput |= 0x02;//put 1 on O1 reg to get 0v on O1 pin
        }
        private void heatLow()
        {
            //make out0 is off and out1 is on
            preOutput |= 0x01;//put 1 on O0 reg to get 0v on O0 pin
            preOutput &= 0xFD;//put 0 on O1 reg to get 5v on O1 pin
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

        private void shutdownAllLamp()
        {
            //make D0 D1 D2 D3 D4 D5 all off
            preData &= 0xC0;
        }

        private void turnLampOn(Byte lampPos)
        {
            //turn on the selected lamp
            preData |= lampPos;
        }

        private void turnLampOff(Byte lampPos)
        {
            //turn off the selected lamp
            preData &= (lampPos ^= 0xFF);
        }

        private void needAmbulance()
        {
            //make d6 on
            preData |= 0x40;
        }

        private void noNeedAmbulance()
        {
            //make d6 off
            preData &= 0xBF;
        }

        private void TurnOffAllLampsFansAndAirDevices()
        {
            //make out0 and out1 off and off
            preOutput |= 0x03;
            //make out2 off
            preOutput &= 0xFB;
            //make D0 D1 D2 D3 D4 D5 all off
            preData &= 0xC0;
        }

        private void needForExtinction()
        {
            //make D7 on
            preData |= 0x80;
        }

        private void noNeedForExtinction()
        {
            //make D7 off
            preData &= 0x7F;
        }
    }
}
