using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using System.Threading;

namespace Conexao_WFA_Arduino
{
    public partial class Form1 : Form
    {
        SerialPort portaSerial = new SerialPort();

        public Form1()
        {
            InitializeComponent();
        }

        private void btnConectar_Click(object sender, EventArgs e)
        {
            //Estrutura condicional para verficar a conexão
            //Criada uma função para verificar se na porta selecionada
            //existe um Arduino
            if (checarArduino()) 
            {
                //Sendo verdadeira a condição
                picBox.Image = Properties.Resources.imgOK; //Altera a imagem da conexao
                btnConectar.Enabled = false;  //Desabilita botão CONECTAR
                btnDesconectar.Enabled = true;  //Habilita botão DESCONECTAR
                cbPortas.Enabled = false;  //Desabilita botão comboBox das Portas
                checkLed.Enabled = true;  //Habilita o checkBox do controle do LED
            }
            else
            {
                //Sendo falsa a condição
                picBox.Image = Properties.Resources.ImgErro; //Altera a imagem da conexao
                btnDesconectar.Enabled = false;  //Desabilita botão DESCONECTAR
            }
        }

        private void btnDesconectar_Click(object sender, EventArgs e)
        {
            btnConectar.Enabled = true;  //Habilita o botão CONECTAR
            btnDesconectar.Enabled = false;  //Desabilita o botão DESCONECTAR
            picBox.Image = Properties.Resources.ImgErro; //Altera a imagem da conexão
            cbPortas.SelectedIndex = -1;  //
            cbPortas.Enabled = true;
            checkLed.Checked = false;
            checkLed.Enabled = false;
            portaSerial.Write("D");
            portaSerial.Close();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //laço de repetição responsável por buscar as portas COM
            // e carregar o ComboBox (cbPortas)
            foreach (string porta in SerialPort.GetPortNames())
            {
                cbPortas.Items.Add(porta);
            }

            //Propriedade para desabilitar o botão
            btnDesconectar.Enabled = false;
        }

        private bool checarArduino()
        {
            try
            {
                //Caso algum erro inesperado ocorra neste bloco, as instruções do catch são executadas
                portaSerial.PortName = cbPortas.SelectedItem.ToString(); //atribuição da porta selecionada
                portaSerial.BaudRate = 9600;  //definição de da taxa de transmissão
                portaSerial.Open();  //Abertura da conexão com a porta serial
                portaSerial.Write("TESTE");  //Envia uma string para o Arduino

                Thread.Sleep(1500);  //Tempo em milessegundos para aguardar o Arduino receber, processar
                                     //e devolver a string esperada
                string msgRecebida = portaSerial.ReadExisting(); //armezenando a resposta do Arduino

                //Condição para verificar se a string enviada pelo arduino é a esperada
                if (msgRecebida.Equals("TESTE"))
                {
                    //Sendo a condição verdadeira, retorna true
                    return true;
                }
                else
                {
                    //Sendo a condição falsa
                    //Verifica se a conexão com a porta serial está aberta
                    if (portaSerial.IsOpen) 
                        portaSerial.Close(); //sendo a condição verdadeira, fecha a conexão
                    
                    return false; //retorna false
                }
            }
            catch(Exception e)
            {
                //Caso ocorra algm erro no bloco do try, uma mensagem com o erro será apresentada
                //E a conexão será fechada, caso esteja aberta
                MessageBox.Show(e.Message.ToString());
                if (portaSerial.IsOpen) 
                    portaSerial.Close();

                return false; //retorna false
            }
        }

        private void checkLed_CheckedChanged(object sender, EventArgs e)
        {
            if (checkLed.Checked)
            {
                picBoxLed.Image = Properties.Resources.LedAceso;
                portaSerial.Write("L");
                checkLed.Text = "Desligar Led";
            }
            else
            {
                picBoxLed.Image = Properties.Resources.LedApagado;
                portaSerial.Write("D");
                checkLed.Text = "Ligar Led";
            }
        }
    }
}
