using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EasyModbus;
using System.IO;

namespace modbus_portal
{
    public partial class Form1 : Form
    {
        private ModbusClient modbusClient;
        public class DestInfo { public int Addr; public string Type; }
        private List<DestInfo> lastMacroDestinations = new List<DestInfo>();

        public class ConstantInfo { public float Value; public bool IsReal; }
        public class AstNode { public string Addr; public string Type; }
        public struct DeviceSettings
        {
            public string IP;
            public int Port;
            public int SlaveID;
            public int ScanTime;
            public bool IsActive;
        }

        private DeviceSettings[] deviceCache = new DeviceSettings[5];
        private string configFilePath = Path.Combine(Application.StartupPath, "gateway_config.txt");

        public Form1()
        {
            InitializeComponent();
            modbusClient = new ModbusClient();

            cmbDeviceIndex.Items.Clear();
            cmbDeviceIndex.Items.AddRange(new object[] { "Cihaz 0 (Ana PLC)", "Cihaz 1", "Cihaz 2", "Cihaz 3", "Cihaz 4" });
            cmbDeviceIndex.SelectedIndex = 0;

            cmbScriptTuru.Items.Clear();
            cmbScriptTuru.Items.AddRange(new object[] { "Assembly", "C" });
            cmbScriptTuru.SelectedIndex = 0;

            for (int i = 0; i < 5; i++)
            {
                deviceCache[i] = new DeviceSettings { IP = "50.10.8.202", Port = 502, SlaveID = 1, ScanTime = 2, IsActive = false };
            }

            pnlStatusLed.BackColor = System.Drawing.Color.Red;

            cmbDeviceIndex.SelectedIndexChanged += cmbDeviceIndex_SelectedIndexChanged;
        }

        private void Log(string message)
        {
            if (txtLog.InvokeRequired)
            {
                txtLog.Invoke(new Action(() => Log(message)));
                return;
            }
            txtLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}\r\n");
            txtLog.SelectionStart = txtLog.Text.Length;
            txtLog.ScrollToCaret();
        }

        private float WordArrayToFloat(int reg0, int reg1)
        {
            byte[] bytes = new byte[4];
            byte[] reg0Bytes = BitConverter.GetBytes((short)reg0);
            byte[] reg1Bytes = BitConverter.GetBytes((short)reg1);

            bytes[0] = reg0Bytes[0];
            bytes[1] = reg0Bytes[1];
            bytes[2] = reg1Bytes[0];
            bytes[3] = reg1Bytes[1];

            return BitConverter.ToSingle(bytes, 0);
        }

