using MahApps.Metro.Controls;
using OurGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Timers;
using System.Collections;
using System.Windows.Controls.Primitives;

namespace PassageManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private LPTSimulation lptSimulation;
        public List<Node> peopleList;
        private Node selectedPerson;
        private Random rand;
        private bool mouseKeyDown;
       

        public MainWindow()
        {
            InitializeComponent();
            lptSimulation = new LPTSimulation(this);
            peopleList = new List<Node>();
            rand = new Random();

            setTheTimer();
        }

        private void setTheTimer()
        {
            //System.Timers.Timer t = new Timer();
            //t.Interval = 5000;
            //t.Elapsed += T_Elapsed;
            
            System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0,0,5000); 
            dispatcherTimer.Start();
        }

        
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            checkLightsSwitchsStatus();

            checkPeopleStatusInTheCanvas();


            //discuss temperature 
            getTheTemperatureSensors();

            //discuss pressure 
            getThePressureSensors();


            lptSimulation.Tick();

            modeTextBlock.Text = "Write";
            modeTextBlock.Foreground = Brushes.Red;

            setTheInputPort();

            setTheDataPort();

            setTheOutputPort();
        }

        //end
        private void checkLightsSwitchsStatus()
        {
            if (!(bool)allLightsToggleButton.IsChecked && (lptSimulation.preInput&0x08)==0x00)//no morning lights
            {

                if ((bool)passage1ToggleButton.IsChecked || (lptSimulation.poepleSensitive & 0x01) == 0x01)
                {
                    section1.Fill = Brushes.Transparent;
                    r1.Fill = Brushes.White;
                    r7.Fill = Brushes.White;
                }
                else
                {
                    section1.Fill = Brushes.Black;
                    r1.Fill = Brushes.Transparent;
                    r7.Fill = Brushes.Transparent;
                }

                if ((bool)passage2ToggleButton.IsChecked || (lptSimulation.poepleSensitive & 0x02) == 0x02)
                {
                    section2.Fill = Brushes.Transparent;
                    r2.Fill = Brushes.White;
                    r8.Fill = Brushes.White;
                }
                else
                {
                    section2.Fill = Brushes.Black;
                    r2.Fill = Brushes.Transparent;
                    r8.Fill = Brushes.Transparent;
                }

                if ((bool)passage3ToggleButton.IsChecked || (lptSimulation.poepleSensitive & 0x04) == 0x04)
                {
                    section3.Fill = Brushes.Transparent;
                    r3.Fill = Brushes.White;
                    r9.Fill = Brushes.White;
                }
                else
                {
                    section3.Fill = Brushes.Black;
                    r3.Fill = Brushes.Transparent;
                    r9.Fill = Brushes.Transparent;
                }

                if ((bool)passage4ToggleButton.IsChecked || (lptSimulation.poepleSensitive & 0x08) == 0x08)
                {
                    section4.Fill = Brushes.Transparent;
                    r4.Fill = Brushes.White;
                    r10.Fill = Brushes.White;
                }
                else
                {
                    section4.Fill = Brushes.Black;
                    r4.Fill = Brushes.Transparent;
                    r10.Fill = Brushes.Transparent;
                }

                if ((bool)passage5ToggleButton.IsChecked || (lptSimulation.poepleSensitive & 0x10) == 0x10)
                {
                    section5.Fill = Brushes.Transparent;
                    r5.Fill = Brushes.White;
                    r11.Fill = Brushes.White;
                }
                else
                {
                    section5.Fill = Brushes.Black;
                    r5.Fill = Brushes.Transparent;
                    r11.Fill = Brushes.Transparent;
                }

                if ((bool)passage6ToggleButton.IsChecked || (lptSimulation.poepleSensitive & 0x20) == 0x20)
                {
                    section6.Fill = Brushes.Transparent;
                    r6.Fill = Brushes.White;
                    r12.Fill = Brushes.White;
                }
                else
                {
                    section6.Fill = Brushes.Black;
                    r6.Fill = Brushes.Transparent;
                    r12.Fill = Brushes.Transparent;
                }

                if((lptSimulation.preOutput&0x08)==0x00)//turn off the lights if the fire happen 
                {

                    r1.Fill = Brushes.Transparent;
                    r2.Fill = Brushes.Transparent;
                    r3.Fill = Brushes.Transparent;
                    r4.Fill = Brushes.Transparent;
                    r5.Fill = Brushes.Transparent;
                    r6.Fill = Brushes.Transparent;
                    r7.Fill = Brushes.Transparent;
                    r8.Fill = Brushes.Transparent;
                    r9.Fill = Brushes.Transparent;
                    r10.Fill = Brushes.Transparent;
                    r11.Fill = Brushes.Transparent;
                    r12.Fill = Brushes.Transparent;
                }

            }
            else if((bool)allLightsToggleButton.IsChecked && (lptSimulation.preOutput & 0x08) == 0x08)//turn on the lights if there is no fire and allLights switch is on
            {
                section1.Fill = Brushes.Transparent;
                section2.Fill = Brushes.Transparent;
                section3.Fill = Brushes.Transparent;
                section4.Fill = Brushes.Transparent;
                section5.Fill = Brushes.Transparent;
                section6.Fill = Brushes.Transparent;

                r1.Fill = Brushes.White;
                r2.Fill = Brushes.White;
                r3.Fill = Brushes.White;
                r4.Fill = Brushes.White;
                r5.Fill = Brushes.White;
                r6.Fill = Brushes.White;
                r7.Fill = Brushes.White;
                r8.Fill = Brushes.White;
                r9.Fill = Brushes.White;
                r10.Fill = Brushes.White;
                r11.Fill = Brushes.White;
                r12.Fill = Brushes.White;
            }

            if ((lptSimulation.preInput & 0x08) == 0x00)// make the passage is lighter without the lamps
            {
                section1.Fill = Brushes.Transparent;
                section2.Fill = Brushes.Transparent;
                section3.Fill = Brushes.Transparent;
                section4.Fill = Brushes.Transparent;
                section5.Fill = Brushes.Transparent;
                section6.Fill = Brushes.Transparent;

            }

        }

        public void setTheDataPort()
        {
            //var z = new BitArray(new int[] { lptSimulation.preData });
            string s = Convert.ToString(lptSimulation.preData, 2).PadLeft(8, '0');

            d0TextBlock.Text = s[7] + "";
            d1TextBlock.Text = s[6] + "";
            d2TextBlock.Text = s[5] + "";
            d3TextBlock.Text = s[4] + "";
            d4TextBlock.Text = s[3] + "";
            d5TextBlock.Text = s[2] + "";
            d6TextBlock.Text = s[1] + "";
            d7TextBlock.Text = s[0] + "";

            if (s[0] == '0')
            {
                d7Rec.Fill = Brushes.Red;
            }
            else
            {
                d7Rec.Fill = Brushes.Green;
            }

            if (s[1] == '0')
            {
                d6Rec.Fill = Brushes.Red;
            }
            else
            {
                d6Rec.Fill = Brushes.Green;
            }

            if (s[2] == '0')
            {
                d5Rec.Fill = Brushes.Red;
            }
            else
            {
                d5Rec.Fill = Brushes.Green;
            }

            if (s[3] == '0')
            {
                d4Rec.Fill = Brushes.Red;
            }
            else
            {
                d4Rec.Fill = Brushes.Green;
            }

            if (s[4] == '0')
            {
                d3Rec.Fill = Brushes.Red;
            }
            else
            {
                d3Rec.Fill = Brushes.Green;
            }

            if (s[5] == '0')
            {
                d2Rec.Fill = Brushes.Red;
            }
            else
            {
                d2Rec.Fill = Brushes.Green;
            }

            if (s[6] == '0')
            {
                d1Rec.Fill = Brushes.Red;
            }
            else
            {
                d1Rec.Fill = Brushes.Green;
            }
            if (s[7] == '0')
            {
                d0Rec.Fill = Brushes.Red;
            }
            else
            {
                d0Rec.Fill = Brushes.Green;
            }


        }

        private void getThePressureSensors()
        {
            if (pressureSlider.Value >= 60 || pressureSlider.Value <=50 ) //
            {
                lptSimulation.preInput |= 0x10;
            }
            else 
            {
                lptSimulation.preInput &= 0xEF;
            }
        }

        private void checkPeopleStatusInTheCanvas()
        {
            if ((lptSimulation.preInput & 0x08) == 0x00)//the morning light
            {

            
            //turn on the light if there is any person
                //for (int i = 0; i < peopleList.Count; i++)
                //{
                //    if (peopleList[i].X >= 0 && peopleList[i].X <= 200)
                //    {
                //        section1.Fill = Brushes.Transparent;
                //        r1.Fill = Brushes.White;
                //        r7.Fill = Brushes.White;
                //    }
                //    if (peopleList[i].X >= 201 && peopleList[i].X <= 425)
                //    {
                //        section2.Fill = Brushes.Transparent;
                //        r2.Fill = Brushes.White;
                //        r8.Fill = Brushes.White;
                //    }
                //    if (peopleList[i].X >= 426 && peopleList[i].X <= 650)
                //    {
                //        section3.Fill = Brushes.Transparent;
                //        r3.Fill = Brushes.White;
                //        r9.Fill = Brushes.White;
                //    }
                //    if (peopleList[i].X >= 651 && peopleList[i].X <= 875)
                //    {
                //        section4.Fill = Brushes.Transparent;
                //        r4.Fill = Brushes.White;
                //        r10.Fill = Brushes.White;
                //    }
                //    if (peopleList[i].X >= 876 && peopleList[i].X <= 1100)
                //    {
                //        section5.Fill = Brushes.Transparent;
                //        r5.Fill = Brushes.White;
                //        r11.Fill = Brushes.White;
                //    }
                //    if (peopleList[i].X >= 1101 && peopleList[i].X <= 1325)
                //    {
                //        section6.Fill = Brushes.Transparent;
                //        r6.Fill = Brushes.White;
                //        r12.Fill = Brushes.White;
                //    }
                //}



            //turn off the light in the empty region
            //var peopleInRegion1 = peopleList.Where(c => c.X >= 0 && c.X <= 200);
            //var peopleInRegion2 = peopleList.Where(c => c.X >= 201 && c.X <= 425);
            //var peopleInRegion3 = peopleList.Where(c => c.X >= 426 && c.X <= 650);
            //var peopleInRegion4 = peopleList.Where(c => c.X >= 651 && c.X <= 875);
            //var peopleInRegion5 = peopleList.Where(c => c.X >= 876 && c.X <= 1100);
            //var peopleInRegion6 = peopleList.Where(c => c.X >= 1101 && c.X <= 1325);

            if (!(bool)allLightsToggleButton.IsChecked)
            {
                if (!(bool)passage1ToggleButton.IsChecked && (lptSimulation.poepleSensitive & 0x01) == 0x00)
                {
                    section1.Fill = Brushes.Black;
                    r1.Fill = Brushes.Transparent;
                    r7.Fill = Brushes.Transparent;
                }

                if (!(bool)passage2ToggleButton.IsChecked && (lptSimulation.poepleSensitive & 0x02) == 0x00)
                {
                    section2.Fill = Brushes.Black;
                    r2.Fill = Brushes.Transparent;
                    r8.Fill = Brushes.Transparent;
                }

                if (!(bool)passage3ToggleButton.IsChecked && (lptSimulation.poepleSensitive & 0x04) == 0x00)
                {
                    section3.Fill = Brushes.Black;
                    r3.Fill = Brushes.Transparent;
                    r9.Fill = Brushes.Transparent;
                }
                if (!(bool)passage4ToggleButton.IsChecked && (lptSimulation.poepleSensitive & 0x08) == 0x00)
                {
                    section4.Fill = Brushes.Black;
                    r4.Fill = Brushes.Transparent;
                    r10.Fill = Brushes.Transparent;
                }

                if (!(bool)passage5ToggleButton.IsChecked && (lptSimulation.poepleSensitive & 0x10) == 0x00)
                {
                    section5.Fill = Brushes.Black;
                    r5.Fill = Brushes.Transparent;
                    r11.Fill = Brushes.Transparent;
                }

                if (!(bool)passage6ToggleButton.IsChecked && (lptSimulation.poepleSensitive & 0x20) == 0x00)
                {
                    section6.Fill = Brushes.Black;
                    r6.Fill = Brushes.Transparent;
                    r12.Fill = Brushes.Transparent;
                }
            }

            if ((lptSimulation.preOutput&0x08)==0x00)//there is fire ...turn of the lights
            {
                section1.Fill = Brushes.Black;
                r1.Fill = Brushes.Transparent;
                r7.Fill = Brushes.Transparent;

                section2.Fill = Brushes.Black;
                r2.Fill = Brushes.Transparent;
                r8.Fill = Brushes.Transparent;

                section3.Fill = Brushes.Black;
                r3.Fill = Brushes.Transparent;
                r9.Fill = Brushes.Transparent;

                section4.Fill = Brushes.Black;
                r4.Fill = Brushes.Transparent;
                r10.Fill = Brushes.Transparent;

                section5.Fill = Brushes.Black;
                r5.Fill = Brushes.Transparent;
                r11.Fill = Brushes.Transparent;

                section6.Fill = Brushes.Black;
                r6.Fill = Brushes.Transparent;
                r12.Fill = Brushes.Transparent;
            }

            }
        }

        private void addPersonButton_Click(object sender, RoutedEventArgs e)
        {
            Node node = new Node();
            node.Width = 100;
            node.Height = 100;
            Canvas.SetLeft(node, rand.Next((int)passageCanvas.ActualWidth - 30));
            Canvas.SetTop(node, rand.Next((int)passageCanvas.ActualHeight - 30));
            node.Y = Canvas.GetTop(node);
            node.X = Canvas.GetLeft(node);
            Panel.SetZIndex(node, 3);
            peopleList.Add(node);
            passageCanvas.Children.Add(node);
        }

        private void getTheTemperatureSensors()
        {
            if (temperatureSlider.Value >= 22 && temperatureSlider.Value <= 28)//moderate
            {
                lptSimulation.preInput &= 0x9F;
            }
            else if (temperatureSlider.Value > -10 && temperatureSlider.Value < 22)//low
            {
                lptSimulation.preInput &= 0xDF;
                lptSimulation.preInput |= 0x40;
            }
            else if (temperatureSlider.Value > 28 && temperatureSlider.Value < 50)//high
            {
                lptSimulation.preInput &= 0xBF;
                lptSimulation.preInput |= 0x20;
            }
            else//upset
            {
                lptSimulation.preInput |= 0x60;
            }
        }

        private void setTheInputPort()
        {
            string s = Convert.ToString(lptSimulation.preInput, 2).PadLeft(8, '0');
            I3TextBlock.Text = s[4] + "";
            I4TextBlock.Text = s[3] + "";
            I5TextBlock.Text = s[2] + "";
            I6TextBlock.Text = s[1] + "";
            if (s[4] == '0')
            {
                I3Rec.Fill = Brushes.Red;
            }
            else
            {
                I3Rec.Fill = Brushes.Green;
            }

            if (s[3] == '0')
            {
                I4Rec.Fill = Brushes.Red;
            }
            else
            {
                I4Rec.Fill = Brushes.Green;
            }

            if (s[2] == '0')
            {
                I5Rec.Fill = Brushes.Red;
            }
            else
            {
                I5Rec.Fill = Brushes.Green;
            }

            if (s[1] == '0')
            {
                I6Rec.Fill = Brushes.Red;
            }
            else
            {
                I6Rec.Fill = Brushes.Green;
            }

            if (s[0] == '0')
            {
                I7Rec.Fill = Brushes.Green;
                I7TextBlock.Text = "1";
            }
            else
            {
                I7Rec.Fill = Brushes.Red;
                I7TextBlock.Text = "0";
            }

        }


        private void canv_DrawBoard_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var z = peopleList.Where(c => c.IsMouseOver).FirstOrDefault();
            if (z != null)
            {
                selectedPerson = z;
                selectedPerson.IsPressed = true;
                mouseKeyDown = true;
            }
         
        }

        private void canv_DrawBoard_MouseUp(object sender, MouseButtonEventArgs e)
        {
            mouseKeyDown = false;
            selectedPerson.IsPressed = false;
            selectedPerson = null;
        }

        private void canv_DrawBoard_MouseMove(object sender, MouseEventArgs e)
        {
            var mousePos = e.MouseDevice.GetPosition(passageCanvas);
            // this if to prevent the vertices from being outside the canvas 
            if (mouseKeyDown &&
               mousePos.X < passageCanvas.ActualWidth - 13 && mousePos.Y < passageCanvas.ActualHeight - 13 //  right and bottom bounds 
             && mousePos.X > passageCanvas.MinWidth + 6 && mousePos.Y > passageCanvas.MinHeight + 6)//  left and top bounds
            {
                Canvas.SetTop(selectedPerson, mousePos.Y - 44);
                Canvas.SetLeft(selectedPerson, mousePos.X - 44);
                selectedPerson.Y = Canvas.GetTop(selectedPerson);
                selectedPerson.X = Canvas.GetLeft(selectedPerson);
            }
        }

        private void passage1ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton b = (ToggleButton)sender;
            if ((bool)b.IsChecked)
            {
                section1.Fill = Brushes.Transparent;
                r1.Fill = Brushes.White;
                r7.Fill = Brushes.White;
            }
            else
            {
                section1.Fill = Brushes.Black;
                r1.Fill = Brushes.Transparent;
                r7.Fill = Brushes.Transparent;
            }
        }

        private void passage2ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton b = (ToggleButton)sender;
            if ((bool)b.IsChecked)
            {
                section2.Fill = Brushes.Transparent;
                r2.Fill = Brushes.White;
                r8.Fill = Brushes.White;
            }
            else
            {
                section2.Fill = Brushes.Black;
                r2.Fill = Brushes.Transparent;
                r8.Fill = Brushes.Transparent;
            }
        }

        private void passage3ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton b = (ToggleButton)sender;
            if ((bool)b.IsChecked)
            {
                section3.Fill = Brushes.Transparent;
                r3.Fill = Brushes.White;
                r9.Fill = Brushes.White;
            }
            else
            {
                section3.Fill = Brushes.Black;
                r3.Fill = Brushes.Transparent;
                r9.Fill = Brushes.Transparent;
            }
        }

        private void passage4ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton b = (ToggleButton)sender;
            if ((bool)b.IsChecked)
            {
                section4.Fill = Brushes.Transparent;
                r4.Fill = Brushes.White;
                r10.Fill = Brushes.White;
            }
            else
            {
                section4.Fill = Brushes.Black;
                r4.Fill = Brushes.Transparent;
                r10.Fill = Brushes.Transparent;
            }
        }

        private void passage5ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton b = (ToggleButton)sender;
            if ((bool)b.IsChecked)
            {
                section5.Fill = Brushes.Transparent;
                r5.Fill = Brushes.White;
                r11.Fill = Brushes.White;
            }
            else
            {
                section5.Fill = Brushes.Black;
                r5.Fill = Brushes.Transparent;
                r11.Fill = Brushes.Transparent;
            }
        }

        private void passage6ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton b = (ToggleButton)sender;
            if ((bool)b.IsChecked)
            {
                section6.Fill = Brushes.Transparent;
                r6.Fill = Brushes.White;
                r12.Fill = Brushes.White;
            }
            else
            {
                section6.Fill = Brushes.Black;
                r6.Fill = Brushes.Transparent;
                r12.Fill = Brushes.Transparent;
            }
        }

        private void morningLightToggleButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton b = (ToggleButton)sender;
            if ((bool)b.IsChecked)
            {
                lptSimulation.preInput |= 0x08;
                section1.Fill = Brushes.Transparent;
                section2.Fill = Brushes.Transparent;
                section3.Fill = Brushes.Transparent;
                section4.Fill = Brushes.Transparent;
                section5.Fill = Brushes.Transparent;
                section6.Fill = Brushes.Transparent;

                r1.Fill = Brushes.Transparent;
                r2.Fill = Brushes.Transparent;
                r3.Fill = Brushes.Transparent;
                r4.Fill = Brushes.Transparent;
                r5.Fill = Brushes.Transparent;
                r6.Fill = Brushes.Transparent;
                r7.Fill = Brushes.Transparent;
                r8.Fill = Brushes.Transparent;
                r9.Fill = Brushes.Transparent;
                r10.Fill = Brushes.Transparent;
                r11.Fill = Brushes.Transparent;
                r12.Fill = Brushes.Transparent;
            }
            else
            {
                lptSimulation.preInput &= 0xF7;
                section1.Fill = Brushes.Black;
                section2.Fill = Brushes.Black;
                section3.Fill = Brushes.Black;
                section4.Fill = Brushes.Black;
                section5.Fill = Brushes.Black;
                section6.Fill = Brushes.Black;

               
            }
        }

        private void fireToggleButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton b = (ToggleButton)sender;
            if ((bool)b.IsChecked)
            {
                lptSimulation.preInput &= 0x7F;
            }
            else
            {
                lptSimulation.preInput |= 0x80;
            }
        }

        private void setTheOutputPort()
        {
            string s = Convert.ToString(lptSimulation.preOutput, 2).PadLeft(8, '0');
            o2TextBlock.Text = s[5] + "";
            
            if (s[7] == '1')
            {
                o0Rec.Fill = Brushes.Red;
                o0TextBlock.Text = "0";
            }
            else
            {
                o0Rec.Fill = Brushes.Green;
                o0TextBlock.Text = "1";
            }

            if (s[6] == '1')
            {
                o1Rec.Fill = Brushes.Red;
                o1TextBlock.Text = "0";
            }
            else
            {
                o1Rec.Fill = Brushes.Green;
                o1TextBlock.Text = "1";
            }

            if (s[5] == '0')
            {
                o2Rec.Fill = Brushes.Red;
            }
            else
            {
                o2Rec.Fill = Brushes.Green;
            }

            if (s[4] == '1')
            {
                o3Rec.Fill = Brushes.Red;
                o3TextBlock.Text = "0";
            }
            else
            {
                o3Rec.Fill = Brushes.Green;
                o3TextBlock.Text = "1";
            }

           

        }

        private void passageCanvas_KeyDown(object sender, KeyEventArgs e)
        {
            bool isDeleteKeyPressed = Keyboard.IsKeyDown(Key.Delete);
            if (isDeleteKeyPressed)
            {
                passageCanvas.Children.Remove(selectedPerson);
                peopleList.Remove(selectedPerson);
            }
        }

        private void allLightsToggleButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton b = (ToggleButton)sender;
            if ((bool)b.IsChecked)
            {
                section1.Fill = Brushes.Transparent;
                section2.Fill = Brushes.Transparent;
                section3.Fill = Brushes.Transparent;
                section4.Fill = Brushes.Transparent;
                section5.Fill = Brushes.Transparent;
                section6.Fill = Brushes.Transparent;

                r1.Fill = Brushes.White;
                r2.Fill = Brushes.White;
                r3.Fill = Brushes.White;
                r4.Fill = Brushes.White;
                r5.Fill = Brushes.White;
                r6.Fill = Brushes.White;
                r7.Fill = Brushes.White;
                r8.Fill = Brushes.White;
                r9.Fill = Brushes.White;
                r10.Fill = Brushes.White;
                r11.Fill = Brushes.White;
                r12.Fill = Brushes.White;
            }
            else
            {
                section1.Fill = Brushes.Black;
                section2.Fill = Brushes.Black;
                section3.Fill = Brushes.Black;
                section4.Fill = Brushes.Black;
                section5.Fill = Brushes.Black;
                section6.Fill = Brushes.Black;

                r1.Fill = Brushes.Transparent;
                r2.Fill = Brushes.Transparent;
                r3.Fill = Brushes.Transparent;
                r4.Fill = Brushes.Transparent;
                r5.Fill = Brushes.Transparent;
                r6.Fill = Brushes.Transparent;
                r7.Fill = Brushes.Transparent;
                r8.Fill = Brushes.Transparent;
                r9.Fill = Brushes.Transparent;
                r10.Fill = Brushes.Transparent;
                r11.Fill = Brushes.Transparent;
                r12.Fill = Brushes.Transparent;
            }
        }
    }
}
