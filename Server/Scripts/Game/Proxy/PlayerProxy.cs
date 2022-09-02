using DataBase;
using PureMVC.Patterns.Proxy;
using System.Collections.Generic;
using Net;

namespace Game
{
    public class PlayerProxy : Proxy
    {

        public new static string NAME = typeof(PlayerProxy).FullName;

        Dictionary<TCPClientState, PlayerVO> datas;

        public PlayerProxy() : base(NAME) { }

        public override void OnRegister()
        {
            datas = new Dictionary<TCPClientState, PlayerVO>();
        }

        public override void OnRemove()
        {
        }





        public PlayerVO GetData(TCPClientState client)
        {
            datas.TryGetValue(client, out PlayerVO data);
            return data;
        }

        public Dictionary<TCPClientState, PlayerVO> GetDatas() => datas;

        public void SetData(PlayerVO data) => datas[data.client] = data;

    }
}