        private int[] FloatToWordArray(float value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            int reg0 = BitConverter.ToInt16(bytes, 0);
            int reg1 = BitConverter.ToInt16(bytes, 2);

            return new int[] { reg0, reg1 };
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                modbusClient.IPAddress = txtIp.Text;
                modbusClient.Port = int.Parse(txtPort.Text);
                modbusClient.Connect();

                Log($">>> Robustel Edge Gateway'e Bağlanıldı ({txtIp.Text}:{txtPort.Text})");
                pnlStatusLed.BackColor = System.Drawing.Color.Lime;
                timer1.Start();
            }
            catch (Exception ex)
            {
                pnlStatusLed.BackColor = System.Drawing.Color.Red;
                Log("Bağlantı Hatası: " + ex.Message);
            }
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            if (modbusClient.Connected)
            {
                modbusClient.Disconnect();
                Log("<<< Gateway Bağlantısı Kesildi.");
            }
            pnlStatusLed.BackColor = System.Drawing.Color.Red;
        }

        private void cmbDeviceIndex_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = cmbDeviceIndex.SelectedIndex;
            txtTargetDeviceIp.Text = deviceCache[index].IP;
            txtTargetDevicePort.Text = deviceCache[index].Port.ToString();
            txtTargetSlaveId.Text = deviceCache[index].SlaveID.ToString();
            txtScanTime.Text = deviceCache[index].ScanTime.ToString();

            Log($"Arayüz Odağı Değişti -> Cihaz {index} profili yüklendi.");
        }

        private void addUpdate_Click(object sender, EventArgs e)
        {
            if (!modbusClient.Connected)
            {
                MessageBox.Show("Lütfen önce Robustel ile bağlantı kurun!");
                return;
            }

            try
            {
                int index = cmbDeviceIndex.SelectedIndex;
                string targetIp = txtTargetDeviceIp.Text.Trim();

                if (!int.TryParse(txtTargetDevicePort.Text, out int targetPort) ||
                    !int.TryParse(txtTargetSlaveId.Text, out int slaveId) ||
                    !int.TryParse(txtScanTime.Text, out int scanTime))
                {
                    MessageBox.Show("Port, Slave ID ve Scan Time alanlarına geçerli sayılar girmelisiniz!");
                    return;
                }

                string[] ipParts = targetIp.Split('.');
                if (ipParts.Length != 4) throw new Exception("IP adresi formatı geçersiz! (Örn: 50.10.8.202)");

                deviceCache[index].IP = targetIp;
                deviceCache[index].Port = targetPort;
                deviceCache[index].SlaveID = slaveId;
                deviceCache[index].ScanTime = scanTime;
                deviceCache[index].IsActive = true;

                int ipHigh = (byte.Parse(ipParts[0]) << 8) | byte.Parse(ipParts[1]);
                int ipLow = (byte.Parse(ipParts[2]) << 8) | byte.Parse(ipParts[3]);

                int[] configPayload = new int[6];
                configPayload[0] = ipHigh;
                configPayload[1] = ipLow;
                configPayload[2] = targetPort;
                configPayload[3] = slaveId;
                configPayload[4] = scanTime;
                configPayload[5] = 1;

                int baseReg = 1001 + (index * 6);
                modbusClient.WriteMultipleRegisters(baseReg, configPayload);

                Log($"ZIRHLI EMİR: Cihaz {index} ayarları ağ geçidine yazıldı ve tarama tetiklendi.");
            }
            catch (Exception ex)
            {
                Log("Cihaz Eklenemedi: " + ex.Message);
            }
        }

        private void btnStopAllScans_Click(object sender, EventArgs e)
        {
            if (!modbusClient.Connected) return;

            try
            {
                pnlStatusLed.BackColor = System.Drawing.Color.Gold;
                modbusClient.WriteSingleRegister(8499, 0);

                for (int i = 0; i < 5; i++)
                {
                    int baseReg = 1001 + (i * 6);
                    modbusClient.WriteSingleRegister(baseReg + 5, 0);
                    deviceCache[i].IsActive = false;
                }
                Log("!!! ACİL DURDURMA TETİKLENDİ: Sahadaki tüm cihazların taraması ve Makrolar askıya alındı.");
                pnlStatusLed.BackColor = System.Drawing.Color.Lime;
            }
            catch (Exception ex)
            {
                Log("Acil Durdurma Hatası: " + ex.Message);
            }
        }

        private void btnApplyInterval_Click(object sender, EventArgs e)
        {
            if (int.TryParse(txtUiInterval.Text, out int newInterval))
            {
                if (newInterval >= 100)
                {
                    timer1.Interval = newInterval;
                    Log($"HIZ GÜNCELLENDİ: Arayüz yenileme sıklığı {newInterval} ms yapıldı.");
                }
                else
                {
                    MessageBox.Show("Emniyet için yenileme sıklığı 100 ms'den küçük olamaz!");
                }
            }
            else
            {
                MessageBox.Show("Lütfen milisaniye cinsinden geçerli bir sayı girin.");
            }
        }

        private void btnSaveConfigLocal_Click(object sender, EventArgs e)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(configFilePath, false))
                {
                    for (int i = 0; i < 5; i++)
                    {
                        sw.WriteLine($"{deviceCache[i].IP};{deviceCache[i].Port};{deviceCache[i].SlaveID};{deviceCache[i].ScanTime};{deviceCache[i].IsActive}");
                    }
                }
                Log($"> YEREL YEDEK ALINDI: Tüm profiller '{configFilePath}' dosyasına kaydedildi.");
            }
            catch (Exception ex)
            {
                Log("Yedekleme Hatası: " + ex.Message);
            }
        }

        private void btnLoadConfigLocal_Click(object sender, EventArgs e)
        {
            if (!File.Exists(configFilePath))
            {
                Log("HATA: Okunacak bir yedek dosyası bulunamadı.");
                return;
            }

            try
            {
                string[] lines = File.ReadAllLines(configFilePath);
                for (int i = 0; i < Math.Min(lines.Length, 5); i++)
                {
                    string[] parts = lines[i].Split(';');
                    if (parts.Length == 5)
                    {
                        deviceCache[i].IP = parts[0];
                        deviceCache[i].Port = int.Parse(parts[1]);
                        deviceCache[i].SlaveID = int.Parse(parts[2]);
                        deviceCache[i].ScanTime = int.Parse(parts[3]);
                        deviceCache[i].IsActive = bool.Parse(parts[4]);
                    }
                }
                Log("< YEDEK YÜKLENDİ: Bilgisayardaki son profiller başarıyla hafızaya alındı.");

                cmbDeviceIndex_SelectedIndexChanged(null, null);
            }
            catch (Exception ex)
            {
                Log("Yedekten Yükleme Hatası: " + ex.Message);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (modbusClient.Connected)
            {
                try
                {
                    int index = cmbDeviceIndex.SelectedIndex;
                    int baseDataRegister = 9000 + (index * 200);

                    int[] readRegs = modbusClient.ReadHoldingRegisters(baseDataRegister, 6);

                    float real1 = WordArrayToFloat(readRegs[0], readRegs[1]);
                    float real2 = WordArrayToFloat(readRegs[2], readRegs[3]);

                    int int1 = (short)readRegs[4];
                    int int2 = (short)readRegs[5];

                    lblRead.Text = $"Seçili Cihaz ({index}) Mevcut Canlı Verileri:\n\n" +
                                   $"Real 1: {real1:F2}\n" +
                                   $"Real 2: {real2:F2}\n" +
                                   $"Int 1: {int1}\n" +
                                   $"Int 2: {int2}";
                }
                catch (Exception ex)
                {
                    Log("Okuma Çevrim Hatası: " + ex.Message);
                }

                try
                {
                    int[] scriptStatus = modbusClient.ReadHoldingRegisters(8500, 1);
                    if (scriptStatus[0] == 2)
                    {
                        int[] stringRegs = modbusClient.ReadHoldingRegisters(8501, 30);
                        string edgeResult = "";

                        foreach (int reg in stringRegs)
                        {
                            if (reg == 0) break;
                            char c1 = (char)(reg >> 8);
                            char c2 = (char)(reg & 0xFF);
                            edgeResult += c1;
                            if (c2 != '\0') edgeResult += c2;
                        }

                        Log($"[SISTEM CEVABI]: {edgeResult}");

                        foreach (var destInfo in lastMacroDestinations)
                        {
                            try
                            {
                                if (destInfo.Type == "REAL")
                                {
                                    int[] val = modbusClient.ReadHoldingRegisters(destInfo.Addr, 2);
                                    float fVal = WordArrayToFloat(val[0], val[1]);
                                    Log($"   => Sonuç: Kayıt [{destInfo.Addr}] = {fVal:F2} (REAL)");
                                }
                                else
                                {
                                    int[] val = modbusClient.ReadHoldingRegisters(destInfo.Addr, 1);
                                    Log($"   => Sonuç: Kayıt [{destInfo.Addr}] = {(short)val[0]} (INT/BOOL/CHAR)");
                                }
                            }
                            catch { }
                        }

                        modbusClient.WriteSingleRegister(8500, 0);
                    }
                    else if (scriptStatus[0] == 3)
                    {
                        Log("[ERROR]: Makro icinde hata veya sonsuz döngü tespit edildi!");
                        modbusClient.WriteSingleRegister(8500, 0);
                    }
                }
                catch { }
            }
        }

        private void btnWrite_Click(object sender, EventArgs e)
        {
            if (!modbusClient.Connected) return;

            try
            {
                int index = cmbDeviceIndex.SelectedIndex;
                int baseWriteRegister = 9000 + (index * 200) + 100;

                float wReal1 = float.Parse(txtWriteReal1.Text);
                float wReal2 = float.Parse(txtWriteReal2.Text);
                int wInt1 = int.Parse(txtWriteInt1.Text);
                int wInt2 = int.Parse(txtWriteInt2.Text);

                int[] float1Words = FloatToWordArray(wReal1);
                int[] float2Words = FloatToWordArray(wReal2);

                int[] payload = new int[6];

                payload[0] = float1Words[0];
                payload[1] = float1Words[1];
                payload[2] = float2Words[0];
                payload[3] = float2Words[1];

                payload[4] = wInt1;
                payload[5] = wInt2;

                modbusClient.WriteMultipleRegisters(baseWriteRegister, payload);
                Log($"YAZMA BAŞARILI: Cihaz {index} için kontrol emirleri Robustel'e iletildi.");
            }
            catch (Exception ex)
            {
                Log("Yazma Hatası: " + ex.Message);
            }
        }

        private string GetAddress(string varName)
        {
            if (varName.ToUpper().StartsWith("FREG_")) return varName.Substring(5);
            if (varName.ToUpper().StartsWith("REG_")) return varName.Substring(4);

            if (FormDegisken.VariableMap.ContainsKey(varName))
                return FormDegisken.VariableMap[varName].Address;

            if (int.TryParse(varName, out _)) return varName;

            throw new Exception($"Tanımsız değişken: '{varName}'. Lütfen 'Değişken Ata' menüsünden tanımlayın.");
        }

        private class ControlBlock
        {
            public string Type { get; set; }
            public int StartIP { get; set; }
            public int FixupIP { get; set; }
            public string ForIncrement { get; set; }

            public List<int> BreakFixups { get; set; } = new List<int>();
            public List<int> ContinueFixups { get; set; } = new List<int>();
        }

        private string TranspileToAssembly(string rawScript, string lang, out Dictionary<int, ConstantInfo> preWriteConstants)
        {
            preWriteConstants = new Dictionary<int, ConstantInfo>();
            if (lang == "Assembly") return rawScript;

            int tempRegStart = 8600;
            List<string> resultAsm = new List<string>();
            string[] lines = rawScript.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            Stack<ControlBlock> blocks = new Stack<ControlBlock>();
            Stack<string> switchVarStack = new Stack<string>();
            int currentCaseFailFixup = -1;
            int lastClosedIfJmpIdx = -1;
            int currentInst = 0;

            Dictionary<string, int> functionMap = new Dictionary<string, int>();
            Dictionary<string, List<int>> functionFixups = new Dictionary<string, List<int>>();

            Dictionary<string, int> labelMap = new Dictionary<string, int>();
            Dictionary<string, List<int>> labelFixups = new Dictionary<string, List<int>>();

            for (int i = 0; i < lines.Length; i++)
            {
                string l = lines[i].Trim();
                if (string.IsNullOrEmpty(l) || l.StartsWith("//") || l == "{") continue;

                if (l.EndsWith(":"))
                {
                    string labelName = l.Substring(0, l.Length - 1).Trim();
                    labelMap[labelName] = currentInst;
                    if (labelFixups.ContainsKey(labelName))
                    {
                        foreach (int fixIp in labelFixups[labelName])
                        {
                            resultAsm[fixIp] = resultAsm[fixIp].Replace("???", currentInst.ToString());
                        }
                        labelFixups.Remove(labelName);
                    }
                    continue;
                }

                var errMatch = System.Text.RegularExpressions.Regex.Match(l, @"^on_error_goto\s*\(\s*([a-zA-Z0-9_]+)\s*\)\s*;$");
                if (errMatch.Success)
                {
                    string labelName = errMatch.Groups[1].Value;
                    if (labelMap.ContainsKey(labelName))
                    {
                        resultAsm.Add($"ON_ERROR_JMP 0 0 {labelMap[labelName]}");
                    }
                    else
                    {
                        resultAsm.Add($"ON_ERROR_JMP 0 0 ???");
                        if (!labelFixups.ContainsKey(labelName)) labelFixups[labelName] = new List<int>();
                        labelFixups[labelName].Add(currentInst);
                    }
                    currentInst++;
                    continue;
                }

                var gotoMatch = System.Text.RegularExpressions.Regex.Match(l, @"^goto\s+([a-zA-Z0-9_]+)\s*;$");
                if (gotoMatch.Success)
                {
                    string labelName = gotoMatch.Groups[1].Value;
                    if (labelMap.ContainsKey(labelName))
                    {
                        resultAsm.Add($"JMP 0 0 {labelMap[labelName]}");
                    }
                    else
                    {
                        resultAsm.Add($"JMP 0 0 ???");
                        if (!labelFixups.ContainsKey(labelName)) labelFixups[labelName] = new List<int>();
                        labelFixups[labelName].Add(currentInst);
                    }
                    currentInst++;
                    continue;
                }

                var funcMatch = System.Text.RegularExpressions.Regex.Match(l, @"^void\s+([a-zA-Z0-9_]+)\s*\(\)\s*\{?$");
                if (funcMatch.Success)
                {
                    string funcName = funcMatch.Groups[1].Value;
                    int skipJmpIp = currentInst;
                    resultAsm.Add($"JMP 0 0 0");
                    currentInst++;

                    blocks.Push(new ControlBlock { Type = "FUNC", StartIP = currentInst, FixupIP = skipJmpIp });
                    functionMap[funcName] = currentInst;

                    if (functionFixups.ContainsKey(funcName))
                    {
                        foreach (int fixIp in functionFixups[funcName])
                        {
                            resultAsm[fixIp] = $"CALL 0 0 {currentInst}";
                        }
                        functionFixups.Remove(funcName);
                    }
                    continue;
                }

                var threadMatch = System.Text.RegularExpressions.Regex.Match(l, @"^thread\s*\{?$");
                if (threadMatch.Success)
                {
                    int threadStartCmdIp = currentInst;
                    resultAsm.Add($"THREAD_START 0 0 ???");
                    currentInst++;

                    int jmpCmdIp = currentInst;
                    resultAsm.Add($"JMP 0 0 ???");
                    currentInst++;

                    int actualThreadStart = currentInst;
                    resultAsm[threadStartCmdIp] = $"THREAD_START 0 0 {actualThreadStart}";

                    blocks.Push(new ControlBlock { Type = "THREAD", StartIP = actualThreadStart, FixupIP = jmpCmdIp });
                    continue;
                }

                // ============================================================
                // YENİ EKLENEN: MODBUS MASTER (OKUMA VE YAZMA YÖNLENDİRMELERİ)
                // ============================================================

                // Modbus Write (Yazma) Formatı: modbus_write(cihaz_id, adres, deger);
                var mwMatch = System.Text.RegularExpressions.Regex.Match(l, @"^modbus_write\s*\(\s*(.+?)\s*,\s*(.+?)\s*,\s*(.+?)\s*\)\s*;$");
                if (mwMatch.Success)
                {
                    string arg1 = CompileExpression(mwMatch.Groups[1].Value, ref tempRegStart, preWriteConstants, resultAsm, ref currentInst).Replace("R_", "");
                    string arg2 = CompileExpression(mwMatch.Groups[2].Value, ref tempRegStart, preWriteConstants, resultAsm, ref currentInst).Replace("R_", "");
                    string arg3 = CompileExpression(mwMatch.Groups[3].Value, ref tempRegStart, preWriteConstants, resultAsm, ref currentInst).Replace("R_", "");
                    resultAsm.Add($"MODBUS_WRITE {arg1} {arg2} {arg3}");
                    currentInst++;
                    continue;
                }

                // Modbus Read (Okuma) Formatı: degisken = modbus_read(cihaz_id, adres);
                var mrMatch = System.Text.RegularExpressions.Regex.Match(l, @"^([a-zA-Z0-9_]+)\s*=\s*modbus_read\s*\(\s*(.+?)\s*,\s*(.+?)\s*\)\s*;$");
                if (mrMatch.Success)
                {
                    string destVar = mrMatch.Groups[1].Value;
                    string arg1 = CompileExpression(mrMatch.Groups[2].Value, ref tempRegStart, preWriteConstants, resultAsm, ref currentInst).Replace("R_", "");
                    string arg2 = CompileExpression(mrMatch.Groups[3].Value, ref tempRegStart, preWriteConstants, resultAsm, ref currentInst).Replace("R_", "");
                    string destAddr = GetAddress(destVar);
                    resultAsm.Add($"MODBUS_READ {arg1} {arg2} {destAddr}");
                    currentInst++;
                    continue;
                }

                // ============================================================

                var callMatch = System.Text.RegularExpressions.Regex.Match(l, @"^([a-zA-Z0-9_]+)\s*\(\)\s*;$");
                if (callMatch.Success && callMatch.Groups[1].Value != "delay" && callMatch.Groups[1].Value != "on_error_goto")
                {
                    string funcName = callMatch.Groups[1].Value;
                    if (functionMap.ContainsKey(funcName))
                    {
                        resultAsm.Add($"CALL 0 0 {functionMap[funcName]}");
                    }
                    else
                    {
                        resultAsm.Add($"CALL 0 0 0");
                        if (!functionFixups.ContainsKey(funcName))
                            functionFixups[funcName] = new List<int>();
                        functionFixups[funcName].Add(currentInst);
                    }
                    currentInst++;
                    continue;
                }

                var pushMatch = System.Text.RegularExpressions.Regex.Match(l, @"^push\s*\(\s*([a-zA-Z0-9_]+)\s*\)\s*;$");
                if (pushMatch.Success)
                {
                    string varName = pushMatch.Groups[1].Value;
                    string addr = GetAddress(varName);
                    resultAsm.Add($"PUSH {addr} 0 0");
                    currentInst++;
                    continue;
                }
                var popMatch = System.Text.RegularExpressions.Regex.Match(l, @"^pop\s*\(\s*([a-zA-Z0-9_]+)\s*\)\s*;$");
                if (popMatch.Success)
                {
                    string varName = popMatch.Groups[1].Value;
                    string addr = GetAddress(varName);
                    resultAsm.Add($"POP 0 0 {addr}");
                    currentInst++;
                    continue;
                }

                if (l.StartsWith("while"))
                {
                    int startP = l.IndexOf('(');
                    int endP = l.LastIndexOf(')');
                    if (startP != -1 && endP != -1)
                    {
                        int loopStart = currentInst;
                        string condition = l.Substring(startP + 1, endP - startP - 1);
                        string condDest = CompileExpression(condition, ref tempRegStart, preWriteConstants, resultAsm, ref currentInst);
                        condDest = condDest.Replace("R_", "");

                        resultAsm.Add($"JMPF {condDest} 0 0");
                        blocks.Push(new ControlBlock { Type = "WHILE", StartIP = loopStart, FixupIP = currentInst });
                        currentInst++;
                    }
                    continue;
                }

                if (l.StartsWith("if"))
                {
                    int startP = l.IndexOf('(');
                    int endP = l.LastIndexOf(')');
                    if (startP != -1 && endP != -1)
                    {
                        string condition = l.Substring(startP + 1, endP - startP - 1);
                        string condDest = CompileExpression(condition, ref tempRegStart, preWriteConstants, resultAsm, ref currentInst);
                        condDest = condDest.Replace("R_", "");

                        resultAsm.Add($"JMPF {condDest} 0 0");
                        blocks.Push(new ControlBlock { Type = "IF", FixupIP = currentInst });
                        currentInst++;
                    }
                    continue;
                }

                bool isElse = l.StartsWith("else") || l.StartsWith("} else") || l.StartsWith("}else");
                if (isElse)
                {
                    int lastIfJump = -1;
                    if (l.StartsWith("}"))
                    {
                        if (blocks.Count > 0 && blocks.Peek().Type == "IF") lastIfJump = blocks.Pop().FixupIP;
                    }
                    else
                    {
                        lastIfJump = lastClosedIfJmpIdx;
                    }

                    if (lastIfJump != -1)
                    {
                        resultAsm.Add("JMP 0 0 0");
                        int endIfJump = currentInst;
                        currentInst++;

                        string[] oldInst = resultAsm[lastIfJump].Split(' ');
                        resultAsm[lastIfJump] = $"JMPF {oldInst[1]} 0 {currentInst}";

                        blocks.Push(new ControlBlock { Type = "ELSE", FixupIP = endIfJump });
                    }
                    continue;
                }

                if (l.StartsWith("for"))
                {
                    int startP = l.IndexOf('(');
                    int endP = l.LastIndexOf(')');
                    if (startP != -1 && endP != -1)
                    {
                        string[] forParts = l.Substring(startP + 1, endP - startP - 1).Split(';');
                        if (forParts.Length == 3)
                        {
                            string initPart = forParts[0].Trim();
                            string condPart = forParts[1].Trim();
                            string incPart = forParts[2].Trim();

                            if (initPart.Contains("="))
                            {
                                int eqIdx = initPart.IndexOf('=');
                                string destVar = initPart.Substring(0, eqIdx).Trim();
                                string expr = initPart.Substring(eqIdx + 1).Trim().Replace(";", "");

                                var initMatches = System.Text.RegularExpressions.Regex.Matches(expr, @"([a-zA-Z0-9_]+)\[([a-zA-Z0-9_]+)\]");
                                foreach (System.Text.RegularExpressions.Match match in initMatches)
                                {
                                    string arrName = match.Groups[1].Value;
                                    string offsetName = match.Groups[2].Value;

                                    int baseAddrVal = int.Parse(GetAddress(arrName));
                                    int baseReg = tempRegStart++;
                                    preWriteConstants[baseReg] = new ConstantInfo { Value = baseAddrVal, IsReal = false };

                                    string offsetReg;
                                    if (int.TryParse(offsetName, out int offsetVal))
                                    {
                                        int offR = tempRegStart++;
                                        preWriteConstants[offR] = new ConstantInfo { Value = offsetVal, IsReal = false };
                                        offsetReg = offR.ToString();
                                    }
                                    else
                                    {
                                        offsetReg = GetAddress(offsetName);
                                    }

                                    int valReg = tempRegStart++;
                                    string arrType = FormDegisken.VariableMap.ContainsKey(arrName) ? FormDegisken.VariableMap[arrName].Type : "INT";

                                    string opc = "LOAD_ARR";
                                    if (arrType == "REAL") opc = "LOAD_ARR_REAL";
                                    else if (arrType == "STRING") opc = "LOAD_CHAR";

                                    resultAsm.Add($"{opc} {baseReg} {offsetReg} {valReg}");
                                    currentInst++;

                                    string replacement = arrType == "REAL" ? ("FREG_" + valReg.ToString()) : ("REG_" + valReg.ToString());
                                    expr = expr.Replace(match.Value, replacement);
                                }

                                string destAddr = GetAddress(destVar);
                                string destType = FormDegisken.VariableMap.ContainsKey(destVar) ? FormDegisken.VariableMap[destVar].Type : "INT";
                                string finalNodeAddr = CompileExpression(expr, ref tempRegStart, preWriteConstants, resultAsm, ref currentInst);
                                string finalType = (finalNodeAddr.StartsWith("R_") || destType == "REAL") ? "REAL" : "INT";
                                finalNodeAddr = finalNodeAddr.Replace("R_", "");

                                if (finalType == "INT" && destType == "REAL") { resultAsm.Add($"ITOF {finalNodeAddr} {destAddr}"); currentInst++; }
                                else if (finalType == "REAL" && destType == "INT") { resultAsm.Add($"FTOI {finalNodeAddr} {destAddr}"); currentInst++; }
                                else { resultAsm.Add($"{(destType == "REAL" ? "FMOV" : "MOV")} {finalNodeAddr} {destAddr}"); currentInst++; }
                            }

                            int loopStart = currentInst;
                            string condDest = CompileExpression(condPart, ref tempRegStart, preWriteConstants, resultAsm, ref currentInst);
                            condDest = condDest.Replace("R_", "");

                            resultAsm.Add($"JMPF {condDest} 0 0");
                            blocks.Push(new ControlBlock { Type = "FOR", StartIP = loopStart, FixupIP = currentInst, ForIncrement = incPart });
                            currentInst++;
                        }
                    }
                    continue;
                }

                if (l.StartsWith("do"))
                {
                    blocks.Push(new ControlBlock { Type = "DO", StartIP = currentInst });
                    continue;
                }

                if (l.StartsWith("} while") || l.StartsWith("}while"))
                {
                    if (blocks.Count > 0 && blocks.Peek().Type == "DO")
                    {
                        var block = blocks.Pop();

                        foreach (int fix in block.ContinueFixups)
                            resultAsm[fix] = $"JMP 0 0 {currentInst}";

                        int startP = l.IndexOf('(');
                        int endP = l.LastIndexOf(')');
                        if (startP != -1 && endP != -1)
                        {
                            string condition = l.Substring(startP + 1, endP - startP - 1);
                            string condDest = CompileExpression(condition, ref tempRegStart, preWriteConstants, resultAsm, ref currentInst);
                            condDest = condDest.Replace("R_", "");

                            resultAsm.Add($"JMPT {condDest} 0 {block.StartIP}");
                            currentInst++;
                        }

                        foreach (int fix in block.BreakFixups)
                            resultAsm[fix] = $"JMP 0 0 {currentInst}";
                    }
                    continue;
                }

                if (l.StartsWith("delay("))
                {
                    int startP = l.IndexOf('(');
                    int endP = l.IndexOf(')');
                    if (startP != -1 && endP != -1)
                    {
                        string delayTimeStr = l.Substring(startP + 1, endP - startP - 1).Trim();
                        if (int.TryParse(delayTimeStr, out int delayTime))
                        {
                            resultAsm.Add($"DELAY 0 0 {delayTime}");
                            currentInst++;
                        }
                    }
                    continue;
                }

                if (l == "break;")
                {
                    var targetBlock = blocks.FirstOrDefault(b => b.Type == "SWITCH" || b.Type == "WHILE" || b.Type == "FOR" || b.Type == "DO");
                    if (targetBlock != null)
                    {
                        resultAsm.Add("JMP 0 0 0");
                        targetBlock.BreakFixups.Add(currentInst);
                        currentInst++;
                    }
                    continue;
                }

                if (l == "continue;")
                {
                    var targetBlock = blocks.FirstOrDefault(b => b.Type == "WHILE" || b.Type == "FOR" || b.Type == "DO");
                    if (targetBlock != null)
                    {
                        resultAsm.Add("JMP 0 0 0");
                        targetBlock.ContinueFixups.Add(currentInst);
                        currentInst++;
                    }
                    continue;
                }

                if (l == "}")
                {
                    if (blocks.Count > 0)
                    {
                        var block = blocks.Pop();
                        if (block.Type == "WHILE")
                        {
                            foreach (int fix in block.ContinueFixups)
                                resultAsm[fix] = $"JMP 0 0 {block.StartIP}";

                            resultAsm.Add($"JMP 0 0 {block.StartIP}");
                            currentInst++;

                            string[] oldInst = resultAsm[block.FixupIP].Split(' ');
                            resultAsm[block.FixupIP] = $"JMPF {oldInst[1]} 0 {currentInst}";

                            foreach (int fix in block.BreakFixups)
                                resultAsm[fix] = $"JMP 0 0 {currentInst}";
                        }
                        else if (block.Type == "IF")
                        {
                            string[] oldInst = resultAsm[block.FixupIP].Split(' ');
                            resultAsm[block.FixupIP] = $"JMPF {oldInst[1]} 0 {currentInst}";
                            lastClosedIfJmpIdx = block.FixupIP;
                        }
                        else if (block.Type == "ELSE")
                        {
                            string[] oldInst = resultAsm[block.FixupIP].Split(' ');
                            resultAsm[block.FixupIP] = $"JMP 0 0 {currentInst}";
                            lastClosedIfJmpIdx = -1;
                        }
                        else if (block.Type == "SWITCH")
                        {
                            if (currentCaseFailFixup != -1)
                            {
                                string[] oldInst = resultAsm[currentCaseFailFixup].Split(' ');
                                resultAsm[currentCaseFailFixup] = $"JMPF {oldInst[1]} 0 {currentInst}";
                                currentCaseFailFixup = -1;
                            }
                            switchVarStack.Pop();

                            foreach (int fix in block.BreakFixups)
                                resultAsm[fix] = $"JMP 0 0 {currentInst}";
                        }
                        else if (block.Type == "FOR")
                        {
                            foreach (int fix in block.ContinueFixups)
                                resultAsm[fix] = $"JMP 0 0 {currentInst}";

                            string incPart = block.ForIncrement;
                            if (incPart.EndsWith("++"))
                            {
                                string varName = incPart.Replace("++", "").Trim();
                                bool isReal = FormDegisken.VariableMap.ContainsKey(varName) && FormDegisken.VariableMap[varName].Type == "REAL";
                                resultAsm.Add($"{(isReal ? "FINC" : "INC")} {GetAddress(varName)} {GetAddress(varName)}");
                                currentInst++;
                            }
                            else if (incPart.EndsWith("--"))
                            {
                                string varName = incPart.Replace("--", "").Trim();
                                bool isReal = FormDegisken.VariableMap.ContainsKey(varName) && FormDegisken.VariableMap[varName].Type == "REAL";
                                resultAsm.Add($"{(isReal ? "FDEC" : "DEC")} {GetAddress(varName)} {GetAddress(varName)}");
                                currentInst++;
                            }

                            resultAsm.Add($"JMP 0 0 {block.StartIP}");
                            currentInst++;

                            string[] oldInst = resultAsm[block.FixupIP].Split(' ');
                            resultAsm[block.FixupIP] = $"JMPF {oldInst[1]} 0 {currentInst}";

                            foreach (int fix in block.BreakFixups)
                                resultAsm[fix] = $"JMP 0 0 {currentInst}";
                        }
                        else if (block.Type == "FUNC")
                        {
                            resultAsm.Add("RET 0 0 0");
                            currentInst++;

                            string[] oldInst = resultAsm[block.FixupIP].Split(' ');
                            resultAsm[block.FixupIP] = $"JMP 0 0 {currentInst}";
                        }
                        else if (block.Type == "THREAD")
                        {
                            resultAsm.Add("THREAD_END 0 0 0");
                            currentInst++;

                            string[] oldInst = resultAsm[block.FixupIP].Split(' ');
                            resultAsm[block.FixupIP] = $"JMP 0 0 {currentInst}";
                        }
                    }
                    continue;
                }

                if (l.StartsWith("switch"))
                {
                    int startP = l.IndexOf('(');
                    int endP = l.IndexOf(')');
                    if (startP != -1 && endP != -1)
                    {
                        switchVarStack.Push(l.Substring(startP + 1, endP - startP - 1).Trim());
                        blocks.Push(new ControlBlock { Type = "SWITCH" });
                    }
                    continue;
                }
                if (l.StartsWith("case"))
                {
                    if (currentCaseFailFixup != -1)
                    {
                        string[] oldInst = resultAsm[currentCaseFailFixup].Split(' ');
                        resultAsm[currentCaseFailFixup] = $"JMPF {oldInst[1]} 0 {currentInst}";
                    }

                    string valStr = l.Replace("case", "").Replace(":", "").Trim();
                    string switchVar = switchVarStack.Peek();
                    string condDest = CompileExpression($"{switchVar} == {valStr}", ref tempRegStart, preWriteConstants, resultAsm, ref currentInst);
                    condDest = condDest.Replace("R_", "");

                    resultAsm.Add($"JMPF {condDest} 0 0");
                    currentCaseFailFixup = currentInst;
                    currentInst++;
                    continue;
                }

                if (l.Contains("="))
                {
                    int eqIdx = l.IndexOf('=');
                    if (l.Substring(max(0, eqIdx - 1), 2) == "==" || l.Substring(max(0, eqIdx - 1), 2) == "!=" ||
                        l.Substring(max(0, eqIdx - 1), 2) == ">=" || l.Substring(max(0, eqIdx - 1), 2) == "<=")
                        continue;

                    string destVar = l.Substring(0, eqIdx).Trim();
                    string expr = l.Substring(eqIdx + 1).Trim().Replace(";", "");

                    if (expr.StartsWith("\"") && expr.EndsWith("\""))
                    {
                        string text = expr.Substring(1, expr.Length - 2);
                        int baseAddr = int.Parse(GetAddress(destVar));
                        for (int k = 0; k < text.Length; k += 2)
                        {
                            char c1 = text[k];
                            char c2 = (k + 1 < text.Length) ? text[k + 1] : '\0';
                            int combined = (c1 << 8) | c2;

                            int constReg = tempRegStart++;
                            preWriteConstants[constReg] = new ConstantInfo { Value = combined, IsReal = false };
                            resultAsm.Add($"MOV {constReg} {baseAddr + (k / 2)}");
                            currentInst++;
                        }
                        if (text.Length % 2 == 0)
                        {
                            int constReg = tempRegStart++;
                            preWriteConstants[constReg] = new ConstantInfo { Value = 0, IsReal = false };
                            resultAsm.Add($"MOV {constReg} {baseAddr + (text.Length / 2)}");
                            currentInst++;
                        }
                        continue;
                    }

                    var matches = System.Text.RegularExpressions.Regex.Matches(expr, @"([a-zA-Z0-9_]+)\[([a-zA-Z0-9_]+)\]");
                    foreach (System.Text.RegularExpressions.Match match in matches)
                    {
                        string arrName = match.Groups[1].Value;
                        string offsetName = match.Groups[2].Value;

                        int baseAddrVal = int.Parse(GetAddress(arrName));
                        int baseReg = tempRegStart++;
                        preWriteConstants[baseReg] = new ConstantInfo { Value = baseAddrVal, IsReal = false };

                        string offsetReg;
                        if (int.TryParse(offsetName, out int offsetVal))
                        {
                            int offR = tempRegStart++;
                            preWriteConstants[offR] = new ConstantInfo { Value = offsetVal, IsReal = false };
                            offsetReg = offR.ToString();
                        }
                        else
                        {
                            offsetReg = GetAddress(offsetName);
                        }

                        int valReg = tempRegStart++;
                        string arrType = FormDegisken.VariableMap.ContainsKey(arrName) ? FormDegisken.VariableMap[arrName].Type : "INT";

                        string opc = "LOAD_ARR";
                        if (arrType == "REAL") opc = "LOAD_ARR_REAL";
                        else if (arrType == "STRING") opc = "LOAD_CHAR";

                        resultAsm.Add($"{opc} {baseReg} {offsetReg} {valReg}");
                        currentInst++;

                        string replacement = arrType == "REAL" ? ("FREG_" + valReg.ToString()) : ("REG_" + valReg.ToString());
                        expr = expr.Replace(match.Value, replacement);
                    }

                    if (destVar.Contains("[") && destVar.EndsWith("]"))
                    {
                        var match = System.Text.RegularExpressions.Regex.Match(destVar, @"^([a-zA-Z0-9_]+)\[([a-zA-Z0-9_]+)\]$");
                        if (match.Success)
                        {
                            string arrName = match.Groups[1].Value;
                            string offsetName = match.Groups[2].Value;

                            string finalNodeAddr = CompileExpression(expr, ref tempRegStart, preWriteConstants, resultAsm, ref currentInst);
                            string finalNodeAddrStripped = finalNodeAddr.Replace("R_", "");

                            string destType = FormDegisken.VariableMap.ContainsKey(arrName) ? FormDegisken.VariableMap[arrName].Type : "INT";
                            string finalType = (finalNodeAddr.StartsWith("R_") || destType == "REAL") ? "REAL" : "INT";

                            int baseAddrVal = int.Parse(GetAddress(arrName));
                            int baseReg = tempRegStart++;
                            preWriteConstants[baseReg] = new ConstantInfo { Value = baseAddrVal, IsReal = false };

                            string offsetReg;
                            if (int.TryParse(offsetName, out int offsetVal))
                            {
                                int offR = tempRegStart++;
                                preWriteConstants[offR] = new ConstantInfo { Value = offsetVal, IsReal = false };
                                offsetReg = offR.ToString();
                            }
                            else
                            {
                                offsetReg = GetAddress(offsetName);
                            }

                            if (finalType == "INT" && destType == "REAL")
                            {
                                int convertedReg = tempRegStart++;
                                resultAsm.Add($"ITOF {finalNodeAddrStripped} {convertedReg}"); currentInst++;
                                finalNodeAddrStripped = convertedReg.ToString();
                            }
                            else if (finalType == "REAL" && destType == "INT")
                            {
                                int convertedReg = tempRegStart++;
                                resultAsm.Add($"FTOI {finalNodeAddrStripped} {convertedReg}"); currentInst++;
                                finalNodeAddrStripped = convertedReg.ToString();
                            }

                            string opc = "STORE_ARR";
                            if (destType == "REAL") opc = "STORE_ARR_REAL";
                            else if (destType == "STRING") opc = "STORE_CHAR";

                            resultAsm.Add($"{opc} {finalNodeAddrStripped} {offsetReg} {baseReg}");
                            currentInst++;
                            continue;
                        }
                    }

                    string destAddr = GetAddress(destVar);
                    // YENİ: Eğer atama yapılacak hedef FREG_ ise tipi otomatik REAL (Float) olarak ayarla
                    string destType2 = "INT";
                    if (destVar.ToUpper().StartsWith("FREG_")) destType2 = "REAL";
                    else if (FormDegisken.VariableMap.ContainsKey(destVar)) destType2 = FormDegisken.VariableMap[destVar].Type;
                    string finalNodeAddr2 = CompileExpression(expr, ref tempRegStart, preWriteConstants, resultAsm, ref currentInst);
                    string finalType2 = (finalNodeAddr2.StartsWith("R_") || destType2 == "REAL") ? "REAL" : "INT";
                    finalNodeAddr2 = finalNodeAddr2.Replace("R_", "");

                    if (finalType2 == "INT" && destType2 == "REAL") { resultAsm.Add($"ITOF {finalNodeAddr2} {destAddr}"); currentInst++; }
                    else if (finalType2 == "REAL" && destType2 == "INT") { resultAsm.Add($"FTOI {finalNodeAddr2} {destAddr}"); currentInst++; }
                    else { resultAsm.Add($"{(destType2 == "REAL" ? "FMOV" : "MOV")} {finalNodeAddr2} {destAddr}"); currentInst++; }
                }
                else if (l.EndsWith("++;"))
                {
                    string varName = l.Replace("++;", "").Trim();
                    bool isReal = FormDegisken.VariableMap.ContainsKey(varName) && FormDegisken.VariableMap[varName].Type == "REAL";
                    resultAsm.Add($"{(isReal ? "FINC" : "INC")} {GetAddress(varName)} {GetAddress(varName)}");
                    currentInst++;
                }
                else if (l.EndsWith("--;"))
                {
                    string varName = l.Replace("--;", "").Trim();
                    bool isReal = FormDegisken.VariableMap.ContainsKey(varName) && FormDegisken.VariableMap[varName].Type == "REAL";
                    resultAsm.Add($"{(isReal ? "FDEC" : "DEC")} {GetAddress(varName)} {GetAddress(varName)}");
                    currentInst++;
                }
            }

            return string.Join("\n", resultAsm);
        }

        private int max(int a, int b) { return a > b ? a : b; }

        private string CompileExpression(string expr, ref int tempRegStart, Dictionary<int, ConstantInfo> consts, List<string> asmOut, ref int currInst)
        {
            string paddedExpr = expr.Replace("==", " == ").Replace("!=", " != ")
                                    .Replace(">=", " >= ").Replace("<=", " <= ")
                                    .Replace("<<", " << ").Replace(">>", " >> ")
                                    .Replace("&&", " & ").Replace("||", " | ")
                                    .Replace("true", " 1 ").Replace("false", " 0 ");

            string[] singleOps = { "(", ")", "+", "-", "*", "/", "%", "&", "|", "^", ">", "<", "~" };
            foreach (string op in singleOps)
                if (!paddedExpr.Contains(op + "=") && !paddedExpr.Contains("=" + op) && !paddedExpr.Contains(op + op))
                    paddedExpr = paddedExpr.Replace(op, $" {op} ");

            paddedExpr = paddedExpr.Replace("  ", " ").Replace("<  =", "<=").Replace(">  =", ">=").Replace("=  =", "==").Replace("!  =", "!=");
            string[] tokens = paddedExpr.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            List<string> refinedTokens = new List<string>();
            string prevToken = "";
            foreach (var t in tokens)
            {
                if (t == "-" && (prevToken == "" || prevToken == "(" || singleOps.Contains(prevToken) || prevToken == "==" || prevToken == "!="))
                    refinedTokens.Add("~NEG~");
                else refinedTokens.Add(t);
                prevToken = t;
            }

            List<string> funcs = new List<string> { "sin", "cos", "tan", "abs", "sqrt", "strlen" };

            Dictionary<string, int> prec = new Dictionary<string, int> {
                {"~NEG~", 6}, {"~", 6}, {"*", 5}, {"/", 5}, {"%", 5}, {"+", 4}, {"-", 4}, {"<<", 3}, {">>", 3},
                {">", 2}, {"<", 2}, {">=", 2}, {"<=", 2}, {"==", 1}, {"!=", 1}, {"&", 0}, {"|", 0}, {"^", 0}
            };

            List<string> output = new List<string>(); Stack<string> opsStack = new Stack<string>();

            foreach (var token in refinedTokens)
            {
                if (funcs.Contains(token))
                {
                    opsStack.Push(token);
                }
                else if (prec.ContainsKey(token))
                {
                    while (opsStack.Count > 0 && opsStack.Peek() != "(" && prec.ContainsKey(opsStack.Peek()) && prec[opsStack.Peek()] >= prec[token])
                        output.Add(opsStack.Pop());
                    opsStack.Push(token);
                }
                else if (token == "(") opsStack.Push(token);
                else if (token == ")")
                {
                    while (opsStack.Count > 0 && opsStack.Peek() != "(") output.Add(opsStack.Pop());
                    if (opsStack.Count > 0) opsStack.Pop();
                    if (opsStack.Count > 0 && funcs.Contains(opsStack.Peek())) output.Add(opsStack.Pop());
                }
                else output.Add(token);
            }
            while (opsStack.Count > 0) output.Add(opsStack.Pop());

            Stack<AstNode> evalStack = new Stack<AstNode>();
            foreach (var token in output)
            {
                if (prec.ContainsKey(token) || funcs.Contains(token))
                {
                    if (token == "~NEG~" || token == "~")
                    {
                        AstNode unaryOp = evalStack.Pop();
                        bool isReal = unaryOp.Type == "REAL";
                        string unaryTempDest = tempRegStart.ToString(); tempRegStart += isReal ? 2 : 1;
                        string zeroAddr = tempRegStart.ToString(); consts[tempRegStart] = new ConstantInfo { Value = 0.0f, IsReal = isReal }; tempRegStart += isReal ? 2 : 1;

                        if (token == "~NEG~") asmOut.Add($"{(isReal ? "FSUB" : "SUB")} {zeroAddr} {unaryOp.Addr} {unaryTempDest}");
                        else asmOut.Add($"NOT {unaryOp.Addr} {unaryTempDest}");
                        currInst++;
                        evalStack.Push(new AstNode { Addr = unaryTempDest, Type = isReal ? "REAL" : "INT" });
                        continue;
                    }
                    else if (funcs.Contains(token))
                    {
                        AstNode unaryOp = evalStack.Pop();
                        bool isReal = unaryOp.Type == "REAL";

                        if (token == "strlen")
                        {
                            string unaryTempDest = tempRegStart.ToString(); tempRegStart += 1; // INT Döner
                            asmOut.Add($"STRLEN {unaryOp.Addr} 0 {unaryTempDest}");
                            currInst++;
                            evalStack.Push(new AstNode { Addr = unaryTempDest, Type = "INT" });
                        }
                        else if (token == "abs")
                        {
                            string unaryTempDest = tempRegStart.ToString(); tempRegStart += isReal ? 2 : 1;
                            string opc = isReal ? "FABS" : "ABS";
                            asmOut.Add($"{opc} {unaryOp.Addr} 0 {unaryTempDest}");
                            currInst++;
                            evalStack.Push(new AstNode { Addr = unaryTempDest, Type = isReal ? "REAL" : "INT" });
                        }
                        else
                        {
                            string opAddr = unaryOp.Addr;
                            if (!isReal)
                            {
                                string nA = tempRegStart.ToString(); tempRegStart += 2;
                                asmOut.Add($"ITOF {opAddr} {nA}"); currInst++;
                                opAddr = nA;
                            }
                            string unaryTempDest = tempRegStart.ToString(); tempRegStart += 2;
                            string opc = "";
                            if (token == "sin") opc = "FSIN";
                            else if (token == "cos") opc = "FCOS";
                            else if (token == "tan") opc = "FTAN";
                            else if (token == "sqrt") opc = "FSQRT";

                            asmOut.Add($"{opc} {opAddr} 0 {unaryTempDest}");
                            currInst++;
                            evalStack.Push(new AstNode { Addr = unaryTempDest, Type = "REAL" });
                        }
                        continue;
                    }

                    AstNode rightNode = evalStack.Pop(); AstNode leftNode = evalStack.Pop();

                    if (leftNode.Type == "INT" && rightNode.Type == "REAL")
                    {
                        string nA = tempRegStart.ToString(); tempRegStart += 2;
                        asmOut.Add($"ITOF {leftNode.Addr} {nA}"); currInst++;
                        leftNode = new AstNode { Addr = nA, Type = "REAL" };
                    }
                    else if (leftNode.Type == "REAL" && rightNode.Type == "INT")
                    {
                        string nA = tempRegStart.ToString(); tempRegStart += 2;
                        asmOut.Add($"ITOF {rightNode.Addr} {nA}"); currInst++;
                        rightNode = new AstNode { Addr = nA, Type = "REAL" };
                    }

                    bool isRealOp = (leftNode.Type == "REAL");
                    bool isCompare = token == "==" || token == "!=" || token == ">" || token == "<" || token == ">=" || token == "<=";

                    string asmOp = "ADD";
                    if (isRealOp)
                    {
                        if (token == "+") asmOp = "FADD";
                        else if (token == "-") asmOp = "FSUB";
                        else if (token == "*") asmOp = "FMUL";
                        else if (token == "/") asmOp = "FDIV";
                        else if (token == "==") asmOp = "FEQ";
                        else if (token == "!=") asmOp = "FNEQ";
                        else if (token == ">") asmOp = "FGT";
                        else if (token == "<") asmOp = "FLT";
                        else if (token == ">=") asmOp = "FGTE"; else if (token == "<=") asmOp = "FLTE";
                    }
                    else
                    {
                        if (token == "+") asmOp = "ADD";
                        else if (token == "-") asmOp = "SUB";
                        else if (token == "*") asmOp = "MUL";
                        else if (token == "/") asmOp = "DIV";
                        else if (token == "%") asmOp = "MOD";
                        else if (token == "&") asmOp = "AND";
                        else if (token == "|") asmOp = "OR";
                        else if (token == "^") asmOp = "XOR";
                        else if (token == "==") asmOp = "EQ";
                        else if (token == "!=") asmOp = "NEQ";
                        else if (token == ">=") asmOp = "GTE";
                        else if (token == "<=") asmOp = "LTE";
                        else if (token == ">") asmOp = "GT"; else if (token == "<") asmOp = "LT";
                    }

                    string resType = isCompare ? "INT" : (isRealOp ? "REAL" : "INT");
                    string binTempDest = tempRegStart.ToString(); tempRegStart += (resType == "REAL") ? 2 : 1;

                    asmOut.Add($"{asmOp} {leftNode.Addr} {rightNode.Addr} {binTempDest}"); currInst++;
                    evalStack.Push(new AstNode { Addr = binTempDest, Type = resType });
                }
                else
                {
                    if (token.StartsWith("REG_"))
                    {
                        evalStack.Push(new AstNode { Addr = token.Substring(4), Type = "INT" });
                    }
                    else if (token.StartsWith("FREG_"))
                    {
                        evalStack.Push(new AstNode { Addr = token.Substring(5), Type = "REAL" });
                    }
                    else if (token.StartsWith("'") && token.EndsWith("'") && token.Length == 3)
                    {
                        int val = token[1];
                        consts[tempRegStart] = new ConstantInfo { Value = val, IsReal = false };
                        evalStack.Push(new AstNode { Addr = tempRegStart.ToString(), Type = "INT" });
                        tempRegStart++;
                    }
                    else if (token.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                    {
                        int val = Convert.ToInt32(token, 16);
                        consts[tempRegStart] = new ConstantInfo { Value = val, IsReal = false };
                        evalStack.Push(new AstNode { Addr = tempRegStart.ToString(), Type = "INT" });
                        tempRegStart++;
                    }
                    else if (float.TryParse(token, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out float val))
                    {
                        bool isRealConst = token.Contains(".");
                        consts[tempRegStart] = new ConstantInfo { Value = val, IsReal = isRealConst };
                        evalStack.Push(new AstNode { Addr = tempRegStart.ToString(), Type = isRealConst ? "REAL" : "INT" });
                        tempRegStart += isRealConst ? 2 : 1;
                    }
                    else
                    {
                        string varType = FormDegisken.VariableMap.ContainsKey(token) ? FormDegisken.VariableMap[token].Type : "INT";
                        evalStack.Push(new AstNode { Addr = GetAddress(token), Type = varType });
                    }
                }
            }
            AstNode fNode = evalStack.Pop();
            return (fNode.Type == "REAL" ? "R_" : "") + fNode.Addr;
        }

        private void writeScript_Click(object sender, EventArgs e)
        {
            if (!modbusClient.Connected) { MessageBox.Show("Gateway'e bağlı değilsiniz!"); return; }
            if (cmbScriptTuru.SelectedItem == null) { MessageBox.Show("Script Türü seçin!"); return; }

            try
            {
                foreach (var variable in FormDegisken.VariableMap.Values)
                {
                    if (!string.IsNullOrEmpty(variable.Value) && int.TryParse(variable.Address, out int address))
                    {
                        try
                        {
                            if (variable.Type == "STRING")
                            {
                                string text = variable.Value.Replace("\"", "");
                                for (int k = 0; k < text.Length; k += 2)
                                {
                                    char c1 = text[k];
                                    char c2 = (k + 1 < text.Length) ? text[k + 1] : '\0';
                                    int combined = (c1 << 8) | c2;
                                    modbusClient.WriteSingleRegister(address + (k / 2), combined);
                                    System.Threading.Thread.Sleep(5);
                                }
                                if (text.Length % 2 == 0)
                                {
                                    modbusClient.WriteSingleRegister(address + (text.Length / 2), 0);
                                    System.Threading.Thread.Sleep(5);
                                }
                                continue;
                            }

                            string valStr = variable.Value.Replace(',', '.').Trim();

                            if (valStr.ToLower() == "true") valStr = "1";
                            else if (valStr.ToLower() == "false") valStr = "0";

                            float numVal = float.Parse(valStr, System.Globalization.CultureInfo.InvariantCulture);
                            if (variable.Type == "REAL") modbusClient.WriteMultipleRegisters(address, FloatToWordArray(numVal));
                            else modbusClient.WriteSingleRegister(address, (int)numVal);
                            System.Threading.Thread.Sleep(5);
                        }
                        catch { }
                    }
                }

                string rawScript = scriptCode.Text.Trim();
                if (string.IsNullOrEmpty(rawScript)) return;
                string seciliTur = cmbScriptTuru.SelectedItem.ToString();

                if (seciliTur == "Assembly")
                    foreach (var variable in FormDegisken.VariableMap)
                        rawScript = rawScript.Replace(variable.Key, variable.Value.Address);

                Dictionary<int, ConstantInfo> preWriteConstants;
                string compiledAssembly = TranspileToAssembly(rawScript, seciliTur, out preWriteConstants);

                Log($"> OLUŞTURULAN ASSEMBLY KODU:\n{compiledAssembly}");

                if (preWriteConstants.Count > 0)
                {
                    int minReg = preWriteConstants.Keys.Min();
                    int maxReg = preWriteConstants.Keys.Max();
                    int totalRegs = maxReg - minReg + 2;
                    int[] constPayload = new int[totalRegs];

                    foreach (var kvp in preWriteConstants)
                    {
                        int offset = kvp.Key - minReg;
                        if (kvp.Value.IsReal)
                        {
                            int[] words = FloatToWordArray(kvp.Value.Value);
                            constPayload[offset] = words[0];
                            constPayload[offset + 1] = words[1];
                        }
                        else
                        {
                            constPayload[offset] = (int)kvp.Value.Value;
                        }
                    }

                    int constChunkSize = 100;
                    for (int i = 0; i < constPayload.Length; i += constChunkSize)
                    {
                        int size = Math.Min(constChunkSize, constPayload.Length - i);
                        int[] chunk = new int[size];
                        Array.Copy(constPayload, i, chunk, 0, size);
                        modbusClient.WriteMultipleRegisters(minReg + i, chunk);
                        System.Threading.Thread.Sleep(30);
                    }
                }

                string[] lines = compiledAssembly.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                List<int> payloadList = new List<int> { lines.Length };
                lastMacroDestinations.Clear();

                foreach (string line in lines)
                {
                    string[] parts = line.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    string cmd = parts[0].ToUpper();
                    int opcode = 0;

                    if (cmd == "ADD") opcode = 1;
                    else if (cmd == "SUB") opcode = 2;
                    else if (cmd == "MUL") opcode = 3;
                    else if (cmd == "DIV") opcode = 4;
                    else if (cmd == "MOD") opcode = 5;
                    else if (cmd == "AND") opcode = 6;
                    else if (cmd == "OR") opcode = 7;
                    else if (cmd == "XOR") opcode = 8;
                    else if (cmd == "EQ") opcode = 16;
                    else if (cmd == "NEQ") opcode = 17;
                    else if (cmd == "GT") opcode = 18;
                    else if (cmd == "LT") opcode = 19;
                    else if (cmd == "GTE") opcode = 20;
                    else if (cmd == "LTE") opcode = 21;
                    else if (cmd == "MOV") opcode = 22;
                    else if (cmd == "INC") opcode = 23;
                    else if (cmd == "DEC") opcode = 24;
                    else if (cmd == "NOT") opcode = 25;
                    else if (cmd == "LOAD_ARR") opcode = 30;
                    else if (cmd == "STORE_ARR") opcode = 31;
                    else if (cmd == "LOAD_ARR_REAL") opcode = 32;
                    else if (cmd == "STORE_ARR_REAL") opcode = 33;
                    else if (cmd == "LOAD_CHAR") opcode = 40;
                    else if (cmd == "STORE_CHAR") opcode = 41;
                    else if (cmd == "FADD") opcode = 51;
                    else if (cmd == "FSUB") opcode = 52;
                    else if (cmd == "FMUL") opcode = 53;
                    else if (cmd == "FDIV") opcode = 54;
                    else if (cmd == "FMOV") opcode = 55;
                    else if (cmd == "FINC") opcode = 56;
                    else if (cmd == "ITOF") opcode = 60;
                    else if (cmd == "FTOI") opcode = 61;
                    else if (cmd == "FEQ") opcode = 62;
                    else if (cmd == "FNEQ") opcode = 63;
                    else if (cmd == "FGT") opcode = 64;
                    else if (cmd == "FLT") opcode = 65;
                    else if (cmd == "FGTE") opcode = 66;
                    else if (cmd == "FLTE") opcode = 67;
                    else if (cmd == "CALL") opcode = 70;
                    else if (cmd == "RET") opcode = 71;
                    else if (cmd == "PUSH") opcode = 72;
                    else if (cmd == "POP") opcode = 73;
                    else if (cmd == "JMP") opcode = 80;
                    else if (cmd == "JMPF") opcode = 81;
                    else if (cmd == "JMPT") opcode = 82;
                    else if (cmd == "ON_ERROR_JMP") opcode = 85;
                    else if (cmd == "DELAY") opcode = 90;
                    else if (cmd == "THREAD_START") opcode = 95;
                    else if (cmd == "THREAD_END") opcode = 96;
                    else if (cmd == "ABS") opcode = 100;
                    else if (cmd == "FABS") opcode = 101;
                    else if (cmd == "FSIN") opcode = 102;
                    else if (cmd == "FCOS") opcode = 103;
                    else if (cmd == "FTAN") opcode = 104;
                    else if (cmd == "FSQRT") opcode = 105;
                    else if (cmd == "STRLEN") opcode = 106;
                    // YENİ EKLENEN KISIM: MODBUS MASTER OPCODELARI
                    else if (cmd == "MODBUS_READ") opcode = 110;
                    else if (cmd == "MODBUS_WRITE") opcode = 111;

                    int src1 = 0, src2 = 0, dest = 0;

                    if ((opcode >= 22 && opcode <= 26) || (opcode >= 55 && opcode <= 57) || opcode == 60 || opcode == 61)
                    {
                        src1 = int.Parse(parts[1]); dest = int.Parse(parts[2]);
                    }
                    else if (opcode >= 100 && opcode <= 106)
                    {
                        src1 = int.Parse(parts[1]); dest = int.Parse(parts[3]);
                    }
                    else if (opcode == 80 || opcode == 70 || opcode == 85 || opcode == 95)
                    {
                        dest = int.Parse(parts[3]);
                    }
                    else if (opcode == 81 || opcode == 82)
                    {
                        src1 = int.Parse(parts[1]); dest = int.Parse(parts[3]);
                    }
                    else if (opcode == 71 || opcode == 96)
                    {
                        // Boş
                    }
                    else if (opcode == 72)
                    {
                        src1 = int.Parse(parts[1]);
                    }
                    else if (opcode == 73 || opcode == 90)
                    {
                        dest = int.Parse(parts[3]);
                    }
                    else
                    {
                        // ADD, SUB, MUL, DIV vb. ve MODBUS_READ, MODBUS_WRITE buraya düşer (3 parametreli)
                        src1 = int.Parse(parts[1]); src2 = int.Parse(parts[2]); dest = int.Parse(parts[3]);
                    }

                    payloadList.Add(opcode); payloadList.Add(src1); payloadList.Add(src2); payloadList.Add(dest);

                    string destType = "INT";
                    if ((opcode >= 51 && opcode <= 60 && opcode != 58 && opcode != 59) || (opcode >= 101 && opcode <= 105)) destType = "REAL";

                    if (!lastMacroDestinations.Any(d => d.Addr == dest) &&
                        opcode != 80 && opcode != 81 && opcode != 82 && opcode != 90 &&
                        opcode != 30 && opcode != 31 && opcode != 32 && opcode != 33 &&
                        opcode != 40 && opcode != 41 &&
                        opcode != 70 && opcode != 71 && opcode != 72 && opcode != 85 &&
                        opcode != 95 && opcode != 96 && opcode != 111) // 111 (MODBUS_WRITE) Register'a yazmaz, dışarıya(cihaza) yazar.
                    {
                        lastMacroDestinations.Add(new DestInfo { Addr = dest, Type = destType });
                    }
                }

                int chunkSize = 100;
                for (int i = 0; i < payloadList.Count; i += chunkSize)
                {
                    int size = Math.Min(chunkSize, payloadList.Count - i);
                    int[] chunk = new int[size];
                    payloadList.CopyTo(i, chunk, 0, size);
                    modbusClient.WriteMultipleRegisters(8000 + i, chunk);
                    System.Threading.Thread.Sleep(20);
                }

                if (rawScript.Contains("sysTick"))
                {
                    modbusClient.WriteSingleRegister(8499, 1);
                    modbusClient.WriteSingleRegister(8500, 1);
                    Log($"> DERLEME BAŞARILI: {seciliTur} scripti çevrildi. (SÜREKLİ MOD AKTİF - Timer Çalışıyor)");
                }
                else
                {
                    modbusClient.WriteSingleRegister(8499, 0);
                    modbusClient.WriteSingleRegister(8500, 1);
                    Log($"> DERLEME BAŞARILI: {seciliTur} scripti çevrildi ve Edge CPU'ya iletildi.");
                }
            }
            catch (Exception ex)
            {
                Log("[DERLEYİCİ HATASI]: " + ex.Message);
            }
        }

        private void btnValue_Click(object sender, EventArgs e)
        {
            FormDegisken frmDegisken = new FormDegisken();
            frmDegisken.ShowDialog();
        }

        private void searchFeature_Click(object sender, EventArgs e)
        {
            if (cmbScriptTuru.SelectedItem == null)
            {
                MessageBox.Show("Lütfen önce bir Script Türü seçin!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            FormOzellik frmOzellik = new FormOzellik(cmbScriptTuru.SelectedItem.ToString());
            frmOzellik.ShowDialog();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}