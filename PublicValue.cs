using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicValue
{
    public class PublicValue1
    {
        public static double num = 0;//静态变量直接用 不用新建类PublicValue1.num
        public static StringBuilder sb = new StringBuilder();
        public static StringBuilder Voltage_Input = new StringBuilder();
        public static StringBuilder Current_Input = new StringBuilder();
        public static StringBuilder Voltage_Output = new StringBuilder();
        public static StringBuilder Current_Output = new StringBuilder();
        public static StringBuilder Voltage_Cap_Input = new StringBuilder();
        public static StringBuilder Current_Cap_Input = new StringBuilder();
        public static StringBuilder Voltage_Cap_Output = new StringBuilder();
        public static StringBuilder power_input = new StringBuilder();
        public static StringBuilder power_cap = new StringBuilder();
        public static StringBuilder power_output = new StringBuilder();

        public static double Voltage_Input_val = new double();
        public static double Current_Input_val = new double();
        public static double Voltage_Output_val = new double();
        public static double Current_Output_val = new double();
        public static double Voltage_Cap_Input_val = new double();
        public static double Current_Cap_Input_val = new double();
        public static double Voltage_Cap_Output_val = new double();
        public static double power_input_val = new double();
        public static double power_cap_val = new double();
        public static double power_output_val = new double();
        
        public static double num_chart = 0;
        public static int chart_flage = 9;
        public static int Usart_power_input_val_sent = 9;
        public static int Usart_sent_flage = 0;

        /*
         * chart_flage 对应标志
       Voltage_Input_val            1
       Current_Input_val            2
       Voltage_Output_val           3
       Current_Output_val           4
       Voltage_Cap_Input_val        5
       Current_Cap_Input_val        6
       Voltage_Cap_Output_val       7
       power_input_val              8
       power_cap_val                9
       power_output_val             10
        */



    }

}
