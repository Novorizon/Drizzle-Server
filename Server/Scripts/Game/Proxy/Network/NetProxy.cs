using Net;
using System;
using System.Net;
using System.Net.Sockets;
using PureMVC.Patterns.Proxy;
using Game.Protobuffer;

namespace Server
{
    public class NetProxy : Proxy
    {
        public new static string NAME = typeof(NetProxy).FullName;

        private TcpListener listener = new TcpListener(IPAddress.Any, 6688);

        ClientProxy clientProxy = null;
        HandlerProxy handlerProxy = null;
        MessageProxy messageProxy = null;

        public bool IsRunning { get; private set; }

        public NetProxy() : base(NAME) { }

        public override void OnRegister()
        {
            base.OnRegister();

            listener = new TcpListener(IPAddress.Any, 6688);
            clientProxy = Facade.RetrieveProxy(ClientProxy.NAME) as ClientProxy;
            handlerProxy = Facade.RetrieveProxy(HandlerProxy.NAME) as HandlerProxy;
            messageProxy = Facade.RetrieveProxy(MessageProxy.NAME) as MessageProxy;


        }


        public override void OnRemove()
        {

        }




        public void StartServer()
        {
            if (!IsRunning)
            {
                IsRunning = true;
                listener.Start();
                listener.BeginAcceptTcpClient(new AsyncCallback(HandleTcpClientAccept), listener);
            }
        }

        private void HandleTcpClientAccept(IAsyncResult ar)
        {
            Log("接收连接");
            if (IsRunning)
            {
                //listener = (TcpListener)ar.AsyncState;

                TcpClient client = listener.EndAcceptTcpClient(ar);
                byte[] buffer = new byte[client.ReceiveBufferSize];

                TCPClientState state = new TCPClientState(client, buffer);
                lock (clientProxy.Clients)
                {
                    Log("lock");
                    clientProxy.Add(state);
                    RaiseClientConnected(state);
                }

                NetworkStream stream = state.NetworkStream;
                stream.BeginRead(state.Buffer, 0, state.Buffer.Length, HandleDataReceived, state);          //开始异步读取数据

                listener.BeginAcceptTcpClient(new AsyncCallback(HandleTcpClientAccept), ar.AsyncState);
            }
        }

        /// 数据接受回调函数
        private void HandleDataReceived(IAsyncResult ar)
        {
            Log(" 数据接受");
            if (IsRunning)
            {
                TCPClientState state = (TCPClientState)ar.AsyncState;
                NetworkStream stream = state.NetworkStream;


                int count = 0;
                try
                {
                    count = stream.EndRead(ar);
                }
                catch
                {
                    count = 0;
                }

                if (count == 0)
                {
                    // connection has been closed
                    lock (clientProxy.Clients)
                    {
                        clientProxy.Remove(state);
                        RaiseClientDisconnected(state); //触发客户端连接断开事件
                        return;
                    }
                }


                messageProxy.ReceiveMessage(state, count);

                //// received byte and trigger event notification
                //RaiseDataReceived(state);//触发数据收到事件

                // continue listening for tcp datagram packets
                stream.BeginRead(state.Buffer, 0, state.Buffer.Length, HandleDataReceived, state);
            }
        }

        /// 触发客户端连接事件
        private void RaiseClientConnected(TCPClientState state)
        {
        }

        /// 触发客户端连接断开事件
        private void RaiseClientDisconnected(TCPClientState state)
        {
            clientProxy.Close(state);
        }


        protected void HandlerFunc(TCPClientState state, object data)
        {
            C2S_Login req = data as C2S_Login;
            Console.WriteLine("C2S_Login");

            S2C_LoginAck ack = new S2C_LoginAck()
            {
                PlayerId = 1234
            };

            messageProxy.SendMessage(state, ack);
        }


        void OnError(string msg)
        {
            //netProxy.CurrentNetState = NetState.disconnected;
            Console.WriteLine(msg);
        }

        void Log(string str)
        {
            Console.WriteLine(str);

        }
    }
}