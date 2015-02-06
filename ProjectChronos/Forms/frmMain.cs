using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Diagnostics;
using System.IO;

using nsAlienRFID2;
using System.Collections.Generic;

using ProjectChronos.Models;
using ProjectChronos.Repositories;

namespace ProjectChronos
{
    public partial class frmMain : Form
    {
        private FinishersRepo finishers = new FinishersRepo();
        private RacersRepo racers = new RacersRepo();

        private class MyStoredReaderState
        {
            internal string userName = null;
            internal string password = null;
            internal int commandPort = 23;
            internal string readerAddress = null;
            internal string readerName = null;
            internal string notifyAddress = null;
            internal string notifyMode = null;
            internal string notifyTime = null;
            internal string tagStreamMode = null;
            internal string ioStreamMode = null;
            internal string tagStreamAddress = null;
            internal string ioStreamAddress = null;
            internal string autoMode = null;
            internal string notifyFormat = null;
            internal string tagStreamFormat = null;
            internal string ioStreamFormat = null;
            internal string tagListMillis = null;
            internal string autoWaitOutput = null;
            internal string autoWorkOutput = null;
            internal string autoTrueOutput = null;
            internal string autoFalseOutput = null;
            internal string ioStreamKeepAliveTime = null;
            internal string tagStreamKeepAliveTime = null;

            internal MyStoredReaderState() { }
        }

        private List<string> tagsCollection = new List<string>();

        private int miReadersCount = 0;

        // SERVERS
        CAlienServer[] mServers = new CAlienServer[3];
        clsReaderMonitor mDiscoverer = null;

        // CONSTANTS PORTS
        const int NOTIFY_PORT = 7797;
        const int TAGSTREAM_PORT = 7798;
        const int IOSTREAM_PORT = 7799;

        Hashtable mClients = new Hashtable();		// readers' IP Addresses as keys; clsReader objects as items
        Hashtable mStoredStates = new Hashtable();	// readers' IP Addresses as keys; MyStoredReaderState objects as items

        static readonly object moCurrentReadersLock = new object();

        private delegate void displayMessageDlgt(string msg);
        private delegate void addNewReaderDlgt(MyStoredReaderState rs);

        private int miOldNodeIndex = -1;	// for showing different tooltips for different nodes

        public frmMain()
        {
            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            cboInterval.SelectedIndex = 4;

            // If you have more than 1 IP Address on your PC use the two parameters' constructor instead.
            mServers[0] = new CAlienServer(7797);	// Notifications
            //mServers[1] = new CAlienServer(7798);	// TagStream
            //mServers[2] = new CAlienServer(7799);	// IOStream

            chlServers.Items.Add("Notify Server", false);
            //chlServers.Items.Add("TagStream Server", false);
            //chlServers.Items.Add("IOStream Server", false);


            mDiscoverer = new clsReaderMonitor();

            mClients = new Hashtable();
            for (int i = 0; i < chlServers.Items.Count; i++)
            {
                addServerEvents(i);
            }
            
            mDiscoverer.ReaderAdded += new nsAlienRFID2 .clsReaderMonitor.ReaderAddedEventHandler(mDiscoverer_ReaderAdded);
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            lock (moCurrentReadersLock)
            {
                foreach (object o in mStoredStates.Keys)
                {
                    string address = o as string;
                    if (mClients.ContainsKey(address))
                    {
                        try
                        {
                            MyStoredReaderState rs = mStoredStates[address] as MyStoredReaderState;

                            clsReader reader = mClients[address] as clsReader;

                            if (!reader.IsConnected)
                                reader.ConnectAndLogin(address, rs.commandPort, rs.userName, rs.password);

                            restoreReaderState(rs, ref reader);
                            reader.StopListen();
                            reader.Dispose();
                            reader = null;
                        }
                        catch { }
                    }
                }
            }
        }

