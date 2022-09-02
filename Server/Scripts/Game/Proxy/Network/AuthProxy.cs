using System;
using PureMVC.Patterns.Proxy;
using Game.Protobuffer;
using Server;
using Net;

namespace Game
{
    public class AuthProxy : Proxy
    {
        public new static string NAME = typeof(AuthProxy).FullName;

        public AuthProxy() : base(NAME) { }
        NetProxy netProxy = null;
        //CharacterProxy characterProxy = null;
        MessageProxy messageProxy = null;
        HandlerProxy handlerProxy = null;

        public override void OnRegister()
        {
            base.OnRegister();
            netProxy = Facade.RetrieveProxy(NetProxy.NAME) as NetProxy;
            messageProxy = Facade.RetrieveProxy(MessageProxy.NAME) as MessageProxy;
            handlerProxy = Facade.RetrieveProxy(HandlerProxy.NAME) as HandlerProxy;

            handlerProxy.RegisterHandler(typeof(C2S_Login_Req), HandleLoginReq);
        }

        public override void OnRemove()
        {
        }



        private void HandleLoginReq(TCPClientState client, object data)
        {
            C2S_Login_Req req = new C2S_Login_Req();

            //查找账号数据 验证

            S2C_LoginAck ack = new S2C_LoginAck();

            messageProxy.SendMessage(client, ack);
        }


        private void HandlePlayerInfoReq(TCPClientState client, object data)
        {
            //查找玩家数据
            S2C_PlayerInfo_Ack ack = new S2C_PlayerInfo_Ack();

            messageProxy.SendMessage(client, ack);
        }

    }
}