using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Socket raw;
            var port = 0;

            IPAddress[] ipaddrs = Dns.GetHostAddresses("127.0.0.1");

            raw = new Socket(AddressFamily.InterNetwork, SocketType.Raw, ProtocolType.IP);
            IPEndPoint local = new IPEndPoint(ipaddrs[0], port);
            raw.Bind(local);

            //raw.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            raw.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.HeaderIncluded, 1);
            byte[] inValue = new byte[] {1, 0, 0, 0};
            byte[] outValue = new byte[] { 0, 0, 0, 0 };
            raw.IOControl(IOControlCode.ReceiveAll, inValue, outValue);

            while (true)
            {
                byte[] buffer = new byte[1500];
                int count = raw.Receive(buffer, SocketFlags.None);
                Console.WriteLine(count);
            }

            ReadStateObject state = new ReadStateObject();
            state.handler = raw;
            raw.BeginReceive(state.buffer,
                0,
                ReadStateObject.MAX_PACKET_SIZE,
                SocketFlags.None,
                new AsyncCallback(ReadCallback),
                state);
            Console.Read();
        }

        private static void ReadCallback(IAsyncResult ar)
        {
            Console.WriteLine("1");
        }

        public static ushort ntohs(ushort netshort)
        {
            return (ushort) System.Net.IPAddress.NetworkToHostOrder((short) netshort);
        }
    }


    class ReadStateObject
    {

        /// 基础网络套接字

        public Socket handler;

        /// 数据缓冲区大小

        public const int MAX_PACKET_SIZE = 65535;

        /// 接收数据的缓冲区

        public byte[] buffer = new byte[MAX_PACKET_SIZE];
    }



    /// IP数据包头结构

    public struct ip_hdr
    {

        /// 4位首部长度+4位IP版本号

        public byte h_lenver;

        /// 8位服务类型TOS

        public byte tos;

        /// 16位总长度（字节）

        public ushort total_len;

        /// 16位标识

        public ushort ident;

        /// 3位标志位

        public ushort frag_and_flags;

        /// 8位生存时间 TTL

        public byte ttl;

        /// 8位协议 (TCP, UDP 或其他)

        public byte proto;

        /// 16位IP首部校验和

        public ushort checksum;

        /// 32位源IP地址

        public uint sourceip;

        /// 32位目的IP地址

        public uint destip;
    }

    /// TCP数据包头结构

    public struct tcp_hdr
    {

        /// 16位源端口

        public ushort th_sport;

        /// 16位目的端口

        public ushort th_dport;

        /// 32位序列号

        public uint th_seq;

        /// 32位确认号

        public uint th_ack;

        /// 4位首部长度/6位保留字

        public byte th_lenres;

        /// 6位标志位

        public byte th_flag;

        /// 16位窗口大小

        public ushort th_win;

        /// 16位校验和

        public ushort th_sum;

        /// 16位紧急数据偏移量

        public ushort th_urp;
    }

    /// UDP数据包头结构

    public struct udp_hdr
    {

        /// 16位源端口

        public ushort uh_sport;

        /// 16位目的端口

        public ushort uh_dport;

        /// 16位长度

        public ushort uh_len;

        /// 16位校验和

        public ushort uh_sum;
    }

    /// ICMP数据包头结构

    public struct icmp_hdr
    {

        /// 8位类型

        public byte i_type;

        /// 8位代码

        public byte i_code;

        /// 16位校验和

        public ushort i_cksum;

        /// 识别号（一般用进程号作为识别号）

        public ushort i_id;

        /// 报文序列号

        public ushort i_seq;

        /// 时间戳

        public uint i_timestamp;
    }
}