        private void addServerEvents(int idx)
        {
            mServers[idx].ServerMessageReceived += new CAlienServer.ServerMessageReceivedEventHandler(mServer_ServerMessageReceived);
            mServers[idx].ServerConnectionEstablished += new CAlienServer.ServerConnectionEstablishedEventHandler(mServer_ServerConnectionEstablished);
            mServers[idx].ServerConnectionEnded += new CAlienServer.ServerConnectionEndedEventHandler(mServer_ServerConnectionEnded);
            mServers[idx].ServerSocketError += new CAlienServer.ServerSocketErrorEventHandler(mServer_ServerSocketError);
        }

        private void displayText(String data)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    object[] temp = { data };
                    this.Invoke(new displayMessageDlgt(displayText), temp);
                    return;
                }
                else
                {
                    NotifyInfo ni = null;
                    AlienUtils.ParseNotification(data, out ni);
                    if (ni != null)
                    {
                        ITagInfo[] tags = ni.TagList;
                        foreach (TagInfo tag in tags)
                        {
                            if (!tagsCollection.Exists(item => item == tag.TagID))
                            {
                                tagsCollection.Add(tag.TagID);

                                DateTime d;
                                DateTime.TryParse(tag.DiscoveryTime, out d);
                                double timestamp = Helper.ConvertToUnixTimestamp(d);

                                Finisher finisher = new Finisher();
                                Racer racer = racers.findByEpc1(tag.TagID);
                                if (racer == null)
                                {
                                    racer = racers.findByEpc2(tag.TagID);
                                }

                                if (racer != null)
                                {
                                    finisher.racerNo = racer.racerNo;
                                    finisher.epc = tag.TagID;
                                    finisher.time = d.ToString("yyyy-MM-dd HH:mm:ss.ffff");
                                    int result = 0;
                                    if (finisher.racerNo > 0)
                                    {
                                        result = finishers.create(finisher);
                                    }
                                    
                                    if (result > 0)
                                    {
                                        txtNotifications.Text += "FINISHER: " + racer.racerName + " : " + tag.TagID + " " + tag.DiscoveryTime + "\r\n";
                                    }
                                    else
                                    {
                                        txtNotifications.Text += tag.TagID + " " + tag.DiscoveryTime + "\r\n";
                                    }
                                }

                                MessageBox.Show(d.ToString("yyyy-MM-dd HH:mm:ss.ffff"));
                                
                            }
                        }
                    }
                    
                    //txtNotifications.Text += data.Replace("\0", "") + "\r\n";
                    /*
                    NotifyInfo ni = null;
                    AlienUtils.ParseNotification(data, out ni);
                    if (ni != null)
                    {
                        lblReaderName.Text = ni.ReaderName;
                    }*/
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception in the DiscplayText(): " + ex.Message);
            }
        }

        private void decreaseConnectionsCount(string msg)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    object[] temp = { msg };
                    this.Invoke(new displayMessageDlgt(decreaseConnectionsCount), temp);
                    return;
                }
                else
                {
                    int count = int.Parse(lblConnections.Text);
                    if (count > 0)
                        count--;
                    lblConnections.Text = count.ToString();
                    txtNotifications.Text += "Connection ended: " + msg.Replace("\0", "") + "\r\n";
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception in the decreaseConnectionsCount(): " + ex.Message);
            }
        }

        private void increaseConnectionsCount(string msg)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    object[] temp = { msg };
                    this.Invoke(new displayMessageDlgt(increaseConnectionsCount), temp);
                    return;
                }
                else
                {
                    int count = int.Parse(lblConnections.Text);
                    count++;
                    lblConnections.Text = count.ToString();
                    txtNotifications.Text += "Connection added: " + msg.Replace("\0", "") + "\r\n";
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception in the increaseConnectionsCount(): " + ex.Message);
            }
        }

        private void btnAddReader_Click(object sender, EventArgs e)
        {
            FormAddReader dlg = new FormAddReader();
            if (dlg.ShowDialog() != DialogResult.OK)
                return;

            string sIPaddress = dlg.txtAddress.Text.Trim();
            string sPort = dlg.txtPort.Text;
            string sUsername = dlg.txtUsername.Text;
            string sPassword = dlg.txtPassword.Text;

            MyStoredReaderState rs = new MyStoredReaderState();
            rs.readerAddress = sIPaddress;
            try
            {
                rs.commandPort = int.Parse(sPort);
                rs.userName = sUsername;
                rs.password = sPassword;

                lock (moCurrentReadersLock)
                {
                    if (!mStoredStates.ContainsKey(sIPaddress))
                        mStoredStates.Add(sIPaddress, rs);
                    else
                        mStoredStates[sIPaddress] = rs;

                    addNewReader(rs);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtNotifications.Clear();
        }

        private void mServer_ServerMessageReceived(string msg)
        {
            if (msg != null)
            {
                try
                {
                    //var tag = AlienUtils.ParseTagData(msg);

                    //MessageBox.Show(tag.TagID);
                    
                }
                catch (Exception ex)
                {

                }
                //displayText(msg);
                displayText(msg.Insert(msg.IndexOf(" ") + 1, "\r\n"));
            }
        }

        private void mServer_ServerConnectionEstablished(string id)
        {
            increaseConnectionsCount(id);
        }

        private void mServer_ServerConnectionEnded(string id)
        {
            decreaseConnectionsCount(id);
        }

        private void mServer_ServerSocketError(string msg)
        {
            displayText(msg);
        }

        private void chkAddDiscovered_CheckedChanged(object sender, EventArgs e)
        {
            if (chkAddDiscovered.Checked)
                mDiscoverer.StartListening();
            else
                mDiscoverer.StopListening();
        }

        private void mDiscoverer_ReaderAdded(IReaderInfo data)
        {
            if (mStoredStates.ContainsKey(data.IPAddress))
                return;

            MyStoredReaderState rs = new MyStoredReaderState();
            rs.readerAddress = data.IPAddress;
            rs.commandPort = data.TelnetPort;
            rs.readerName = data.Name;
            lock (moCurrentReadersLock)
            {
                if (!mStoredStates.ContainsKey(data.IPAddress))
                    mStoredStates.Add(data.IPAddress, rs);

                addNewReader(rs);
            }
        }

        private void addNewReader(MyStoredReaderState rs)
        {
            if (this.InvokeRequired)
            {
                object[] temp = { rs };
                try
                {
                    this.Invoke(new addNewReaderDlgt(addNewReader), temp);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
                return;
            }
            else
            {
                bool found = false;
                foreach (TreeNode n in treeView1.Nodes)
                {
                    if (n.Text == rs.readerAddress)
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    TreeNode[] nodeChildren = new TreeNode[1];
                    nodeChildren[0] = new TreeNode("Notify");
                    //nodeChildren[1] = new TreeNode("TagStream");
                    //nodeChildren[2] = new TreeNode("IOStream");
                    TreeNode node = new TreeNode(rs.readerAddress, nodeChildren);
                    node.Tag = rs.readerAddress;

                    miReadersCount++;

                    treeView1.Nodes.Add(node);
                }
                lblReadersCount.Text = miReadersCount.ToString();
            }
        }

        private void connect(MyStoredReaderState myReaderState, ref clsReader reader)
        {
            if (myReaderState.userName == null)
                myReaderState.userName = "alien";
            if (myReaderState.password == null)
                myReaderState.password = "password";

            lock (moCurrentReadersLock)
            {
                reader.ConnectAndLogin(
                    myReaderState.readerAddress,
                    myReaderState.commandPort,
                    myReaderState.userName,
                    myReaderState.password);
            }
        }

        private void saveReaderState(MyStoredReaderState myReaderState)
        {
            string sIPaddress = myReaderState.readerAddress;

            clsReader reader = null;
            lock (moCurrentReadersLock)
            {
                try
                {
                    if (!mClients.ContainsKey(sIPaddress))
                    {
                        reader = new clsReader();
                        connect(myReaderState, ref reader);
                        if (!reader.IsConnected)
                        {
                            throw new Exception("Can't connect to the reader.");
                        }

                        string r = null;

                        try { r = reader.TagStreamAddress; }
                        catch { }
                        if ((r != null) && (r.IndexOf("Error") == -1))
                        { myReaderState.tagStreamAddress = r; r = null; }

                        try { r = reader.TagStreamMode; }
                        catch { }
                        if ((r != null) && (r.IndexOf("Error") == -1))
                        { myReaderState.tagStreamMode = r; r = null; }

                        try { r = reader.IOStreamAddress; }
                        catch { }
                        if ((r != null) && (r.IndexOf("Error") == -1))
                        { myReaderState.ioStreamAddress = r; r = null; }

                        try { r = reader.IOStreamMode; }
                        catch { }
                        if ((r != null) && (r.IndexOf("Error") == -1))
                        { myReaderState.ioStreamMode = r; r = null; }

                        try { r = reader.NotifyAddress; }
                        catch { }
                        if ((r != null) && r.IndexOf("Error") == -1)
                        { myReaderState.notifyAddress = r; r = null; }

                        try { r = reader.NotifyMode; }
                        catch { }
                        if ((r != null) && r.IndexOf("Error") == -1)
                        { myReaderState.notifyMode = r; r = null; }

                        try { r = reader.NotifyTime; }
                        catch { }
                        if ((r != null) && r.IndexOf("Error") == -1)
                        { myReaderState.notifyTime = r; r = null; }

                        try { r = reader.NotifyFormat; }
                        catch { }
                        if ((r != null) && (r.IndexOf("Error") == -1))
                        { myReaderState.notifyFormat = r; r = null; }

                        try { r = reader.TagStreamFormat; }
                        catch { }
                        if ((r != null) && (r.IndexOf("Error") == -1))
                        { myReaderState.tagStreamFormat = r; r = null; }

                        try { r = reader.IOStreamFormat; }
                        catch { }
                        if ((r != null) && (r.IndexOf("Error") == -1))
                        { myReaderState.ioStreamFormat = r; r = null; }

                        try { r = reader.TagListMillis; }
                        catch { }
                        if ((r != null) && (r.IndexOf("Error") == -1))
                        { myReaderState.tagListMillis = r; r = null; }

                        try { r = reader.AutoWaitOutput; }
                        catch { }
                        if ((r != null) && (r.IndexOf("Error") == -1))
                        { myReaderState.autoWaitOutput = r; r = null; }

                        try { r = reader.AutoWorkOutput; }
                        catch { }
                        if ((r != null) && (r.IndexOf("Error") == -1))
                        { myReaderState.autoWorkOutput = r; r = null; }

                        try { r = reader.AutoTrueOutput; }
                        catch { }
                        if ((r != null) && (r.IndexOf("Error") == -1))
                        { myReaderState.autoTrueOutput = r; r = null; }

                        try { r = reader.AutoFalseOutput; }
                        catch { }
                        if ((r != null) && (r.IndexOf("Error") == -1))
                        { myReaderState.autoFalseOutput = r; r = null; }

                        try { r = reader.IOStreamKeepAliveTime; }
                        catch { }
                        if ((r != null) && (r.IndexOf("Error") == -1))
                        { myReaderState.ioStreamKeepAliveTime = r; r = null; }

                        try { r = reader.TagStreamKeepAliveTime; }
                        catch { }
                        if ((r != null) && (r.IndexOf("Error") == -1))
                        { myReaderState.tagStreamKeepAliveTime = r; r = null; }

                        try { r = reader.AutoMode; }
                        catch { }
                        if ((r != null) && (r.IndexOf("Error") == -1))
                        { myReaderState.autoMode = r; r = null; }

                        mClients.Add(sIPaddress, reader);
                    }
                }
                catch (Exception ex)
                {
                    if (mClients.ContainsKey(sIPaddress))
                        mClients.Remove(sIPaddress);

                    if ((reader != null) && (reader.IsConnected))
                        reader.Dispose();
                }
            }// releasing lock
        }

        private void restoreReaderState(MyStoredReaderState rs, ref clsReader reader)
        {
            try { reader.AutoMode = "Off"; }
            catch { }
            if (rs.notifyMode != null)
                try { reader.NotifyMode = rs.notifyMode; }
                catch { }
            if (rs.tagStreamMode != null)
                try { reader.TagStreamMode = rs.tagStreamMode; }
                catch { }
            if (rs.ioStreamMode != null)
                try { reader.IOStreamMode = rs.ioStreamMode; }
                catch { }
            if (rs.notifyTime != null)
                try { reader.NotifyTime = rs.notifyTime; }
                catch { }
            if (rs.notifyAddress != null)
                try { reader.NotifyAddress = rs.notifyAddress; }
                catch { }
            if (rs.tagStreamAddress != null)
                try { reader.TagStreamAddress = rs.tagStreamAddress; }
                catch { }
            if (rs.ioStreamAddress != null)
                try { reader.IOStreamAddress = rs.ioStreamAddress; }
                catch { }
            if (rs.notifyFormat != null)
                try { reader.NotifyFormat = rs.notifyFormat; }
                catch { }
            if (rs.tagStreamFormat != null)
                try { reader.TagStreamFormat = rs.tagStreamFormat; }
                catch { }
            if (rs.ioStreamFormat != null)
                try { reader.IOStreamFormat = rs.ioStreamFormat; }
                catch { }
            if (rs.tagListMillis != null)
                try { reader.TagListMillis = rs.tagListMillis; }
                catch { }
            if (rs.autoWaitOutput != null)
                try { reader.AutoWaitOutput = rs.autoWaitOutput; }
                catch { }
            if (rs.autoWorkOutput != null)
                try { reader.AutoWorkOutput = rs.autoWorkOutput; }
                catch { }
            if (rs.autoTrueOutput != null)
                try { reader.AutoTrueOutput = rs.autoTrueOutput; }
                catch { }
            if (rs.autoFalseOutput != null)
                try { reader.AutoFalseOutput = rs.autoFalseOutput; }
                catch { }
            if (rs.ioStreamKeepAliveTime != null)
                try { reader.IOStreamKeepAliveTime = rs.ioStreamKeepAliveTime; }
                catch { }
            if (rs.tagStreamKeepAliveTime != null)
                try { reader.TagStreamKeepAliveTime = rs.tagStreamKeepAliveTime; }
                catch { }
            if (rs.autoMode != null)
                try { reader.AutoMode = rs.autoMode; }
                catch { }
        }

        private void chlServers_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void chlServers_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;

            if (e.NewValue == CheckState.Checked)
                mServers[e.Index].StartListening();
            else
                mServers[e.Index].StopListening();

            this.Cursor = Cursors.Default;
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            
        }

        private void treeView1_MouseMove(object sender, MouseEventArgs e)
        {
            TreeNode tn = this.treeView1.GetNodeAt(e.X, e.Y);

            if (tn != null)
            {
                int currentNodeIndex = tn.Index;
                if (currentNodeIndex != miOldNodeIndex)
                {
                    miOldNodeIndex = currentNodeIndex;
                    if (this.toolTip1 != null && this.toolTip1.Active)
                        this.toolTip1.Active = false; //turn it off 

                    string textToShow = "";
                    if (tn.Tag == null)
                    {
                        if (tn.Checked)
                            textToShow = "Uncheck to set the " + tn.Text + "Mode Off.";
                        else
                            textToShow = "Check to set the " + tn.Text + "Mode On";
                    }
                    else if (!tn.Checked)
                    {
                        textToShow = "Check to configure reader.";
                    }
                    else
                        textToShow = "";

                    this.toolTip1.SetToolTip(this.treeView1, textToShow);
                    this.toolTip1.Active = true; //make it active so it can show 
                }
            } 
        }

        private void treeView1_AfterCheck(object sender, TreeViewEventArgs e)
        {
            string tag = e.Node.Tag as string;
            if ((tag != null) && (tag != ""))
            {
                if (e.Node.Checked)
                {
                    lock (moCurrentReadersLock)
                    {
                        if (!mClients.ContainsKey(tag))	// configuring reader for the first time
                        {
                            saveReaderState(mStoredStates[tag] as MyStoredReaderState);
                            clsReader reader = null;
                            reader = mClients[tag] as clsReader;
                            if ((reader == null) || (!reader.IsConnected))
                            {
                                MessageBox.Show("Can't connect to the reader.");
                                treeView1.Nodes.Remove(e.Node);
                                lblReadersCount.Text = (--miReadersCount).ToString();
                            }
                            else
                            {
                                this.Cursor = Cursors.WaitCursor;

                                // any of these can throw, especially for older readers
                                try { reader.NotifyAddress = mServers[0].NotificationHost; }
                                catch { }
                                try { reader.TagStreamAddress = mServers[1].NotificationHost; }
                                catch { }
                                try { reader.IOStreamAddress = mServers[2].NotificationHost; }
                                catch { }
                                try { reader.TagListMillis = "On"; }
                                catch { }
                                try { reader.AutoMode = "On"; }
                                catch { }

                                // cusotmize these properties (and any of other related ones) as you desire
                                try { reader.NotifyTime = cboInterval.Text; }
                                catch { }
                                try { reader.NotifyFormat = "Text"; }
                                catch { }
                                try { reader.NotifyHeader = "Off"; }
                                catch { }
                                try { reader.TagStreamFormat = "Terse"; }
                                catch { }
                                try { reader.IOStreamFormat = "Text"; }
                                catch { }
                                try { reader.IOStreamKeepAliveTime = "30"; }
                                catch { }
                                try { reader.TagStreamKeepAliveTime = "30"; }
                                catch { }
                                try { reader.AutoWaitOutput = "-1"; }
                                catch { }
                                try { reader.AutoWorkOutput = "-1"; }
                                catch { }
                                try { reader.AutoTrueOutput = "-1"; }
                                catch { }
                                try { reader.AutoFalseOutput = "-1"; }
                                catch { }
                                try { reader.TimeZone = "8"; }
                                catch { }

                                this.Cursor = Cursors.Default;
                            }
                            e.Node.Expand();
                        }
                    }// release lock
                }
                else
                {
                    e.Node.Checked = true;
                }
            }
            else	// Start / Stop automatic messages from the reader
            {
                tag = e.Node.Parent.Tag as String;
                lock (moCurrentReadersLock)
                {
                    if (!mClients.ContainsKey(tag))
                        e.Node.Parent.Checked = true;

                    clsReader reader = mClients[tag] as clsReader;
                    if ((reader != null) && !(reader.IsConnected))
                    {
                        connect((mStoredStates[tag] as MyStoredReaderState), ref reader);
                        if (!reader.IsConnected)
                        {
                            MessageBox.Show("Can't connect to the reader.");
                            treeView1.Nodes.Remove(e.Node.Parent);
                            lblReadersCount.Text = (--miReadersCount).ToString();
                            return;
                        }
                    }
                    string cmd = e.Node.Text + "Mode";
                    string strToSend = "Set " + cmd + " = " + (e.Node.Checked ? "On" : "Off");

                    try { reader.SendReceive(strToSend, false); }
                    catch (Exception ex)
                    {
                        if (e.Node.Checked)
                        {
                            MessageBox.Show(ex.Message);
                            e.Node.Checked = false;
                        }
                    }
                } // release lock
            }
        }

        private void btnRacerList_Click(object sender, EventArgs e)
        {
            Forms.frmRacerList frm = new Forms.frmRacerList();
            frm.Show();
        }

        private void btnFinishers_Click(object sender, EventArgs e)
        {
            Forms.FinisherList frm = new Forms.FinisherList();
            frm.Show();
        }



    }
}
